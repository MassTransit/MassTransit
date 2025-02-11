---

title: InMemoryPublishTopology

---

# InMemoryPublishTopology

Namespace: MassTransit.InMemoryTransport

```csharp
public class InMemoryPublishTopology : PublishTopology, IPublishTopologyConfigurator, IPublishTopology, IPublishTopologyConfigurationObserverConnector, ISpecification, IPublishTopologyConfigurationObserver, IInMemoryPublishTopologyConfigurator, IInMemoryPublishTopology
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PublishTopology](../../masstransit-abstractions/masstransit-topology/publishtopology) → [InMemoryPublishTopology](../masstransit-inmemorytransport/inmemorypublishtopology)<br/>
Implements [IPublishTopologyConfigurator](../../masstransit-abstractions/masstransit/ipublishtopologyconfigurator), [IPublishTopology](../../masstransit-abstractions/masstransit/ipublishtopology), [IPublishTopologyConfigurationObserverConnector](../../masstransit-abstractions/masstransit-configuration/ipublishtopologyconfigurationobserverconnector), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IPublishTopologyConfigurationObserver](../../masstransit-abstractions/masstransit-configuration/ipublishtopologyconfigurationobserver), [IInMemoryPublishTopologyConfigurator](../masstransit/iinmemorypublishtopologyconfigurator), [IInMemoryPublishTopology](../masstransit/iinmemorypublishtopology)

## Constructors

### **InMemoryPublishTopology(IMessageTopology)**

```csharp
public InMemoryPublishTopology(IMessageTopology messageTopology)
```

#### Parameters

`messageTopology` [IMessageTopology](../../masstransit-abstractions/masstransit/imessagetopology)<br/>

## Methods

### **CreateMessageTopology\<T\>()**

```csharp
protected IMessagePublishTopologyConfigurator CreateMessageTopology<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IMessagePublishTopologyConfigurator](../../masstransit-abstractions/masstransit/imessagepublishtopologyconfigurator)<br/>
