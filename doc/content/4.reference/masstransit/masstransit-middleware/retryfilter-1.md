---

title: RetryFilter<TContext>

---

# RetryFilter\<TContext\>

Namespace: MassTransit.Middleware

Uses a retry policy to handle exceptions, retrying the operation in according
 with the policy

```csharp
public class RetryFilter<TContext> : IFilter<TContext>, IProbeSite
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RetryFilter\<TContext\>](../masstransit-middleware/retryfilter-1)<br/>
Implements [IFilter\<TContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **RetryFilter(IRetryPolicy, RetryObservable)**

```csharp
public RetryFilter(IRetryPolicy retryPolicy, RetryObservable observers)
```

#### Parameters

`retryPolicy` [IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

`observers` [RetryObservable](../../masstransit-abstractions/masstransit-observables/retryobservable)<br/>
