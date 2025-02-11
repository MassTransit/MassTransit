---

title: CompensateContextTimeoutSpecification<TArguments>

---

# CompensateContextTimeoutSpecification\<TArguments\>

Namespace: MassTransit.Configuration

```csharp
public class CompensateContextTimeoutSpecification<TArguments> : IPipeSpecification<CompensateContext<TArguments>>, ISpecification, ITimeoutConfigurator
```

#### Type Parameters

`TArguments`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CompensateContextTimeoutSpecification\<TArguments\>](../masstransit-configuration/compensatecontexttimeoutspecification-1)<br/>
Implements [IPipeSpecification\<CompensateContext\<TArguments\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [ITimeoutConfigurator](../masstransit/itimeoutconfigurator)

## Properties

### **Timeout**

```csharp
public TimeSpan Timeout { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

## Constructors

### **CompensateContextTimeoutSpecification()**

```csharp
public CompensateContextTimeoutSpecification()
```

## Methods

### **Apply(IPipeBuilder\<CompensateContext\<TArguments\>\>)**

```csharp
public void Apply(IPipeBuilder<CompensateContext<TArguments>> builder)
```

#### Parameters

`builder` [IPipeBuilder\<CompensateContext\<TArguments\>\>](../../masstransit-abstractions/masstransit-configuration/ipipebuilder-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
