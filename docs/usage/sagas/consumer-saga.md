# Consumer Sagas

Consumer sagas use a class, similar to a consumer, and declare interfaces for the correlated event types.

Consumer sagas combined data and behavior in a single class. In the above example, a new saga instance is created by the _SubmitOrder_ message.

```cs {16}
public interface SubmitOrder :
    ICorrelatedBy<Guid>
{
    DateTime OrderDate { get; }
}

public class OrderSaga :
    ISaga,
    InitiatedBy<SubmitOrder>
{
    public Guid CorrelationId { get; set; }

    public DateTime? SubmitDate { get; set; }
    public DateTime? AcceptDate { get; set; }

    public async Task Consume(ConsumeContext<SubmitOrder> context)
    {
        SubmitDate = context.Message.OrderDate;
    }
}
```

To add the _OrderAccepted_ message to the saga, an additional interface and method is specified.

```cs {19}
public interface OrderAccepted :
    ICorrelatedBy<Guid>
{
    DateTime Timestamp { get; }
}

public class OrderSaga :
    ISaga,
    InitiatedBy<SubmitOrder>,
    Orchestrates<OrderAccepted>,
{
    public Guid CorrelationId { get; set; }

    public DateTime? SubmitDate { get; set; }
    public DateTime? AcceptDate { get; set; }

    public async Task Consume(ConsumeContext<SubmitOrder> context) {...}

    public async Task Consume(ConsumeContext<OrderAccepted> context)
    {
        AcceptDate = context.Message.Timestamp;
    }
}
```

To add the _OrderShipped_ message to the saga, which is correlated by a separate property, an additional interface and method is specified.

```cs {22,27-28}
public interface OrderShipped
{
    Guid OrderId { get; }
    DateTime ShipDate { get; }
}

public class OrderSaga :
    ISaga,
    InitiatedBy<SubmitOrder>,
    Orchestrates<OrderAccepted>,
    Observes<OrderShipped>
{
    public Guid CorrelationId { get; set; }

    public DateTime? SubmitDate { get; set; }
    public DateTime? AcceptDate { get; set; }
    public DateTime? ShipDate { get; set; }

    public async Task Consume(ConsumeContext<SubmitOrder> context) {...}
    public async Task Consume(ConsumeContext<OrderAccepted> context) {...}

    public async Task Consume(ConsumeContext<OrderShipped> context)
    {
        ShipDate = context.Message.ShipDate;
    }

    public Expression<Func<OrderSaga, OrderShipped, bool>> CorrelationExpression =>
        (saga,message) => saga.CorrelationId == message.OrderId;
}
```

The saga is configured on a receive endpoint using the `.Saga` method.

```cs
var repository = new InMemorySagaRepository<OrderSaga>();

var busControl = Bus.Factory.CreateUsingInMemory(cfg =>
{
    cfg.ReceiveEndpoint("order-saga", e =>
    {
        e.Saga<OrderSaga>(repository);
    });
});
```

### Container Registration

If you're using a container, saga registration is fully supported. The example below configures the saga using an in-memory repository with an in-memory transport.

```cs
services.AddMassTransit(cfg =>
{
    cfg.AddSaga<OrderSaga>()
        .InMemoryRepository();

    cfg.AddBus(context => Bus.Factory.CreateUsingInMemory(cfg =>
    {
        cfg.ConfigureEndpoints(context);
    }));
});
```

The configuration for the various supported saga persistence storage engines is detailed in the [persistence](persistence.md) documentation.
