---

title: IMessageTopology

---

# IMessageTopology

Namespace: MassTransit

```csharp
public interface IMessageTopology : IMessageTopologyConfigurationObserverConnector
```

Implements [IMessageTopologyConfigurationObserverConnector](../masstransit-configuration/imessagetopologyconfigurationobserverconnector)

## Properties

### **EntityNameFormatter**

The entity name formatter used to format message names

```csharp
public abstract IEntityNameFormatter EntityNameFormatter { get; }
```

#### Property Value

[IEntityNameFormatter](../masstransit/ientitynameformatter)<br/>

## Methods

### **GetMessageTopology\<T\>()**

Returns the message topology for the specified message type

```csharp
IMessageTopology<T> GetMessageTopology<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IMessageTopology\<T\>](../masstransit/imessagetopology-1)<br/>
