---

title: PartitionSagaSpecification<TSaga>

---

# PartitionSagaSpecification\<TSaga\>

Namespace: MassTransit.Configuration

```csharp
public class PartitionSagaSpecification<TSaga> : IPipeSpecification<SagaConsumeContext<TSaga>>, ISpecification
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PartitionSagaSpecification\<TSaga\>](../masstransit-configuration/partitionsagaspecification-1)<br/>
Implements [IPipeSpecification\<SagaConsumeContext\<TSaga\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **PartitionSagaSpecification(IPartitioner, PartitionKeyProvider\<SagaConsumeContext\<TSaga\>\>)**

```csharp
public PartitionSagaSpecification(IPartitioner partitioner, PartitionKeyProvider<SagaConsumeContext<TSaga>> keyProvider)
```

#### Parameters

`partitioner` [IPartitioner](../masstransit/ipartitioner)<br/>

`keyProvider` [PartitionKeyProvider\<SagaConsumeContext\<TSaga\>\>](../masstransit/partitionkeyprovider-1)<br/>

## Methods

### **Apply(IPipeBuilder\<SagaConsumeContext\<TSaga\>\>)**

```csharp
public void Apply(IPipeBuilder<SagaConsumeContext<TSaga>> builder)
```

#### Parameters

`builder` [IPipeBuilder\<SagaConsumeContext\<TSaga\>\>](../../masstransit-abstractions/masstransit-configuration/ipipebuilder-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
