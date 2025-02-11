---

title: FilterPipeSpecification<TContext>

---

# FilterPipeSpecification\<TContext\>

Namespace: MassTransit.Configuration

Adds an arbitrary filter to the pipe

```csharp
public class FilterPipeSpecification<TContext> : IPipeSpecification<TContext>, ISpecification
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FilterPipeSpecification\<TContext\>](../masstransit-configuration/filterpipespecification-1)<br/>
Implements [IPipeSpecification\<TContext\>](../masstransit-configuration/ipipespecification-1), [ISpecification](../masstransit/ispecification)

## Constructors

### **FilterPipeSpecification(IFilter\<TContext\>)**

```csharp
public FilterPipeSpecification(IFilter<TContext> filter)
```

#### Parameters

`filter` [IFilter\<TContext\>](../masstransit/ifilter-1)<br/>

## Methods

### **Apply(IPipeBuilder\<TContext\>)**

```csharp
public void Apply(IPipeBuilder<TContext> builder)
```

#### Parameters

`builder` [IPipeBuilder\<TContext\>](../masstransit-configuration/ipipebuilder-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
