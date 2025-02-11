---

title: RateLimitPipeSpecification<T>

---

# RateLimitPipeSpecification\<T\>

Namespace: MassTransit.Configuration

```csharp
public class RateLimitPipeSpecification<T> : IPipeSpecification<T>, ISpecification
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RateLimitPipeSpecification\<T\>](../masstransit-configuration/ratelimitpipespecification-1)<br/>
Implements [IPipeSpecification\<T\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **RateLimitPipeSpecification(Int32, TimeSpan, IPipeRouter)**

```csharp
public RateLimitPipeSpecification(int rateLimit, TimeSpan interval, IPipeRouter router)
```

#### Parameters

`rateLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`interval` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

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
