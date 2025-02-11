---

title: ICircuitBreakerConfigurator<TContext>

---

# ICircuitBreakerConfigurator\<TContext\>

Namespace: MassTransit

Configure the settings on the circuit breaker

```csharp
public interface ICircuitBreakerConfigurator<TContext> : IExceptionConfigurator
```

#### Type Parameters

`TContext`<br/>

Implements [IExceptionConfigurator](../../masstransit-abstractions/masstransit/iexceptionconfigurator)

## Properties

### **TrackingPeriod**

The period after which the attempt/failure counts are reset.

```csharp
public abstract TimeSpan TrackingPeriod { set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **TripThreshold**

The percentage of attempts that must fail before the circuit breaker trips into
 an open state.

```csharp
public abstract int TripThreshold { set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ActiveThreshold**

The number of attempts that must occur before the circuit breaker becomes active. Until the
 breaker activates, it will not open on failure

```csharp
public abstract int ActiveThreshold { set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ResetInterval**

Sets a specific reset interval for the circuit to attempt to close after being tripped.
 By default, this is an incrementing scale up to one minute.

```csharp
public abstract TimeSpan ResetInterval { set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **Router**

Configure a router for sending events from the circuit breaker

```csharp
public abstract IPipeRouter Router { set; }
```

#### Property Value

[IPipeRouter](../masstransit-middleware/ipiperouter)<br/>
