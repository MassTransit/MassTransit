---

title: OutboxConsumeContext<TMessage>

---

# OutboxConsumeContext\<TMessage\>

Namespace: MassTransit.Middleware

```csharp
public interface OutboxConsumeContext<TMessage> : OutboxConsumeContext, ConsumeContext, PipeContext, MessageContext, IPublishEndpoint, IPublishObserverConnector, ISendEndpointProvider, ISendObserverConnector, OutboxSendContext, IServiceProvider, ConsumeContext<TMessage>
```

#### Type Parameters

`TMessage`<br/>

Implements [OutboxConsumeContext](../masstransit-middleware/outboxconsumecontext), [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext), [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [MessageContext](../../masstransit-abstractions/masstransit/messagecontext), [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [OutboxSendContext](../masstransit-middleware/outboxsendcontext), IServiceProvider, [ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1)
