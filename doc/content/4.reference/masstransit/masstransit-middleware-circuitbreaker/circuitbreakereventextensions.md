---

title: CircuitBreakerEventExtensions

---

# CircuitBreakerEventExtensions

Namespace: MassTransit.Middleware.CircuitBreaker

```csharp
public static class CircuitBreakerEventExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CircuitBreakerEventExtensions](../masstransit-middleware-circuitbreaker/circuitbreakereventextensions)

## Methods

### **PublishCircuitBreakerOpened(IPipe\<EventContext\>, Exception)**

Set the concurrency limit of the filter

```csharp
public static Task PublishCircuitBreakerOpened(IPipe<EventContext> pipe, Exception exception)
```

#### Parameters

`pipe` [IPipe\<EventContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PublishCircuitBreakerClosed(IPipe\<EventContext\>)**

Set the concurrency limit of the filter

```csharp
public static Task PublishCircuitBreakerClosed(IPipe<EventContext> pipe)
```

#### Parameters

`pipe` [IPipe\<EventContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
