---

title: MessagePublishPipeSplitFilterSpecification<TMessage, T>

---

# MessagePublishPipeSplitFilterSpecification\<TMessage, T\>

Namespace: MassTransit.Configuration

```csharp
public class MessagePublishPipeSplitFilterSpecification<TMessage, T> : ISpecificationPipeSpecification<PublishContext<TMessage>>, ISpecification
```

#### Type Parameters

`TMessage`<br/>

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessagePublishPipeSplitFilterSpecification\<TMessage, T\>](../masstransit-configuration/messagepublishpipesplitfilterspecification-2)<br/>
Implements [ISpecificationPipeSpecification\<PublishContext\<TMessage\>\>](../masstransit-configuration/ispecificationpipespecification-1), [ISpecification](../masstransit/ispecification)

## Constructors

### **MessagePublishPipeSplitFilterSpecification(ISpecificationPipeSpecification\<PublishContext\<T\>\>)**

```csharp
public MessagePublishPipeSplitFilterSpecification(ISpecificationPipeSpecification<PublishContext<T>> specification)
```

#### Parameters

`specification` [ISpecificationPipeSpecification\<PublishContext\<T\>\>](../masstransit-configuration/ispecificationpipespecification-1)<br/>

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
