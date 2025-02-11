---

title: IPublishTopologyConfigurator

---

# IPublishTopologyConfigurator

Namespace: MassTransit

```csharp
public interface IPublishTopologyConfigurator : IPublishTopology, IPublishTopologyConfigurationObserverConnector, ISpecification
```

Implements [IPublishTopology](../masstransit/ipublishtopology), [IPublishTopologyConfigurationObserverConnector](../masstransit-configuration/ipublishtopologyconfigurationobserverconnector), [ISpecification](../masstransit/ispecification)

## Methods

### **GetMessageTopology\<T\>()**

Returns the specification for the message type

```csharp
IMessagePublishTopologyConfigurator<T> GetMessageTopology<T>()
```

#### Type Parameters

`T`<br/>
The message type

#### Returns

[IMessagePublishTopologyConfigurator\<T\>](../masstransit/imessagepublishtopologyconfigurator-1)<br/>

### **GetMessageTopology(Type)**

Returns the specification for the message type

```csharp
IMessagePublishTopologyConfigurator GetMessageTopology(Type messageType)
```

#### Parameters

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IMessagePublishTopologyConfigurator](../masstransit/imessagepublishtopologyconfigurator)<br/>

### **TryAddConvention(IPublishTopologyConvention)**

Adds a convention to the topology, which will be applied to every message type
 requested, to determine if a convention for the message type is available.

```csharp
bool TryAddConvention(IPublishTopologyConvention convention)
```

#### Parameters

`convention` [IPublishTopologyConvention](../masstransit-configuration/ipublishtopologyconvention)<br/>
The Publish topology convention

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **AddMessagePublishTopology\<T\>(IMessagePublishTopology\<T\>)**

Add a Publish topology for a specific message type

```csharp
void AddMessagePublishTopology<T>(IMessagePublishTopology<T> topology)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`topology` [IMessagePublishTopology\<T\>](../masstransit/imessagepublishtopology-1)<br/>
The topology
