---

title: ISqlPublishTopologyConfigurator

---

# ISqlPublishTopologyConfigurator

Namespace: MassTransit

```csharp
public interface ISqlPublishTopologyConfigurator : IPublishTopologyConfigurator, IPublishTopology, IPublishTopologyConfigurationObserverConnector, ISpecification, ISqlPublishTopology
```

Implements [IPublishTopologyConfigurator](../../masstransit-abstractions/masstransit/ipublishtopologyconfigurator), [IPublishTopology](../../masstransit-abstractions/masstransit/ipublishtopology), [IPublishTopologyConfigurationObserverConnector](../../masstransit-abstractions/masstransit-configuration/ipublishtopologyconfigurationobserverconnector), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [ISqlPublishTopology](../masstransit/isqlpublishtopology)

## Methods

### **GetMessageTopology\<T\>()**

```csharp
ISqlMessagePublishTopologyConfigurator<T> GetMessageTopology<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[ISqlMessagePublishTopologyConfigurator\<T\>](../masstransit/isqlmessagepublishtopologyconfigurator-1)<br/>

### **GetMessageTopology(Type)**

```csharp
ISqlMessagePublishTopologyConfigurator GetMessageTopology(Type messageType)
```

#### Parameters

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[ISqlMessagePublishTopologyConfigurator](../masstransit/isqlmessagepublishtopologyconfigurator)<br/>
