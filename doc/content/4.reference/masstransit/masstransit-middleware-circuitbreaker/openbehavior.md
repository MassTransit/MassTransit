---

title: OpenBehavior

---

# OpenBehavior

Namespace: MassTransit.Middleware.CircuitBreaker

Represents a circuit that is unavailable, with a timer waiting to partially close
 the circuit.

```csharp
public class OpenBehavior : ICircuitBreakerBehavior, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [OpenBehavior](../masstransit-middleware-circuitbreaker/openbehavior)<br/>
Implements [ICircuitBreakerBehavior](../masstransit-middleware-circuitbreaker/icircuitbreakerbehavior), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **OpenBehavior(ICircuitBreaker, Exception, IEnumerator\<TimeSpan\>)**

```csharp
public OpenBehavior(ICircuitBreaker breaker, Exception exception, IEnumerator<TimeSpan> timeoutEnumerator)
```

#### Parameters

`breaker` [ICircuitBreaker](../masstransit-middleware-circuitbreaker/icircuitbreaker)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`timeoutEnumerator` [IEnumerator\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerator-1)<br/>
