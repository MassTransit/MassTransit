---

title: ClosedBehavior

---

# ClosedBehavior

Namespace: MassTransit.Middleware.CircuitBreaker

Represents a closed, normally operating circuit breaker state

```csharp
public class ClosedBehavior : ICircuitBreakerBehavior, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ClosedBehavior](../masstransit-middleware-circuitbreaker/closedbehavior)<br/>
Implements [ICircuitBreakerBehavior](../masstransit-middleware-circuitbreaker/icircuitbreakerbehavior), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ClosedBehavior(ICircuitBreaker)**

```csharp
public ClosedBehavior(ICircuitBreaker breaker)
```

#### Parameters

`breaker` [ICircuitBreaker](../masstransit-middleware-circuitbreaker/icircuitbreaker)<br/>
