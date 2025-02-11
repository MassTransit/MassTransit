---

title: IConsumerMessageFilter<TConsumer, TMessage>

---

# IConsumerMessageFilter\<TConsumer, TMessage\>

Namespace: MassTransit.Middleware

Adapts a consumer to consume the message type

```csharp
public interface IConsumerMessageFilter<TConsumer, TMessage> : IFilter<ConsumerConsumeContext<TConsumer, TMessage>>, IProbeSite
```

#### Type Parameters

`TConsumer`<br/>
The consumer type

`TMessage`<br/>
The message type

Implements [IFilter\<ConsumerConsumeContext\<TConsumer, TMessage\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)
