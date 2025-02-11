---

title: ISagaMessageFilter<TSaga, TMessage>

---

# ISagaMessageFilter\<TSaga, TMessage\>

Namespace: MassTransit.Middleware

Adapts a consumer to consume the message type

```csharp
public interface ISagaMessageFilter<TSaga, TMessage> : IFilter<SagaConsumeContext<TSaga, TMessage>>, IProbeSite
```

#### Type Parameters

`TSaga`<br/>
The consumer type

`TMessage`<br/>
The message type

Implements [IFilter\<SagaConsumeContext\<TSaga, TMessage\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)
