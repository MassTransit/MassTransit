---
navigation.title: Transaction
---

# Transaction Filter

::alert{type="warning"}
Transactions, and using a shared transaction, is an advanced concept. Every scenario is different, so this is more of a guideline than a rule.
::

The message pipeline in MassTransit is asynchronous, leveraging the Task Parallel Library (TPL) extensively to maximum thread utilization. This means that receiving an individual message may involve several threads over the life cycle of the consumer. To prevent strange things from happening, developers should avoid using any *static* or *thread static* variables as these are one of the main causes of errors in asynchronous programming.

The .NET `System.Transactions` namespace is a static hound, with many applications following the model of using a transaction scope to wrap a transactional operation.

```csharp
public class Repository
{
    public void Save(Entity entity)
    {
        using(var scope = new TransactionScope())
        {
            SaveEntity(entity);

            scope.Complete();
        }
    }
}
```

In this example, the creation of a `TransactionScope` actually sets a static variable, `Transaction.Current`, to the created or ambient transaction. That word *ambient* should be a big clue — it's using a static variable (in this case, it's actually a thread static, but anyway).

It turns out that the above example is simple, and works, because there are no asynchronous methods. But that also means that the method blocks the thread while the database performs work (which takes an eternity in CPU time). Most databases support asynchronous operations (including Entity Framework), so it is logical to assume that using those methods would increase thread utilization.

It is also often requested that a set of operations be managed as a *unit of work*. A single transaction is shared across multiple operations that are committed as a single unit. If the commit fails, everything is undone and the message is faulted (or retried, if the retry middleware is used).

## Usage

MassTransit includes transaction middleware to share a single committable transaction across any number consumers and any dependencies used by the those consumers. To use the middleware, it must be added to the bus or receive endpoint.

```csharp
Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.ReceiveEndpoint("event_queue", e =>
    {
        e.UseTransaction(x =>
        {
            Timeout = TimeSpan.FromSeconds(90);
            IsolationLevel = IsolationLevel.ReadCommitted;
        });

        e.Consumer<UpdateCustomerAddressConsumer>();
    })
});
```

For each message, a new `CommittableTransaction` is created. This transaction can be passed to classes that support transactional operations, such as `DbContext`, `SqlCommand`, and `SqlConnection`. It can also be used to create any `TransactionScope` that may be required to support a synchronous operation.

To use the transaction directly in a consumer, the transaction can be pulled from the `ConsumeContext`.

```csharp
public class TransactionalConsumer :
    IConsumer<UpdateCustomerAddress>
{
    readonly SqlConnection _connection; // ctor injected

    public async Task Consume(ConsumeContext<UpdateCustomerAddress> context)
    {
        var transactionContext = context.GetPayload<TransactionContext>();

        _connection.EnlistTransaction(transactionContext.Transaction);

        using (SqlCommand command = new SqlCommand(sql, _connection))
        {
            using (var reader = await command.ExecuteReaderAsync())
            {
            }
        }

        // the connection lifetime should be managed by a container
        // or perhaps another more specific middleware component.
    }
}
```

The connection (and by use of the connection, the command) are enlisted in the transaction. Once the method completes, and control is returned to the transaction middleware, if no exceptions are thrown the transaction is committed (which should complete the database operation). If an exception is thrown, the transaction is rolled back.

