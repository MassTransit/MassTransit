---

title: SagaConsumeContext<TSaga, TMessage>

---

# SagaConsumeContext\<TSaga, TMessage\>

Namespace: MassTransit

Consume context including the saga instance consuming the message

```csharp
public interface SagaConsumeContext<TSaga, TMessage> : SagaConsumeContext<TSaga>, ConsumeContext, PipeContext, MessageContext, IPublishEndpoint, IPublishObserverConnector, ISendEndpointProvider, ISendObserverConnector, ConsumeContext<TMessage>
```

#### Type Parameters

`TSaga`<br/>
The saga type

`TMessage`<br/>
The message type

Implements [SagaConsumeContext\<TSaga\>](../masstransit/sagaconsumecontext-1), [ConsumeContext](../masstransit/consumecontext), [PipeContext](../masstransit/pipecontext), [MessageContext](../masstransit/messagecontext), [IPublishEndpoint](../masstransit/ipublishendpoint), [IPublishObserverConnector](../masstransit/ipublishobserverconnector), [ISendEndpointProvider](../masstransit/isendendpointprovider), [ISendObserverConnector](../masstransit/isendobserverconnector), [ConsumeContext\<TMessage\>](../masstransit/consumecontext-1)
