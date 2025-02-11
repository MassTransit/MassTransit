---

title: ConcurrencyLimitPipeSpecification<T>

---

# ConcurrencyLimitPipeSpecification\<T\>

Namespace: MassTransit.Configuration

Configures a concurrency limit on the pipe. If the management endpoint is specified,
 the consumer and appropriate mediator is created to handle the adjustment of the limit.

```csharp
public class ConcurrencyLimitPipeSpecification<T> : IPipeSpecification<T>, ISpecification
```

#### Type Parameters

`T`<br/>
The message type being limited

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConcurrencyLimitPipeSpecification\<T\>](../masstransit-configuration/concurrencylimitpipespecification-1)<br/>
Implements [IPipeSpecification\<T\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **ConcurrencyLimitPipeSpecification(Int32, IPipeRouter)**

```csharp
public ConcurrencyLimitPipeSpecification(int concurrencyLimit, IPipeRouter router)
```

#### Parameters

`concurrencyLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`router` [IPipeRouter](../masstransit-middleware/ipiperouter)<br/>

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
