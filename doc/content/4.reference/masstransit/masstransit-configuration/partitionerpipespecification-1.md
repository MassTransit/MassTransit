---

title: PartitionerPipeSpecification<T>

---

# PartitionerPipeSpecification\<T\>

Namespace: MassTransit.Configuration

```csharp
public class PartitionerPipeSpecification<T> : IPipeSpecification<T>, ISpecification
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PartitionerPipeSpecification\<T\>](../masstransit-configuration/partitionerpipespecification-1)<br/>
Implements [IPipeSpecification\<T\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **PartitionerPipeSpecification(PartitionKeyProvider\<T\>, Int32)**

```csharp
public PartitionerPipeSpecification(PartitionKeyProvider<T> keyProvider, int partitionCount)
```

#### Parameters

`keyProvider` [PartitionKeyProvider\<T\>](../masstransit/partitionkeyprovider-1)<br/>

`partitionCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **PartitionerPipeSpecification(PartitionKeyProvider\<T\>, IPartitioner)**

```csharp
public PartitionerPipeSpecification(PartitionKeyProvider<T> keyProvider, IPartitioner partitioner)
```

#### Parameters

`keyProvider` [PartitionKeyProvider\<T\>](../masstransit/partitionkeyprovider-1)<br/>

`partitioner` [IPartitioner](../masstransit/ipartitioner)<br/>

## Methods

### **Apply(IPipeBuilder\<T\>)**

```csharp
public void Apply(IPipeBuilder<T> builder)
```

#### Parameters

`builder` [IPipeBuilder\<T\>](../../masstransit-abstractions/masstransit-configuration/ipipebuilder-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
