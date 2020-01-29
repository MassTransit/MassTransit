# Azure Service Bus

Azure Service Bus provides a feature called *message sessions*, to process multiple messages at once and to store some state on a temporary basis, which can be retrieved by some key.

The latter give us an ability to use this feature as saga state storage. Using message sessions as saga persistence, you can only use Azure Service Bus for both messaging and saga persistence purposes, without needing any additional infrastructure. You have to explicitly enable message sessions when configuring the endpoint, and use parameterless constructor to instantiate the saga repository.

When using message sessions, concurrency is managed by Azure Service Bus.

::: tip
Message sessions can only be correlated using the CorrelationId, which is copied to the message SessionId. Correlation expressions are not supported when using message sessions.
:::

Here is the basic sample of how to use the Azure Service Bus message session as saga repository:

```cs
public class OrderState :
    SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }

    public DateTime? OrderDate { get; set; }
}
```

### Container Integration

To configure a message session as the saga repository for a saga, use the code shown below using the _AddMassTransit_ container extension.

```cs {4}
container.AddMassTransit(cfg =>
{
    cfg.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .MessageSessionRepository();
});
```

Then, configure the endpoint to require a message session.

```cs
sbc.ReceiveEndpoint("order-state", ep =>
{
    ep.RequiresSession = true;
    ep.ConfigureSaga<OrderState>(provider);
});
```

To configure the receive endpoint without a container, the state machine and instance type can be specified explicitly.

```cs
var sagaStateMachine = new OrderStateMachine();
var repository = new MessageSessionSagaRepository<OrderState>(); 

cfg.ReceiveEndpoint("order-state", ep =>
{
    ep.RequiresSession = true;
    ep.StateMachineSaga(sagaStateMachine, repository);
});
```
