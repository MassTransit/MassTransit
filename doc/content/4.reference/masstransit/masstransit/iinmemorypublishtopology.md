---

title: IInMemoryPublishTopology

---

# IInMemoryPublishTopology

Namespace: MassTransit

```csharp
public interface IInMemoryPublishTopology : IPublishTopology, IPublishTopologyConfigurationObserverConnector
```

Implements [IPublishTopology](../../masstransit-abstractions/masstransit/ipublishtopology), [IPublishTopologyConfigurationObserverConnector](../../masstransit-abstractions/masstransit-configuration/ipublishtopologyconfigurationobserverconnector)

## Methods

### **GetMessageTopology\<T\>()**

```csharp
IInMemoryMessagePublishTopology<T> GetMessageTopology<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IInMemoryMessagePublishTopology\<T\>](../masstransit/iinmemorymessagepublishtopology-1)<br/>
