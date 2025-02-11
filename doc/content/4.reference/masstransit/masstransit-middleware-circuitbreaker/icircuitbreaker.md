---

title: ICircuitBreaker

---

# ICircuitBreaker

Namespace: MassTransit.Middleware.CircuitBreaker

Provides access to a circuit breaker from a state object

```csharp
public interface ICircuitBreaker
```

## Properties

### **TripThreshold**

The number of failures before opening the circuit breaker

```csharp
public abstract int TripThreshold { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ActiveThreshold**

The minimum number of attempts before the breaker can possibly trip

```csharp
public abstract int ActiveThreshold { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **OpenDuration**

Window duration before attempt/success/failure counts are reset

```csharp
public abstract TimeSpan OpenDuration { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

## Methods

### **Open(Exception, ICircuitBreakerBehavior, IEnumerator\<TimeSpan\>)**

Open the circuit breaker, preventing any further access to the resource until
 the timer expires

```csharp
Task Open(Exception exception, ICircuitBreakerBehavior behavior, IEnumerator<TimeSpan> timeoutEnumerator)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>
The exception to return when the circuit breaker is accessed

`behavior` [ICircuitBreakerBehavior](../masstransit-middleware-circuitbreaker/icircuitbreakerbehavior)<br/>

`timeoutEnumerator` [IEnumerator\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerator-1)<br/>
A previously created enumerator for a timeout period

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ClosePartially(Exception, IEnumerator\<TimeSpan\>, ICircuitBreakerBehavior)**

Partially open the circuit breaker, allowing the eventual return to a closed
 state

```csharp
Task ClosePartially(Exception exception, IEnumerator<TimeSpan> timeoutEnumerator, ICircuitBreakerBehavior behavior)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`timeoutEnumerator` [IEnumerator\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerator-1)<br/>

`behavior` [ICircuitBreakerBehavior](../masstransit-middleware-circuitbreaker/icircuitbreakerbehavior)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Close(ICircuitBreakerBehavior)**

Close the circuit breaker, allowing normal execution

```csharp
Task Close(ICircuitBreakerBehavior behavior)
```

#### Parameters

`behavior` [ICircuitBreakerBehavior](../masstransit-middleware-circuitbreaker/icircuitbreakerbehavior)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
