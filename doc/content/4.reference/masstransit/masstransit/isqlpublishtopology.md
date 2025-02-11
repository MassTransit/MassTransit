---

title: ISqlPublishTopology

---

# ISqlPublishTopology

Namespace: MassTransit

```csharp
public interface ISqlPublishTopology : IPublishTopology, IPublishTopologyConfigurationObserverConnector
```

Implements [IPublishTopology](../../masstransit-abstractions/masstransit/ipublishtopology), [IPublishTopologyConfigurationObserverConnector](../../masstransit-abstractions/masstransit-configuration/ipublishtopologyconfigurationobserverconnector)

## Methods

### **GetMessageTopology\<T\>()**

```csharp
ISqlMessagePublishTopology<T> GetMessageTopology<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[ISqlMessagePublishTopology\<T\>](../masstransit/isqlmessagepublishtopology-1)<br/>

### **GetPublishBrokerTopology()**

```csharp
BrokerTopology GetPublishBrokerTopology()
```

#### Returns

[BrokerTopology](../masstransit-sqltransport-topology/brokertopology)<br/>
