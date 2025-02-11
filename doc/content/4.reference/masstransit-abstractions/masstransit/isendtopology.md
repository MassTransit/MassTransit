---

title: ISendTopology

---

# ISendTopology

Namespace: MassTransit

```csharp
public interface ISendTopology : ISendTopologyConfigurationObserverConnector
```

Implements [ISendTopologyConfigurationObserverConnector](../masstransit-configuration/isendtopologyconfigurationobserverconnector)

## Properties

### **DeadLetterQueueNameFormatter**

```csharp
public abstract IDeadLetterQueueNameFormatter DeadLetterQueueNameFormatter { get; }
```

#### Property Value

[IDeadLetterQueueNameFormatter](../masstransit/ideadletterqueuenameformatter)<br/>

### **ErrorQueueNameFormatter**

```csharp
public abstract IErrorQueueNameFormatter ErrorQueueNameFormatter { get; }
```

#### Property Value

[IErrorQueueNameFormatter](../masstransit/ierrorqueuenameformatter)<br/>

## Methods

### **GetMessageTopology\<T\>()**

Returns the specification for the message type

```csharp
IMessageSendTopologyConfigurator<T> GetMessageTopology<T>()
```

#### Type Parameters

`T`<br/>
The message type

#### Returns

[IMessageSendTopologyConfigurator\<T\>](../masstransit/imessagesendtopologyconfigurator-1)<br/>
