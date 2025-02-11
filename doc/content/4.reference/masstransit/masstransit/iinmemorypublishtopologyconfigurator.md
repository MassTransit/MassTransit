---

title: IInMemoryPublishTopologyConfigurator

---

# IInMemoryPublishTopologyConfigurator

Namespace: MassTransit

```csharp
public interface IInMemoryPublishTopologyConfigurator : IPublishTopologyConfigurator, IPublishTopology, IPublishTopologyConfigurationObserverConnector, ISpecification, IInMemoryPublishTopology
```

Implements [IPublishTopologyConfigurator](../../masstransit-abstractions/masstransit/ipublishtopologyconfigurator), [IPublishTopology](../../masstransit-abstractions/masstransit/ipublishtopology), [IPublishTopologyConfigurationObserverConnector](../../masstransit-abstractions/masstransit-configuration/ipublishtopologyconfigurationobserverconnector), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IInMemoryPublishTopology](../masstransit/iinmemorypublishtopology)

## Methods

### **GetMessageTopology\<T\>()**

```csharp
IInMemoryMessagePublishTopologyConfigurator<T> GetMessageTopology<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IInMemoryMessagePublishTopologyConfigurator\<T\>](../masstransit/iinmemorymessagepublishtopologyconfigurator-1)<br/>

### **GetMessageTopology(Type)**

```csharp
IInMemoryMessagePublishTopologyConfigurator GetMessageTopology(Type messageType)
```

#### Parameters

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IInMemoryMessagePublishTopologyConfigurator](../masstransit/iinmemorymessagepublishtopologyconfigurator)<br/>
