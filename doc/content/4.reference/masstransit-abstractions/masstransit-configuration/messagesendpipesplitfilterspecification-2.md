---

title: MessageSendPipeSplitFilterSpecification<TMessage, T>

---

# MessageSendPipeSplitFilterSpecification\<TMessage, T\>

Namespace: MassTransit.Configuration

```csharp
public class MessageSendPipeSplitFilterSpecification<TMessage, T> : ISpecificationPipeSpecification<SendContext<TMessage>>, ISpecification
```

#### Type Parameters

`TMessage`<br/>

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageSendPipeSplitFilterSpecification\<TMessage, T\>](../masstransit-configuration/messagesendpipesplitfilterspecification-2)<br/>
Implements [ISpecificationPipeSpecification\<SendContext\<TMessage\>\>](../masstransit-configuration/ispecificationpipespecification-1), [ISpecification](../masstransit/ispecification)

## Constructors

### **MessageSendPipeSplitFilterSpecification(ISpecificationPipeSpecification\<SendContext\<T\>\>)**

```csharp
public MessageSendPipeSplitFilterSpecification(ISpecificationPipeSpecification<SendContext<T>> specification)
```

#### Parameters

`specification` [ISpecificationPipeSpecification\<SendContext\<T\>\>](../masstransit-configuration/ispecificationpipespecification-1)<br/>

## Methods

### **Apply(ISpecificationPipeBuilder\<SendContext\<TMessage\>\>)**

```csharp
public void Apply(ISpecificationPipeBuilder<SendContext<TMessage>> builder)
```

#### Parameters

`builder` [ISpecificationPipeBuilder\<SendContext\<TMessage\>\>](../masstransit-configuration/ispecificationpipebuilder-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
