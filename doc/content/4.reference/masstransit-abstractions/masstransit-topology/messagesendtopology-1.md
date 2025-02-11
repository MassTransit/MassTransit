---

title: MessageSendTopology<TMessage>

---

# MessageSendTopology\<TMessage\>

Namespace: MassTransit.Topology

```csharp
public class MessageSendTopology<TMessage> : IMessageSendTopologyConfigurator<TMessage>, IMessageSendTopologyConfigurator, ISpecification, IMessageSendTopology<TMessage>
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageSendTopology\<TMessage\>](../masstransit-topology/messagesendtopology-1)<br/>
Implements [IMessageSendTopologyConfigurator\<TMessage\>](../masstransit/imessagesendtopologyconfigurator-1), [IMessageSendTopologyConfigurator](../masstransit/imessagesendtopologyconfigurator), [ISpecification](../masstransit/ispecification), [IMessageSendTopology\<TMessage\>](../masstransit/imessagesendtopology-1)

## Constructors

### **MessageSendTopology()**

```csharp
public MessageSendTopology()
```

## Methods

### **Add(IMessageSendTopology\<TMessage\>)**

```csharp
public void Add(IMessageSendTopology<TMessage> sendTopology)
```

#### Parameters

`sendTopology` [IMessageSendTopology\<TMessage\>](../masstransit/imessagesendtopology-1)<br/>

### **AddDelegate(IMessageSendTopology\<TMessage\>)**

```csharp
public void AddDelegate(IMessageSendTopology<TMessage> configuration)
```

#### Parameters

`configuration` [IMessageSendTopology\<TMessage\>](../masstransit/imessagesendtopology-1)<br/>

### **Apply(ITopologyPipeBuilder\<SendContext\<TMessage\>\>)**

```csharp
public void Apply(ITopologyPipeBuilder<SendContext<TMessage>> builder)
```

#### Parameters

`builder` [ITopologyPipeBuilder\<SendContext\<TMessage\>\>](../masstransit-configuration/itopologypipebuilder-1)<br/>

### **TryGetConvention\<TConvention\>(TConvention)**

```csharp
public bool TryGetConvention<TConvention>(out TConvention convention)
```

#### Type Parameters

`TConvention`<br/>

#### Parameters

`convention` TConvention<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryAddConvention(IMessageSendTopologyConvention\<TMessage\>)**

```csharp
public bool TryAddConvention(IMessageSendTopologyConvention<TMessage> convention)
```

#### Parameters

`convention` [IMessageSendTopologyConvention\<TMessage\>](../masstransit-configuration/imessagesendtopologyconvention-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryAddConvention(ISendTopologyConvention)**

```csharp
public bool TryAddConvention(ISendTopologyConvention convention)
```

#### Parameters

`convention` [ISendTopologyConvention](../masstransit-configuration/isendtopologyconvention)<br/>

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

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
