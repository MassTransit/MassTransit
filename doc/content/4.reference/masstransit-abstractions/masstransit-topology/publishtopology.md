---

title: PublishTopology

---

# PublishTopology

Namespace: MassTransit.Topology

```csharp
public class PublishTopology : IPublishTopologyConfigurator, IPublishTopology, IPublishTopologyConfigurationObserverConnector, ISpecification, IPublishTopologyConfigurationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PublishTopology](../masstransit-topology/publishtopology)<br/>
Implements [IPublishTopologyConfigurator](../masstransit/ipublishtopologyconfigurator), [IPublishTopology](../masstransit/ipublishtopology), [IPublishTopologyConfigurationObserverConnector](../masstransit-configuration/ipublishtopologyconfigurationobserverconnector), [ISpecification](../masstransit/ispecification), [IPublishTopologyConfigurationObserver](../masstransit-configuration/ipublishtopologyconfigurationobserver)

## Constructors

### **PublishTopology()**

```csharp
public PublishTopology()
```

## Methods

### **TryGetPublishAddress(Type, Uri, Uri)**

```csharp
public bool TryGetPublishAddress(Type messageType, Uri baseAddress, out Uri publishAddress)
```

#### Parameters

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`baseAddress` Uri<br/>

`publishAddress` Uri<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **ConnectPublishTopologyConfigurationObserver(IPublishTopologyConfigurationObserver)**

```csharp
public ConnectHandle ConnectPublishTopologyConfigurationObserver(IPublishTopologyConfigurationObserver observer)
```

#### Parameters

`observer` [IPublishTopologyConfigurationObserver](../masstransit-configuration/ipublishtopologyconfigurationobserver)<br/>

#### Returns

[ConnectHandle](../masstransit/connecthandle)<br/>

### **TryAddConvention(IPublishTopologyConvention)**

```csharp
public bool TryAddConvention(IPublishTopologyConvention convention)
```

#### Parameters

`convention` [IPublishTopologyConvention](../masstransit-configuration/ipublishtopologyconvention)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **GetMessageTopology(Type)**

```csharp
public IMessagePublishTopologyConfigurator GetMessageTopology(Type messageType)
```

#### Parameters

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IMessagePublishTopologyConfigurator](../masstransit/imessagepublishtopologyconfigurator)<br/>

### **CreateMessageTopology\<T\>()**

```csharp
protected IMessagePublishTopologyConfigurator CreateMessageTopology<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IMessagePublishTopologyConfigurator](../masstransit/imessagepublishtopologyconfigurator)<br/>

### **GetMessageTopology\<T\>()**

```csharp
protected IMessagePublishTopologyConfigurator<T> GetMessageTopology<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IMessagePublishTopologyConfigurator\<T\>](../masstransit/imessagepublishtopologyconfigurator-1)<br/>

### **OnMessageTopologyCreated\<T\>(IMessagePublishTopologyConfigurator\<T\>)**

```csharp
protected void OnMessageTopologyCreated<T>(IMessagePublishTopologyConfigurator<T> messageTopology)
```

#### Type Parameters

`T`<br/>

#### Parameters

`messageTopology` [IMessagePublishTopologyConfigurator\<T\>](../masstransit/imessagepublishtopologyconfigurator-1)<br/>

### **ForEachMessageType\<T\>(Action\<T\>)**

```csharp
protected void ForEachMessageType<T>(Action<T> callback)
```

#### Type Parameters

`T`<br/>

#### Parameters

`callback` [Action\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
