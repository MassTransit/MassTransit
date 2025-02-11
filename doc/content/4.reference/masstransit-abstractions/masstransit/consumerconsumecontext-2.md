---

title: ConsumerConsumeContext<TConsumer, TMessage>

---

# ConsumerConsumeContext\<TConsumer, TMessage\>

Namespace: MassTransit

A consumer and consume context mixed together, carrying both a consumer and the message
 consume context.

```csharp
public interface ConsumerConsumeContext<TConsumer, TMessage> : ConsumerConsumeContext<TConsumer>, ConsumeContext, PipeContext, MessageContext, IPublishEndpoint, IPublishObserverConnector, ISendEndpointProvider, ISendObserverConnector, ConsumeContext<TMessage>
```

#### Type Parameters

`TConsumer`<br/>
The consumer type

`TMessage`<br/>
The message type

Implements [ConsumerConsumeContext\<TConsumer\>](../masstransit/consumerconsumecontext-1), [ConsumeContext](../masstransit/consumecontext), [PipeContext](../masstransit/pipecontext), [MessageContext](../masstransit/messagecontext), [IPublishEndpoint](../masstransit/ipublishendpoint), [IPublishObserverConnector](../masstransit/ipublishobserverconnector), [ISendEndpointProvider](../masstransit/isendendpointprovider), [ISendObserverConnector](../masstransit/isendobserverconnector), [ConsumeContext\<TMessage\>](../masstransit/consumecontext-1)
