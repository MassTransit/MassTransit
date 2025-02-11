---

title: InterceptPipeSpecification<TContext>

---

# InterceptPipeSpecification\<TContext\>

Namespace: MassTransit.Configuration

Adds a fork to the pipe

```csharp
public class InterceptPipeSpecification<TContext> : IPipeSpecification<TContext>, ISpecification
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InterceptPipeSpecification\<TContext\>](../masstransit-configuration/interceptpipespecification-1)<br/>
Implements [IPipeSpecification\<TContext\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **InterceptPipeSpecification(IPipe\<TContext\>)**

```csharp
public InterceptPipeSpecification(IPipe<TContext> pipe)
```

#### Parameters

`pipe` [IPipe\<TContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

## Methods

### **Apply(IPipeBuilder\<TContext\>)**

```csharp
public void Apply(IPipeBuilder<TContext> builder)
```

#### Parameters

`builder` [IPipeBuilder\<TContext\>](../../masstransit-abstractions/masstransit-configuration/ipipebuilder-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
