---

title: ContextFilterPipeSpecification<TContext>

---

# ContextFilterPipeSpecification\<TContext\>

Namespace: MassTransit.Configuration

```csharp
public class ContextFilterPipeSpecification<TContext> : IPipeSpecification<TContext>, ISpecification
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ContextFilterPipeSpecification\<TContext\>](../masstransit-configuration/contextfilterpipespecification-1)<br/>
Implements [IPipeSpecification\<TContext\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **ContextFilterPipeSpecification(Func\<TContext, Task\<Boolean\>\>)**

```csharp
public ContextFilterPipeSpecification(Func<TContext, Task<bool>> filter)
```

#### Parameters

`filter` [Func\<TContext, Task\<Boolean\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

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
