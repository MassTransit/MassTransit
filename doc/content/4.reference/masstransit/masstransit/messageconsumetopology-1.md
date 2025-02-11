---

title: MessageConsumeTopology<TMessage>

---

# MessageConsumeTopology\<TMessage\>

Namespace: MassTransit

```csharp
public class MessageConsumeTopology<TMessage> : IMessageConsumeTopologyConfigurator<TMessage>, IMessageConsumeTopologyConfigurator, ISpecification, IMessageConsumeTopology<TMessage>
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageConsumeTopology\<TMessage\>](../masstransit/messageconsumetopology-1)<br/>
Implements [IMessageConsumeTopologyConfigurator\<TMessage\>](../../masstransit-abstractions/masstransit/imessageconsumetopologyconfigurator-1), [IMessageConsumeTopologyConfigurator](../../masstransit-abstractions/masstransit/imessageconsumetopologyconfigurator), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IMessageConsumeTopology\<TMessage\>](../../masstransit-abstractions/masstransit/imessageconsumetopology-1)

## Properties

### **ConfigureConsumeTopology**

```csharp
public bool ConfigureConsumeTopology { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **MessageConsumeTopology()**

```csharp
public MessageConsumeTopology()
```

## Methods

### **Add(IMessageConsumeTopology\<TMessage\>)**

```csharp
public void Add(IMessageConsumeTopology<TMessage> consumeTopology)
```

#### Parameters

`consumeTopology` [IMessageConsumeTopology\<TMessage\>](../../masstransit-abstractions/masstransit/imessageconsumetopology-1)<br/>

### **AddDelegate(IMessageConsumeTopology\<TMessage\>)**

```csharp
public void AddDelegate(IMessageConsumeTopology<TMessage> configuration)
```

#### Parameters

`configuration` [IMessageConsumeTopology\<TMessage\>](../../masstransit-abstractions/masstransit/imessageconsumetopology-1)<br/>

### **Apply(ITopologyPipeBuilder\<ConsumeContext\<TMessage\>\>)**

```csharp
public void Apply(ITopologyPipeBuilder<ConsumeContext<TMessage>> builder)
```

#### Parameters

`builder` [ITopologyPipeBuilder\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit-configuration/itopologypipebuilder-1)<br/>

### **TryAddConvention(IMessageConsumeTopologyConvention\<TMessage\>)**

```csharp
public bool TryAddConvention(IMessageConsumeTopologyConvention<TMessage> convention)
```

#### Parameters

`convention` [IMessageConsumeTopologyConvention\<TMessage\>](../../masstransit-abstractions/masstransit-configuration/imessageconsumetopologyconvention-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **UpdateConvention\<TConvention\>(Func\<TConvention, TConvention\>)**

```csharp
public void UpdateConvention<TConvention>(Func<TConvention, TConvention> update)
```

#### Type Parameters

`TConvention`<br/>

#### Parameters

`update` [Func\<TConvention, TConvention\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **AddOrUpdateConvention\<TConvention\>(Func\<TConvention\>, Func\<TConvention, TConvention\>)**

```csharp
public void AddOrUpdateConvention<TConvention>(Func<TConvention> add, Func<TConvention, TConvention> update)
```

#### Type Parameters

`TConvention`<br/>

#### Parameters

`add` [Func\<TConvention\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

`update` [Func\<TConvention, TConvention\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

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
