---

title: ConcurrencyLimitConsumePipeSpecification<T>

---

# ConcurrencyLimitConsumePipeSpecification\<T\>

Namespace: MassTransit.Configuration

Adds a concurrency limit filter to the message pipe.

```csharp
public class ConcurrencyLimitConsumePipeSpecification<T> : IPipeSpecification<ConsumeContext<T>>, ISpecification
```

#### Type Parameters

`T`<br/>
The message type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConcurrencyLimitConsumePipeSpecification\<T\>](../masstransit-configuration/concurrencylimitconsumepipespecification-1)<br/>
Implements [IPipeSpecification\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **ConcurrencyLimitConsumePipeSpecification(IConcurrencyLimiter)**

```csharp
public ConcurrencyLimitConsumePipeSpecification(IConcurrencyLimiter limiter)
```

#### Parameters

`limiter` [IConcurrencyLimiter](../masstransit-middleware/iconcurrencylimiter)<br/>

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
