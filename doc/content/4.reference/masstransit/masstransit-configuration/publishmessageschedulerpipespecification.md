---

title: PublishMessageSchedulerPipeSpecification

---

# PublishMessageSchedulerPipeSpecification

Namespace: MassTransit.Configuration

```csharp
public class PublishMessageSchedulerPipeSpecification : IPipeSpecification<ConsumeContext>, ISpecification
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PublishMessageSchedulerPipeSpecification](../masstransit-configuration/publishmessageschedulerpipespecification)<br/>
Implements [IPipeSpecification\<ConsumeContext\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **PublishMessageSchedulerPipeSpecification()**

```csharp
public PublishMessageSchedulerPipeSpecification()
```

## Methods

### **Apply(IPipeBuilder\<ConsumeContext\>)**

```csharp
public void Apply(IPipeBuilder<ConsumeContext> builder)
```

#### Parameters

`builder` [IPipeBuilder\<ConsumeContext\>](../../masstransit-abstractions/masstransit-configuration/ipipebuilder-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
