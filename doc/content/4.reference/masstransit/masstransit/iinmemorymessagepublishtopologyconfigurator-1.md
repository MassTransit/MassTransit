---

title: IInMemoryMessagePublishTopologyConfigurator<TMessage>

---

# IInMemoryMessagePublishTopologyConfigurator\<TMessage\>

Namespace: MassTransit

```csharp
public interface IInMemoryMessagePublishTopologyConfigurator<TMessage> : IMessagePublishTopologyConfigurator<TMessage>, IMessagePublishTopologyConfigurator, IMessagePublishTopology, ISpecification, IMessagePublishTopology<TMessage>, IInMemoryMessagePublishTopology<TMessage>, IInMemoryMessagePublishTopology, IInMemoryMessagePublishTopologyConfigurator
```

#### Type Parameters

`TMessage`<br/>

Implements [IMessagePublishTopologyConfigurator\<TMessage\>](../../masstransit-abstractions/masstransit/imessagepublishtopologyconfigurator-1), [IMessagePublishTopologyConfigurator](../../masstransit-abstractions/masstransit/imessagepublishtopologyconfigurator), [IMessagePublishTopology](../../masstransit-abstractions/masstransit/imessagepublishtopology), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IMessagePublishTopology\<TMessage\>](../../masstransit-abstractions/masstransit/imessagepublishtopology-1), [IInMemoryMessagePublishTopology\<TMessage\>](../masstransit/iinmemorymessagepublishtopology-1), [IInMemoryMessagePublishTopology](../masstransit/iinmemorymessagepublishtopology), [IInMemoryMessagePublishTopologyConfigurator](../masstransit/iinmemorymessagepublishtopologyconfigurator)

## Properties

### **ExchangeType**

```csharp
public abstract ExchangeType ExchangeType { set; }
```

#### Property Value

[ExchangeType](../masstransit-transports-fabric/exchangetype)<br/>