While not shown here, a class that provides the connection, and enlists the connection upon creation, should be added to the container to ensure that the transaction is not enlisted twice (not sure that's a bad thing though, it should be ignored). Also, as long as only a single connection string is enlisted, the DTC should not get involved. Using the same transaction across multiple connection strings is a bad thing, as it will make the DTC come into play which slows the world down significantly.

## Unit of Work (Buffer)

Sometimes you just have to integrate with Database first systems, but still want some of the perks that message buses have to offer. A good example is an API with your typical HTTP Requests. You want to manipulate your DB, commit, and then upon success, release the messages to the broker. This is NOT a distributed transaction. There's still a risk that you could have the DB up and the broker down, causing the messages to never be sent to the broker. So you've been warned!

There are two options to provide this buffer:

- Transactional Enlistment Bus
- Transactional Bus

## Transactional Enlistment Bus

Transports don't typically support transactions, so sending messages during a transaction only to encounter an exception resulting in a transaction rollback may lead to messages that were sent without the transaction being committed.

::alert{type="info"}
MassTransit has an in-memory outbox to deal with this problem, which can be used within a message consumer. It leverages the durable nature of a message transport to ensure that messages are ultimately sent. There is an extensive article and [video](https://youtu.be/P41IsVAc1nI) explaining the outbox behavior. This approach is preferred to performing transactional database writes outside of a consumer.
::

However, sometimes you are coming from the database first and can't get around it. For those situations, MassTransit has a _very simple_ transactional bus which enlists in the current transaction and defers outgoing messages until the transaction is being committed. There is still no rollback, once the messages are delivered to the broker, there is no pulling them back.


```csharp
services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
    });

    x.AddTransactionalEnlistmentBus();
});
```

That is all that's needed. Now here's an example usage within an MVC Action.  It's also important to use `TransactionScopeAsyncFlowOption.Enabled` as shown below.

```csharp
public class MyController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly MyDbContext _dbContext;

    public ValuesController(IPublishEndpoint publishEndpoint, MyDbContext dbContext)
    {
        _publishEndpoint = publishEndpoint;
        _dbContext = dbContext;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] string value)
    {
        using(var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            _dbContext.Posts.Add(new Post{...});
            await _dbContext.SaveChangesAsync();

            await _publishEndpoint.Publish(new PostCreated{...});

            transaction.Complete();
        }

        return Ok();
    }
}
```

Here's an example from within a Console App, with no Container:

```csharp
public class Program
{
    public static async Task Main()
    {
        var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
        {
            sbc.Host("rabbitmq://localhost");
        });

        await bus.StartAsync(); // This is important!

        var transactionalBus = new TransactionalEnlistmentBus(bus);

        while(/*some condition*/)
        {
            using(var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Do whatever business logic you need.

                await transactionalBus.Publish(new ReportQueued{...});
                await transactionalBus.Send(new CalculateReport{...});

                // Maybe other business logic

                transaction.Complete();
            }
        }

        Console.WriteLine("Press any key to exit");
        await Task.Run(() => Console.ReadKey());

        await bus.StopAsync();
    }
}
```

## Transactional Bus

Here we are again, another option for holding onto the messages and releasing them as close to the database transaction Commit as possible. We made this alternative because using TransactionScope from the previous section, could [in certain cases](https://github.com/MassTransit/MassTransit/issues/2075) still cause a 2 phase commit escalation (not to mention that TransactionScope doesn't truely have async support, so we make [concessions by calling TaskUtil.Await](https://github.com/MassTransit/MassTransit/blob/develop/src/MassTransit/Transactions/TransactionalBusEnlistment.cs#L83)). So to offer an alternative to these drawbacks, MassTransit provides an Outbox Bus.

::alert{type="danger"}
Never use the TransactionalBus or TransactionalEnlistmentBus when writing consumers. These tools are very specific and should be used only in the scenarios described.
::

The examples will show it's usage in an ASP.NET MVC application, which is where we most commonly use Scoped lifetime for our DbContext and therefore we want the same for our TransactionalBus. You could possibly use it in some console applications, but ones WITHOUT a MT Consumer. Once you have consumers you will ALWAYS use `ConsumeContext` to interact with the bus, and never the `IBus`.

First Register the outbox bus.

```csharp
services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
    });

    x.AddTransactionalBus();
});
```

Then use within your controller.

```csharp
public class MyController : ControllerBase
{
    private readonly ITransactionalBus _transactionalBus;
    private readonly MyDbContext _dbContext;

    public ValuesController(ITransactionalBus transactionalBus, MyDbContext dbContext)
    {
        _transactionalBus = transactionalBus;
        _dbContext = dbContext;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] string value)
    {
        using(var transaction = await _dbContext.Database.BeginTransactionAsync())
        {
            try
            {
                _dbContext.Posts.Add(new Post{...});
                await _dbContext.SaveChangesAsync();

                await _transactionalBus.Publish(new PostCreated{...});

                await transaction.CommitAsync();
                await _transactionalBus.Release(); // Immediately after CommitAsync
            }
            catch (Exception)
            {
                transaction.Rollback();
            }

        }

        return Ok();
    }
}
```

One option to remove some of the boilerplate of opening a transaction each Action that writes to the DB is to make a Filter. You can then include all of the boilerplate code to begin the transaction, and release the outbox.

```csharp
public class DbContextTransactionFilter : TypeFilterAttribute
{
    public DbContextTransactionFilter()
        : base(typeof(DbContextTransactionFilterImpl))
    {
    }

    // This will be scoped per http request
    private class DbContextTransactionFilterImpl : IAsyncActionFilter
    {
        private readonly MyDbContext _db;
        private readonly ILogger _logger;
        private readonly ITransactionalBus _transactionalBus;

        public DbContextTransactionFilterImpl(
            MyDbContext db,
            ILogger<DbContextTransactionFilter> logger,
            ITransactionalBus transactionalBus)
        {
            _db = db;
            _logger = logger;
            _transactionalBus = transactionalBus;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var actionExecuted = await next();
                if (actionExecuted.Exception != null && !actionExecuted.ExceptionHandled)
                {
                    await transaction.RollbackAsync();
                }
                else
                {
                    await transaction.CommitAsync();
                    await _transactionalBus.Release(); // Immediately after CommitAsync
                }
            }
            catch (Exception)
            {
                try
                {
                    await transaction.RollbackAsync();
                }
                catch (Exception e)
                {
                    // Swallow failed rollback
                    _logger.LogWarning(e, "Tried to rollback transaction but failed, swallow exception.");
                }

                throw;
            }
        }
    }
}
```

Now your Controller Action will look like:

```csharp
public class MyController : ControllerBase
{
    private readonly ITransactionalBus _transactionalBus;
    private readonly MyDbContext _dbContext;

    public ValuesController(ITransactionalBus transactionalBus, MyDbContext dbContext)
    {
        _transactionalBus = transactionalBus;
        _dbContext = dbContext;
    }

    [HttpPost]
    [DbContextTransactionFilter]
    public async Task<IActionResult> Post([FromBody] string value)
    {
        _dbContext.Posts.Add(new Post{...});
        await _dbContext.SaveChangesAsync();

        await _transactionalBus.Publish(new PostCreated{...});

        return Ok();
    }
}
```
