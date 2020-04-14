# Definitions

Definitions are used to specify the behavior of consumers, sagas, and activities so that they can be automatically configured. Definitions are used by MassTransit's common container registration, and definitions can be explicitely registered or discovered automatically using the `AddConsumers...`, `AddSagaStateMachines...`, etc. methods.

### Consumer

```cs
public class SubmitOrderConsumerDefinition :
    ConsumerDefinition<SubmitOrderConsumer>
{
    public SubmitOrderConsumerDefinition()
    {
        // override the default endpoint name, for whatever reason
        EndpointName = "ha-submit-order";

        // limit the number of messages consumed concurrently
        // this applies to the consumer only, not the endpoint
        ConcurrentMessageLimit = 4;
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<DiscoveryPingConsumer> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Interval(5, 1000));
        endpointConfigurator.UseInMemoryOutbox();
    }
}
```

### Saga

```cs
public class OrderStateDefinition :
    SagaDefinition<OrderState>
{
    public OrderStateDefinition()
    {
        // specify the message limit at the endpoint level, which influences
        // the endpoint prefetch count, if supported
        Endpoint(e => e.ConcurrentMessageLimit = 16);
    }

    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderState> sagaConfigurator)
    {
        var partition = endpointConfigurator.CreatePartitioner(16);

        sagaConfigurator.Message<SubmitOrder>(x => x.UsePartitioner(partition, m => m.Message.CorrelationId));
        sagaConfigurator.Message<OrderAccepted>(x => x.UsePartitioner(partition, m => m.Message.CorrelationId));
        sagaConfigurator.Message<OrderCanceled>(x => x.UsePartitioner(partition, m => m.Message.CorrelationId));
    }
}
```

