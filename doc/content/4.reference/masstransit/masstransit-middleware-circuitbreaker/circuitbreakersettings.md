---

title: CircuitBreakerSettings

---

# CircuitBreakerSettings

Namespace: MassTransit.Middleware.CircuitBreaker

```csharp
public interface CircuitBreakerSettings
```

## Properties

### **TrackingPeriod**

The window duration to keep track of errors before they fall off the breaker state

```csharp
public abstract TimeSpan TrackingPeriod { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **ResetTimeout**

The time to wait after the breaker has opened before attempting to close it

```csharp
public abstract IEnumerable<TimeSpan> ResetTimeout { get; }
```

#### Property Value

[IEnumerable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **TripThreshold**

A percentage of how many failures versus successful calls before the breaker
 is opened. Should be 0-100, but seriously like 5-10.

```csharp
public abstract int TripThreshold { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ActiveThreshold**

The active count of attempts before the circuit breaker can be tripped

```csharp
public abstract int ActiveThreshold { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Router**

The router used to publish events related to the circuit breaker behavior

```csharp
public abstract IPipeRouter Router { get; }
```

#### Property Value

[IPipeRouter](../masstransit-middleware/ipiperouter)<br/>
