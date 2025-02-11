---

title: MessagePublishTopology<TMessage>

---

# MessagePublishTopology\<TMessage\>

Namespace: MassTransit.Topology

```csharp
public class MessagePublishTopology<TMessage> : IMessagePublishTopologyConfigurator<TMessage>, IMessagePublishTopologyConfigurator, IMessagePublishTopology, ISpecification, IMessagePublishTopology<TMessage>
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessagePublishTopology\<TMessage\>](../masstransit-topology/messagepublishtopology-1)<br/>
Implements [IMessagePublishTopologyConfigurator\<TMessage\>](../masstransit/imessagepublishtopologyconfigurator-1), [IMessagePublishTopologyConfigurator](../masstransit/imessagepublishtopologyconfigurator), [IMessagePublishTopology](../masstransit/imessagepublishtopology), [ISpecification](../masstransit/ispecification), [IMessagePublishTopology\<TMessage\>](../masstransit/imessagepublishtopology-1)

## Properties

### **Exclude**

```csharp
public bool Exclude { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **MessagePublishTopology(IPublishTopology)**

```csharp
public MessagePublishTopology(IPublishTopology publishTopology)
```

#### Parameters

`publishTopology` [IPublishTopology](../masstransit/ipublishtopology)<br/>

## Methods

### **Add(IMessagePublishTopology\<TMessage\>)**

```csharp
public void Add(IMessagePublishTopology<TMessage> publishTopology)
```

#### Parameters

`publishTopology` [IMessagePublishTopology\<TMessage\>](../masstransit/imessagepublishtopology-1)<br/>

### **AddDelegate(IMessagePublishTopology\<TMessage\>)**

```csharp
public void AddDelegate(IMessagePublishTopology<TMessage> configuration)
```

#### Parameters

`configuration` [IMessagePublishTopology\<TMessage\>](../masstransit/imessagepublishtopology-1)<br/>

### **Apply(ITopologyPipeBuilder\<PublishContext\<TMessage\>\>)**

```csharp
public void Apply(ITopologyPipeBuilder<PublishContext<TMessage>> builder)
```

#### Parameters

`builder` [ITopologyPipeBuilder\<PublishContext\<TMessage\>\>](../masstransit-configuration/itopologypipebuilder-1)<br/>

### **TryGetPublishAddress(Uri, Uri)**

```csharp
public bool TryGetPublishAddress(Uri baseAddress, out Uri publishAddress)
```

#### Parameters

`baseAddress` Uri<br/>

`publishAddress` Uri<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryAddConvention(IMessagePublishTopologyConvention\<TMessage\>)**

```csharp
public bool TryAddConvention(IMessagePublishTopologyConvention<TMessage> convention)
```

#### Parameters

`convention` [IMessagePublishTopologyConvention\<TMessage\>](../masstransit-configuration/imessagepublishtopologyconvention-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryAddConvention(IPublishTopologyConvention)**

```csharp
public bool TryAddConvention(IPublishTopologyConvention convention)
```

#### Parameters

`convention` [IPublishTopologyConvention](../masstransit-configuration/ipublishtopologyconvention)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **AddOrUpdateConvention\<TConvention\>(Func\<TConvention\>, Func\<TConvention, TConvention\>)**

```csharp
public void AddOrUpdateConvention<TConvention>(Func<TConvention> add, Func<TConvention, TConvention> update)
```

#### Type Parameters

`TConvention`<br/>

#### Parameters

`add` [Func\<TConvention\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

`update` [Func\<TConvention, TConvention\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
