---

title: MessageConsumeTopologyPipeSpecification<TMessage>

---

# MessageConsumeTopologyPipeSpecification\<TMessage\>

Namespace: MassTransit.Configuration

```csharp
public class MessageConsumeTopologyPipeSpecification<TMessage> : ISpecificationPipeSpecification<ConsumeContext<TMessage>>, ISpecification
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageConsumeTopologyPipeSpecification\<TMessage\>](../masstransit-configuration/messageconsumetopologypipespecification-1)<br/>
Implements [ISpecificationPipeSpecification\<ConsumeContext\<TMessage\>\>](../masstransit-configuration/ispecificationpipespecification-1), [ISpecification](../masstransit/ispecification)

## Constructors

### **MessageConsumeTopologyPipeSpecification(IMessageConsumeTopology\<TMessage\>)**

```csharp
public MessageConsumeTopologyPipeSpecification(IMessageConsumeTopology<TMessage> messageConsumeTopology)
```

#### Parameters

`messageConsumeTopology` [IMessageConsumeTopology\<TMessage\>](../masstransit/imessageconsumetopology-1)<br/>

## Methods

### **Apply(ISpecificationPipeBuilder\<ConsumeContext\<TMessage\>\>)**

```csharp
public void Apply(ISpecificationPipeBuilder<ConsumeContext<TMessage>> builder)
```

#### Parameters

`builder` [ISpecificationPipeBuilder\<ConsumeContext\<TMessage\>\>](../masstransit-configuration/ispecificationpipebuilder-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
