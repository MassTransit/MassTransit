---

title: HalfOpenBehavior

---

# HalfOpenBehavior

Namespace: MassTransit.Middleware.CircuitBreaker

Executes until the success count is met. If a fault occurs before the success
 count is reached, the circuit reopens.

```csharp
public class HalfOpenBehavior : ICircuitBreakerBehavior, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [HalfOpenBehavior](../masstransit-middleware-circuitbreaker/halfopenbehavior)<br/>
Implements [ICircuitBreakerBehavior](../masstransit-middleware-circuitbreaker/icircuitbreakerbehavior), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **HalfOpenBehavior(ICircuitBreaker, Exception, IEnumerator\<TimeSpan\>)**

```csharp
public HalfOpenBehavior(ICircuitBreaker breaker, Exception exception, IEnumerator<TimeSpan> timeoutEnumerator)
```

#### Parameters

`breaker` [ICircuitBreaker](../masstransit-middleware-circuitbreaker/icircuitbreaker)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`timeoutEnumerator` [IEnumerator\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerator-1)<br/>
