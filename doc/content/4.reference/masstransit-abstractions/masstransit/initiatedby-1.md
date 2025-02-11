---

title: InitiatedBy<TMessage>

---

# InitiatedBy\<TMessage\>

Namespace: MassTransit

Specifies that the message type TMessage starts a new saga.

```csharp
public interface InitiatedBy<TMessage> : IConsumer<TMessage>, IConsumer
```

#### Type Parameters

`TMessage`<br/>

Implements [IConsumer\<TMessage\>](../masstransit/iconsumer-1), [IConsumer](../masstransit/iconsumer)
