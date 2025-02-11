---

title: CircuitBreakerFilter<TContext>

---

# CircuitBreakerFilter\<TContext\>

Namespace: MassTransit.Middleware

```csharp
public class CircuitBreakerFilter<TContext> : IFilter<TContext>, IProbeSite, ICircuitBreaker
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CircuitBreakerFilter\<TContext\>](../masstransit-middleware/circuitbreakerfilter-1)<br/>
Implements [IFilter\<TContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [ICircuitBreaker](../masstransit-middleware-circuitbreaker/icircuitbreaker)

## Properties

### **OpenDuration**

```csharp
public TimeSpan OpenDuration { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **TripThreshold**

```csharp
public int TripThreshold { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ActiveThreshold**

```csharp
public int ActiveThreshold { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **CircuitBreakerFilter(CircuitBreakerSettings, IExceptionFilter)**

```csharp
public CircuitBreakerFilter(CircuitBreakerSettings settings, IExceptionFilter exceptionFilter)
```

#### Parameters

`settings` [CircuitBreakerSettings](../masstransit-middleware-circuitbreaker/circuitbreakersettings)<br/>

`exceptionFilter` [IExceptionFilter](../../masstransit-abstractions/masstransit/iexceptionfilter)<br/>

## Methods

### **Send(TContext, IPipe\<TContext\>)**

```csharp
public Task Send(TContext context, IPipe<TContext> next)
```

#### Parameters

`context` TContext<br/>

`next` [IPipe\<TContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
