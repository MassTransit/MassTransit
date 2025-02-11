---

title: ConsumeTopology

---

# ConsumeTopology

Namespace: MassTransit

```csharp
public class ConsumeTopology : IConsumeTopologyConfigurator, IConsumeTopology, IConsumeTopologyConfigurationObserverConnector, ISpecification, IConsumeTopologyConfigurationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumeTopology](../masstransit/consumetopology)<br/>
Implements [IConsumeTopologyConfigurator](../../masstransit-abstractions/masstransit/iconsumetopologyconfigurator), [IConsumeTopology](../../masstransit-abstractions/masstransit/iconsumetopology), [IConsumeTopologyConfigurationObserverConnector](../../masstransit-abstractions/masstransit-configuration/iconsumetopologyconfigurationobserverconnector), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IConsumeTopologyConfigurationObserver](../../masstransit-abstractions/masstransit-configuration/iconsumetopologyconfigurationobserver)

## Methods

### **GetMessageTopology(Type)**

```csharp
public IMessageConsumeTopologyConfigurator GetMessageTopology(Type messageType)
```

#### Parameters

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IMessageConsumeTopologyConfigurator](../../masstransit-abstractions/masstransit/imessageconsumetopologyconfigurator)<br/>

### **CreateTemporaryQueueName(String)**

```csharp
public string CreateTemporaryQueueName(string tag)
```

#### Parameters

`tag` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ConnectConsumeTopologyConfigurationObserver(IConsumeTopologyConfigurationObserver)**

```csharp
public ConnectHandle ConnectConsumeTopologyConfigurationObserver(IConsumeTopologyConfigurationObserver observer)
```

#### Parameters

`observer` [IConsumeTopologyConfigurationObserver](../../masstransit-abstractions/masstransit-configuration/iconsumetopologyconfigurationobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **TryAddConvention(IConsumeTopologyConvention)**

```csharp
public bool TryAddConvention(IConsumeTopologyConvention convention)
```

#### Parameters

`convention` [IConsumeTopologyConvention](../../masstransit-abstractions/masstransit-configuration/iconsumetopologyconvention)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **GetMessageTopology\<T\>()**

```csharp
protected IMessageConsumeTopologyConfigurator<T> GetMessageTopology<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IMessageConsumeTopologyConfigurator\<T\>](../../masstransit-abstractions/masstransit/imessageconsumetopologyconfigurator-1)<br/>

### **All(Func\<IMessageConsumeTopologyConfigurator, Boolean\>)**

```csharp
protected bool All(Func<IMessageConsumeTopologyConfigurator, bool> callback)
```

#### Parameters

`callback` [Func\<IMessageConsumeTopologyConfigurator, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **SelectMany\<T, TResult\>(Func\<T, IEnumerable\<TResult\>\>)**

```csharp
protected IEnumerable<TResult> SelectMany<T, TResult>(Func<T, IEnumerable<TResult>> selector)
```

#### Type Parameters

`T`<br/>

`TResult`<br/>

#### Parameters

`selector` [Func\<T, IEnumerable\<TResult\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IEnumerable\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **ForEach\<T\>(Action\<T\>)**

```csharp
protected void ForEach<T>(Action<T> callback)
```

#### Type Parameters

`T`<br/>

#### Parameters

`callback` [Action\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **CreateMessageTopology\<T\>()**

```csharp
protected IMessageConsumeTopologyConfigurator CreateMessageTopology<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IMessageConsumeTopologyConfigurator](../../masstransit-abstractions/masstransit/imessageconsumetopologyconfigurator)<br/>

### **OnMessageTopologyCreated\<T\>(IMessageConsumeTopologyConfigurator\<T\>)**

```csharp
protected void OnMessageTopologyCreated<T>(IMessageConsumeTopologyConfigurator<T> messageTopology)
```

#### Type Parameters

`T`<br/>

#### Parameters

`messageTopology` [IMessageConsumeTopologyConfigurator\<T\>](../../masstransit-abstractions/masstransit/imessageconsumetopologyconfigurator-1)<br/>
