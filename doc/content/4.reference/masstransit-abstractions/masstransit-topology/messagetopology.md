---

title: MessageTopology

---

# MessageTopology

Namespace: MassTransit.Topology

```csharp
public class MessageTopology : IMessageTopologyConfigurator, IMessageTopology, IMessageTopologyConfigurationObserverConnector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageTopology](../masstransit-topology/messagetopology)<br/>
Implements [IMessageTopologyConfigurator](../masstransit-configuration/imessagetopologyconfigurator), [IMessageTopology](../masstransit/imessagetopology), [IMessageTopologyConfigurationObserverConnector](../masstransit-configuration/imessagetopologyconfigurationobserverconnector)

## Properties

### **EntityNameFormatter**

```csharp
public IEntityNameFormatter EntityNameFormatter { get; private set; }
```

#### Property Value

[IEntityNameFormatter](../masstransit/ientitynameformatter)<br/>

## Constructors

### **MessageTopology(IEntityNameFormatter)**

```csharp
public MessageTopology(IEntityNameFormatter entityNameFormatter)
```

#### Parameters

`entityNameFormatter` [IEntityNameFormatter](../masstransit/ientitynameformatter)<br/>

## Methods

### **SetEntityNameFormatter(IEntityNameFormatter)**

```csharp
public void SetEntityNameFormatter(IEntityNameFormatter entityNameFormatter)
```

#### Parameters

`entityNameFormatter` [IEntityNameFormatter](../masstransit/ientitynameformatter)<br/>

### **ConnectMessageTopologyConfigurationObserver(IMessageTopologyConfigurationObserver)**

```csharp
public ConnectHandle ConnectMessageTopologyConfigurationObserver(IMessageTopologyConfigurationObserver observer)
```

#### Parameters

`observer` [IMessageTopologyConfigurationObserver](../masstransit-configuration/imessagetopologyconfigurationobserver)<br/>

#### Returns

[ConnectHandle](../masstransit/connecthandle)<br/>

### **CreateMessageTopology\<T\>(Type)**

```csharp
protected IMessageTypeTopologyConfigurator CreateMessageTopology<T>(Type type)
```

#### Type Parameters

`T`<br/>

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IMessageTypeTopologyConfigurator](../masstransit-configuration/imessagetypetopologyconfigurator)<br/>
