---

title: ExecuteContextTimeoutSpecification<TArguments>

---

# ExecuteContextTimeoutSpecification\<TArguments\>

Namespace: MassTransit.Configuration

```csharp
public class ExecuteContextTimeoutSpecification<TArguments> : IPipeSpecification<ExecuteContext<TArguments>>, ISpecification, ITimeoutConfigurator
```

#### Type Parameters

`TArguments`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExecuteContextTimeoutSpecification\<TArguments\>](../masstransit-configuration/executecontexttimeoutspecification-1)<br/>
Implements [IPipeSpecification\<ExecuteContext\<TArguments\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [ITimeoutConfigurator](../masstransit/itimeoutconfigurator)

## Properties

### **Timeout**

```csharp
public TimeSpan Timeout { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

## Constructors

### **ExecuteContextTimeoutSpecification()**

```csharp
public ExecuteContextTimeoutSpecification()
```

## Methods

### **Apply(IPipeBuilder\<ExecuteContext\<TArguments\>\>)**

```csharp
public void Apply(IPipeBuilder<ExecuteContext<TArguments>> builder)
```

#### Parameters

`builder` [IPipeBuilder\<ExecuteContext\<TArguments\>\>](../../masstransit-abstractions/masstransit-configuration/ipipebuilder-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
