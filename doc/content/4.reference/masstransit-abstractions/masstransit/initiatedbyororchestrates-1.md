---

title: InitiatedByOrOrchestrates<TMessage>

---

# InitiatedByOrOrchestrates\<TMessage\>

Namespace: MassTransit

Specifies that a class implementing ISaga consumes TMessage as part of the saga

```csharp
public interface InitiatedByOrOrchestrates<TMessage> : IConsumer<TMessage>, IConsumer
```

#### Type Parameters

`TMessage`<br/>
The type of message to consume

Implements [IConsumer\<TMessage\>](../masstransit/iconsumer-1), [IConsumer](../masstransit/iconsumer)
