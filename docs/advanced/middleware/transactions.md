# Transaction

::: warning
Transactions, and using a shared transaction, is an advanced concept. Every scenario is different, so this is more of a guideline than a rule.
:::

The message pipeline in MassTransit is asynchronous, leveraging the Task Parallel Library (TPL) extensively to maximum thread utilization. This means that receiving an individual message may involve several threads over the lifecycle of the consumer. To prevent strange things from happening, developers should avoid using any *static* or *thread static* variables as these are one of the main causes of errors in asynchronous programming.

The .NET `System.Transactions` namespace is a static hound, with many applications following the model of using a transaction scope to wrap a transactional operation.

```cs
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

```cs
Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host("localhost");

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

## Transaction Outbox

While the [In Memory Outbox](/usage/exceptions#outbox) is the best choice for an outbox when within a consumer, sometimes you need to Publish/Send a message onto the bus from outside a consumer. Such scenarios might be from a console app, or maybe an HTTP Endpoint. Below is an example of registration and usage for the Transaction Outbox.

```cs
services.AddMassTransit(x =>
{
    x.AddBus(context => Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        var host = cfg.Host("localhost", "/");
    }));

    // This needs to be after AddBus, because it registers a decorator over IPublishEndpoint and ISendEndpointProvider, and DI Registration order matters (last registration wins)
    x.AddTransactionOutbox();
});
```

That is all that's needed. Now here's an example usage within an MVC Action.  It's also important to use `TransactionScopeAsyncFlowOption.Enabled` as shown below.

```cs
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

```cs
public class Program
{
    public static async Task Main()
    {
        var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
        {
            sbc.Host("rabbitmq://localhost");
        });

        await bus.StartAsync(); // This is important!

        var outbox = new TransactionOutbox(bus, bus, new NullLogger()); // Treat this as a singleton, thread safe.

        while(/*some condition*/)
        {
            using(var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Do whatever business logic you need.

                await outbox.Publish(new ReportQueued{...});
                await outbox.Send(new CalculateReport{...});

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
