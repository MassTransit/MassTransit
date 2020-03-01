# Mediator

MassTransit can be used as a mediator with full support for consumers, handlers, and even sagas (including state machines). When using the mediator, all components are instantiated in-process and in-memory. There is no transport required, and messages are not serialized.

::: tip Mediator
Mediator is a [behavioral design pattern](https://en.wikipedia.org/wiki/Mediator_pattern) in which a _mediator_ encapsulates communication between objects to reduce coupling.
:::

### Configuration

To configure and create a mediator, call the factory method as shown below. In this example, the consumer will handle the `SubmitOrder` command.

```cs
public interface SubmitOrder
{
    string OrderNumber { get; }
}

class SubmitOrderConsumer :
    IConsumer<SubmitOrder>
{
    public async Task Consume(ConsumeContext<SubmitOrder> context)
    {
        // ... do the work ...
    }
}

var mediator = Bus.Factory.CreateMediator(cfg =>
{
    cfg.Consumer<SubmitOrderConsumer>();
});
```

Once created, the mediator can be used to execute the command.

```cs
await mediator.Send<SubmitOrder>(new { OrderNumber = "90210" });
```

MassTransit dispatches the command to the consumer asynchronously. Once the _Consume_ method completes, the _Send_ method will complete. If the consumer throws an exception, it will be propagated back to the caller.


### Requests

To send a request using the mediator, a request client can be created from the `IMediator` interface (which implements the same `IClientFactory` used to create request clients for a bus).

```cs
public interface GetOrderStatus
{
    string OrderNumber { get; }
}

public interface OrderStatus
{
    string OrderNumber { get; }
    string Status { get; }
}

class OrderStatusConsumer :
    IConsumer<GetOrderStatus>
{
    public async Task Consume(ConsumeContext<GetOrderStatus> context)
    {
        await context.RespondAsync<OrderStatus>(new { context.Message.OrderNumber, Status = "Pending" })
    }
}

var mediator = Bus.Factory.CreateMediator(cfg =>
{
    cfg.Consumer<OrderStatusConsumer>();
});

var client = mediator.CreateRequestClient<GetOrderStatus>();
```

Once created, the client can be used to send requests.

```cs
var response = await client.GetResponse<OrderStatus>(new { OrderNumber = "90210" });

Console.WriteLine("Order Status: {0}", response.Message.Status);
```

Just like _Send_, the call is executed asynchronously. If an exception occurs, the exception will be propagated back to the caller. If the request times out, or the request is cancelled, the _GetResponse_ method will throw an exception as well (either a _RequestTimeoutException_ or an _OperationCancelledException_).

### Containers

Supported dependency injection containers can be used with the mediator, including consumer and saga registration. To configure the mediator and register all consumers and sagas in an assembly, the configuration shown below can be used.

```cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddMassTransit(x =>
    {
        r.AddConsumers(Assembly.GetExecutingAssembly());

        r.AddMediator();
    });
}
```

> The syntax may vary by container, see the [containers](/usage/containers/) section for container-specific examples.

Sagas can also be registered, using the `.AddStateMachineSaga` and `.AddSaga` methods. The saga repositories must also be configured. An example saga registration is shown below.

```cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddMassTransit(x =>
    {
        r.AddSagaStateMachine<OrderStateMachine, OrderState>()
            .InMemoryRepository();

        r.AddMediator();
    });
}
```





