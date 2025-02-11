---

title: PartitionMessageSpecification<T>

---

# PartitionMessageSpecification\<T\>

Namespace: MassTransit.Configuration

```csharp
public class PartitionMessageSpecification<T> : IPipeSpecification<ConsumeContext<T>>, ISpecification
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PartitionMessageSpecification\<T\>](../masstransit-configuration/partitionmessagespecification-1)<br/>
Implements [IPipeSpecification\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **PartitionMessageSpecification(IPartitioner)**

```csharp
public PartitionMessageSpecification(IPartitioner partitioner)
```

#### Parameters

`partitioner` [IPartitioner](../masstransit/ipartitioner)<br/>

## Methods

### **Apply(IPipeBuilder\<ConsumeContext\<T\>\>)**

```csharp
public void Apply(IPipeBuilder<ConsumeContext<T>> builder)
```

#### Parameters

`builder` [IPipeBuilder\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit-configuration/ipipebuilder-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
