---

title: PartitionConsumerSpecification<TConsumer>

---

# PartitionConsumerSpecification\<TConsumer\>

Namespace: MassTransit.Configuration

```csharp
public class PartitionConsumerSpecification<TConsumer> : IPipeSpecification<ConsumerConsumeContext<TConsumer>>, ISpecification
```

#### Type Parameters

`TConsumer`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PartitionConsumerSpecification\<TConsumer\>](../masstransit-configuration/partitionconsumerspecification-1)<br/>
Implements [IPipeSpecification\<ConsumerConsumeContext\<TConsumer\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **PartitionConsumerSpecification(IPartitioner, PartitionKeyProvider\<ConsumerConsumeContext\<TConsumer\>\>)**

```csharp
public PartitionConsumerSpecification(IPartitioner partitioner, PartitionKeyProvider<ConsumerConsumeContext<TConsumer>> keyProvider)
```

#### Parameters

`partitioner` [IPartitioner](../masstransit/ipartitioner)<br/>

`keyProvider` [PartitionKeyProvider\<ConsumerConsumeContext\<TConsumer\>\>](../masstransit/partitionkeyprovider-1)<br/>

## Methods

### **Apply(IPipeBuilder\<ConsumerConsumeContext\<TConsumer\>\>)**

```csharp
public void Apply(IPipeBuilder<ConsumerConsumeContext<TConsumer>> builder)
```

#### Parameters

`builder` [IPipeBuilder\<ConsumerConsumeContext\<TConsumer\>\>](../../masstransit-abstractions/masstransit-configuration/ipipebuilder-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
