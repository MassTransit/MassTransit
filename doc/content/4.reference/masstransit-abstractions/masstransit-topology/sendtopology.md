---

title: SendTopology

---

# SendTopology

Namespace: MassTransit.Topology

```csharp
public class SendTopology : ISendTopologyConfigurator, ISendTopology, ISendTopologyConfigurationObserverConnector, ISpecification, ISendTopologyConfigurationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SendTopology](../masstransit-topology/sendtopology)<br/>
Implements [ISendTopologyConfigurator](../masstransit/isendtopologyconfigurator), [ISendTopology](../masstransit/isendtopology), [ISendTopologyConfigurationObserverConnector](../masstransit-configuration/isendtopologyconfigurationobserverconnector), [ISpecification](../masstransit/ispecification), [ISendTopologyConfigurationObserver](../masstransit-configuration/isendtopologyconfigurationobserver)

## Properties

### **DeadLetterQueueNameFormatter**

```csharp
public IDeadLetterQueueNameFormatter DeadLetterQueueNameFormatter { get; set; }
```

#### Property Value

[IDeadLetterQueueNameFormatter](../masstransit/ideadletterqueuenameformatter)<br/>

### **ErrorQueueNameFormatter**

```csharp
public IErrorQueueNameFormatter ErrorQueueNameFormatter { get; set; }
```

#### Property Value

[IErrorQueueNameFormatter](../masstransit/ierrorqueuenameformatter)<br/>

## Constructors

### **SendTopology()**

```csharp
public SendTopology()
```

## Methods

### **GetMessageTopology\<T\>()**

```csharp
public IMessageSendTopologyConfigurator<T> GetMessageTopology<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IMessageSendTopologyConfigurator\<T\>](../masstransit/imessagesendtopologyconfigurator-1)<br/>

### **ConnectSendTopologyConfigurationObserver(ISendTopologyConfigurationObserver)**

```csharp
public ConnectHandle ConnectSendTopologyConfigurationObserver(ISendTopologyConfigurationObserver observer)
```

#### Parameters

`observer` [ISendTopologyConfigurationObserver](../masstransit-configuration/isendtopologyconfigurationobserver)<br/>

#### Returns

[ConnectHandle](../masstransit/connecthandle)<br/>

### **TryAddConvention(ISendTopologyConvention)**

```csharp
public bool TryAddConvention(ISendTopologyConvention convention)
```

#### Parameters

`convention` [ISendTopologyConvention](../masstransit-configuration/isendtopologyconvention)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **CreateMessageTopology\<T\>(Type)**

```csharp
protected IMessageSendTopologyConfigurator CreateMessageTopology<T>(Type type)
```

#### Type Parameters

`T`<br/>

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IMessageSendTopologyConfigurator](../masstransit/imessagesendtopologyconfigurator)<br/>

### **OnMessageTopologyCreated\<T\>(IMessageSendTopologyConfigurator\<T\>)**

```csharp
protected void OnMessageTopologyCreated<T>(IMessageSendTopologyConfigurator<T> messageTopology)
```

#### Type Parameters

`T`<br/>

#### Parameters

`messageTopology` [IMessageSendTopologyConfigurator\<T\>](../masstransit/imessagesendtopologyconfigurator-1)<br/>
