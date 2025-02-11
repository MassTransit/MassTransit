---

title: MessagePublishTopologyPipeSpecification<TMessage>

---

# MessagePublishTopologyPipeSpecification\<TMessage\>

Namespace: MassTransit.Configuration

```csharp
public class MessagePublishTopologyPipeSpecification<TMessage> : ISpecificationPipeSpecification<PublishContext<TMessage>>, ISpecification
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessagePublishTopologyPipeSpecification\<TMessage\>](../masstransit-configuration/messagepublishtopologypipespecification-1)<br/>
Implements [ISpecificationPipeSpecification\<PublishContext\<TMessage\>\>](../masstransit-configuration/ispecificationpipespecification-1), [ISpecification](../masstransit/ispecification)

## Constructors

### **MessagePublishTopologyPipeSpecification(IMessagePublishTopology\<TMessage\>)**

```csharp
public MessagePublishTopologyPipeSpecification(IMessagePublishTopology<TMessage> messagePublishTopology)
```

#### Parameters

`messagePublishTopology` [IMessagePublishTopology\<TMessage\>](../masstransit/imessagepublishtopology-1)<br/>

## Methods

### **Apply(ISpecificationPipeBuilder\<PublishContext\<TMessage\>\>)**

```csharp
public void Apply(ISpecificationPipeBuilder<PublishContext<TMessage>> builder)
```

#### Parameters

`builder` [ISpecificationPipeBuilder\<PublishContext\<TMessage\>\>](../masstransit-configuration/ispecificationpipebuilder-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
