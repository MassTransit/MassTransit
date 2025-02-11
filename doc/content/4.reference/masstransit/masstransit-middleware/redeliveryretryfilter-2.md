---

title: RedeliveryRetryFilter<TContext, TMessage>

---

# RedeliveryRetryFilter\<TContext, TMessage\>

Namespace: MassTransit.Middleware

Uses the message redelivery mechanism, if available, to delay a retry without blocking message delivery

```csharp
public class RedeliveryRetryFilter<TContext, TMessage> : IFilter<TContext>, IProbeSite
```

#### Type Parameters

`TContext`<br/>
The context type

`TMessage`<br/>
The message type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RedeliveryRetryFilter\<TContext, TMessage\>](../masstransit-middleware/redeliveryretryfilter-2)<br/>
Implements [IFilter\<TContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **RedeliveryRetryFilter(IRetryPolicy, RetryObservable)**

```csharp
public RedeliveryRetryFilter(IRetryPolicy retryPolicy, RetryObservable observers)
```

#### Parameters

`retryPolicy` [IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

`observers` [RetryObservable](../../masstransit-abstractions/masstransit-observables/retryobservable)<br/>

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
