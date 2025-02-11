---

title: InlineFilterPipeSpecification<TContext>

---

# InlineFilterPipeSpecification\<TContext\>

Namespace: MassTransit.Configuration

Adds an arbitrary filter to the pipe

```csharp
public class InlineFilterPipeSpecification<TContext> : IPipeSpecification<TContext>, ISpecification
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InlineFilterPipeSpecification\<TContext\>](../masstransit-configuration/inlinefilterpipespecification-1)<br/>
Implements [IPipeSpecification\<TContext\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **InlineFilterPipeSpecification(InlineFilterMethod\<TContext\>)**

```csharp
public InlineFilterPipeSpecification(InlineFilterMethod<TContext> filterMethod)
```

#### Parameters

`filterMethod` [InlineFilterMethod\<TContext\>](../masstransit/inlinefiltermethod-1)<br/>

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
