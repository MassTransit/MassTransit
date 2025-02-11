---

title: TimeoutSpecification<T>

---

# TimeoutSpecification\<T\>

Namespace: MassTransit.Configuration

```csharp
public class TimeoutSpecification<T> : IPipeSpecification<ConsumeContext<T>>, ISpecification, ITimeoutConfigurator
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TimeoutSpecification\<T\>](../masstransit-configuration/timeoutspecification-1)<br/>
Implements [IPipeSpecification\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [ITimeoutConfigurator](../masstransit/itimeoutconfigurator)

## Properties

### **Timeout**

```csharp
public TimeSpan Timeout { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

## Constructors

### **TimeoutSpecification()**

```csharp
public TimeoutSpecification()
```

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
