---

title: IInMemoryMessagePublishTopology<TMessage>

---

# IInMemoryMessagePublishTopology\<TMessage\>

Namespace: MassTransit

```csharp
public interface IInMemoryMessagePublishTopology<TMessage> : IMessagePublishTopology<TMessage>, IMessagePublishTopology, IInMemoryMessagePublishTopology
```

#### Type Parameters

`TMessage`<br/>

Implements [IMessagePublishTopology\<TMessage\>](../../masstransit-abstractions/masstransit/imessagepublishtopology-1), [IMessagePublishTopology](../../masstransit-abstractions/masstransit/imessagepublishtopology), [IInMemoryMessagePublishTopology](../masstransit/iinmemorymessagepublishtopology)

## Properties

### **ExchangeType**

```csharp
public abstract ExchangeType ExchangeType { get; }
```

#### Property Value

[ExchangeType](../masstransit-transports-fabric/exchangetype)<br/>
