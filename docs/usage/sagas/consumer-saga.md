# Consumer Sagas

Consumer sagas use a class, similar to a consumer, and declare interfaces for the correlated event types.

Consumer sagas combined data and behavior in a single class. In the above example, a new saga instance is created by the _SubmitOrder_ message.

```cs {16}
public interface SubmitOrder :
    CorrelatedBy<Guid>
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
    CorrelatedBy<Guid>
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

In some cases, a single message may either initiate a new saga instance or orchestrate an existing instance. Introduced in version 7.0.7, the `InitiatedByOrOrchestrates<T>` interface supports this in a consumer saga.

```cs
public interface OrderInvoiced :
    CorrelatedBy<Guid>
{
    DateTime Timestamp { get; }
    decimal Amount { get; }
}

public class OrderPaymentSaga :
    ISaga,
    InitiatedByOrOrchestrates<OrderInvoiced>
{
    public Guid CorrelationId { get; set; }

    public DateTime? InvoiceDate { get; set; }
    public decimal? Amount { get; set; }

    public async Task Consume(ConsumeContext<OrderInvoiced> context)
    {
        InvoiceDate = context.Message.Timestamp;
        Amount = context.Message.Amount;
    }
}
```

If you're using a container, saga registration is fully supported. The example below configures the saga using an in-memory repository with an in-memory transport.

```cs
services.AddMassTransit(x =>
{
    x.AddSaga<OrderSaga>()
        .InMemoryRepository();

    x.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});
```

If using the legacy configuration syntax, the saga can be configured on a receive endpoint using the `.Saga` method.

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


The configuration for the various supported saga persistence storage engines is detailed in the [persistence](persistence.md) documentation.
