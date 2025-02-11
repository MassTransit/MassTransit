---

title: ISendTopologyConfigurator

---

# ISendTopologyConfigurator

Namespace: MassTransit

```csharp
public interface ISendTopologyConfigurator : ISendTopology, ISendTopologyConfigurationObserverConnector, ISpecification
```

Implements [ISendTopology](../masstransit/isendtopology), [ISendTopologyConfigurationObserverConnector](../masstransit-configuration/isendtopologyconfigurationobserverconnector), [ISpecification](../masstransit/ispecification)

## Properties

### **DeadLetterQueueNameFormatter**

Specify a dead letter queue name formatter, which is used to format the name for a dead letter queue.
 Defaults to (queue name)_skipped.

```csharp
public abstract IDeadLetterQueueNameFormatter DeadLetterQueueNameFormatter { get; set; }
```

#### Property Value

[IDeadLetterQueueNameFormatter](../masstransit/ideadletterqueuenameformatter)<br/>

### **ErrorQueueNameFormatter**

Specify an error queue name formatter, which is used to format the name for an error queue.
 Defaults to (queue name)_error.

```csharp
public abstract IErrorQueueNameFormatter ErrorQueueNameFormatter { get; set; }
```

#### Property Value

[IErrorQueueNameFormatter](../masstransit/ierrorqueuenameformatter)<br/>

## Methods

### **TryAddConvention(ISendTopologyConvention)**

Adds a convention to the topology, which will be applied to every message type
 requested, to determine if a convention for the message type is available.

```csharp
bool TryAddConvention(ISendTopologyConvention convention)
```

#### Parameters

`convention` [ISendTopologyConvention](../masstransit-configuration/isendtopologyconvention)<br/>
The send topology convention

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **AddMessageSendTopology\<T\>(IMessageSendTopology\<T\>)**

Add a send topology for a specific message type

```csharp
void AddMessageSendTopology<T>(IMessageSendTopology<T> topology)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`topology` [IMessageSendTopology\<T\>](../masstransit/imessagesendtopology-1)<br/>
The topology
