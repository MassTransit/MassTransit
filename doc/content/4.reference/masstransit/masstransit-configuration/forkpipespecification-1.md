---

title: ForkPipeSpecification<TContext>

---

# ForkPipeSpecification\<TContext\>

Namespace: MassTransit.Configuration

Adds a fork to the pipe

```csharp
public class ForkPipeSpecification<TContext> : IPipeSpecification<TContext>, ISpecification
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ForkPipeSpecification\<TContext\>](../masstransit-configuration/forkpipespecification-1)<br/>
Implements [IPipeSpecification\<TContext\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **ForkPipeSpecification(IPipe\<TContext\>)**

```csharp
public ForkPipeSpecification(IPipe<TContext> pipe)
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
