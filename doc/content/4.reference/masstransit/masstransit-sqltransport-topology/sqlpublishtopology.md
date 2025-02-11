---

title: SqlPublishTopology

---

# SqlPublishTopology

Namespace: MassTransit.SqlTransport.Topology

```csharp
public class SqlPublishTopology : PublishTopology, IPublishTopologyConfigurator, IPublishTopology, IPublishTopologyConfigurationObserverConnector, ISpecification, IPublishTopologyConfigurationObserver, ISqlPublishTopologyConfigurator, ISqlPublishTopology
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PublishTopology](../../masstransit-abstractions/masstransit-topology/publishtopology) → [SqlPublishTopology](../masstransit-sqltransport-topology/sqlpublishtopology)<br/>
Implements [IPublishTopologyConfigurator](../../masstransit-abstractions/masstransit/ipublishtopologyconfigurator), [IPublishTopology](../../masstransit-abstractions/masstransit/ipublishtopology), [IPublishTopologyConfigurationObserverConnector](../../masstransit-abstractions/masstransit-configuration/ipublishtopologyconfigurationobserverconnector), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IPublishTopologyConfigurationObserver](../../masstransit-abstractions/masstransit-configuration/ipublishtopologyconfigurationobserver), [ISqlPublishTopologyConfigurator](../masstransit/isqlpublishtopologyconfigurator), [ISqlPublishTopology](../masstransit/isqlpublishtopology)

## Constructors

### **SqlPublishTopology(IMessageTopology)**

```csharp
public SqlPublishTopology(IMessageTopology messageTopology)
```

#### Parameters

`messageTopology` [IMessageTopology](../../masstransit-abstractions/masstransit/imessagetopology)<br/>

## Methods

### **GetPublishBrokerTopology()**

```csharp
public BrokerTopology GetPublishBrokerTopology()
```

#### Returns

[BrokerTopology](../masstransit-sqltransport-topology/brokertopology)<br/>

### **CreateMessageTopology\<T\>()**

```csharp
protected IMessagePublishTopologyConfigurator CreateMessageTopology<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IMessagePublishTopologyConfigurator](../../masstransit-abstractions/masstransit/imessagepublishtopologyconfigurator)<br/>
