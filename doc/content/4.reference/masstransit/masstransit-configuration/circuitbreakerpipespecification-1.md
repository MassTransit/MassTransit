---

title: CircuitBreakerPipeSpecification<T>

---

# CircuitBreakerPipeSpecification\<T\>

Namespace: MassTransit.Configuration

```csharp
public class CircuitBreakerPipeSpecification<T> : ExceptionSpecification, IExceptionConfigurator, IPipeSpecification<T>, ISpecification, ICircuitBreakerConfigurator<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExceptionSpecification](../../masstransit-abstractions/masstransit-configuration/exceptionspecification) → [CircuitBreakerPipeSpecification\<T\>](../masstransit-configuration/circuitbreakerpipespecification-1)<br/>
Implements [IExceptionConfigurator](../../masstransit-abstractions/masstransit/iexceptionconfigurator), [IPipeSpecification\<T\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [ICircuitBreakerConfigurator\<T\>](../masstransit/icircuitbreakerconfigurator-1)

## Properties

### **TrackingPeriod**

```csharp
public TimeSpan TrackingPeriod { set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **TripThreshold**

```csharp
public int TripThreshold { set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ActiveThreshold**

```csharp
public int ActiveThreshold { set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ResetInterval**

```csharp
public TimeSpan ResetInterval { set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **Router**

```csharp
public IPipeRouter Router { set; }
```

#### Property Value

[IPipeRouter](../masstransit-middleware/ipiperouter)<br/>

## Constructors

### **CircuitBreakerPipeSpecification()**

```csharp
public CircuitBreakerPipeSpecification()
```

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
