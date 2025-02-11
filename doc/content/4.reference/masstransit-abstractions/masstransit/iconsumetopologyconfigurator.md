---

title: IConsumeTopologyConfigurator

---

# IConsumeTopologyConfigurator

Namespace: MassTransit

```csharp
public interface IConsumeTopologyConfigurator : IConsumeTopology, IConsumeTopologyConfigurationObserverConnector, ISpecification
```

Implements [IConsumeTopology](../masstransit/iconsumetopology), [IConsumeTopologyConfigurationObserverConnector](../masstransit-configuration/iconsumetopologyconfigurationobserverconnector), [ISpecification](../masstransit/ispecification)

## Methods

### **GetMessageTopology\<T\>()**

Returns the specification for the message type

```csharp
IMessageConsumeTopologyConfigurator<T> GetMessageTopology<T>()
```

#### Type Parameters

`T`<br/>
The message type

#### Returns

[IMessageConsumeTopologyConfigurator\<T\>](../masstransit/imessageconsumetopologyconfigurator-1)<br/>

### **GetMessageTopology(Type)**

Returns the specification for the message type

```csharp
IMessageConsumeTopologyConfigurator GetMessageTopology(Type messageType)
```

#### Parameters

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IMessageConsumeTopologyConfigurator](../masstransit/imessageconsumetopologyconfigurator)<br/>

### **TryAddConvention(IConsumeTopologyConvention)**

Adds a convention to the topology, which will be applied to every message type
 requested, to determine if a convention for the message type is available.

```csharp
bool TryAddConvention(IConsumeTopologyConvention convention)
```

#### Parameters

`convention` [IConsumeTopologyConvention](../masstransit-configuration/iconsumetopologyconvention)<br/>
The Consume topology convention

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
