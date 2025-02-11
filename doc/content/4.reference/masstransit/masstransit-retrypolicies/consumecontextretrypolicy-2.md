---

title: ConsumeContextRetryPolicy<TFilter, TContext>

---

# ConsumeContextRetryPolicy\<TFilter, TContext\>

Namespace: MassTransit.RetryPolicies

```csharp
public class ConsumeContextRetryPolicy<TFilter, TContext> : IRetryPolicy, IProbeSite
```

#### Type Parameters

`TFilter`<br/>

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumeContextRetryPolicy\<TFilter, TContext\>](../masstransit-retrypolicies/consumecontextretrypolicy-2)<br/>
Implements [IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ConsumeContextRetryPolicy(IRetryPolicy, CancellationToken, Func\<TFilter, IRetryPolicy, RetryContext, TContext\>)**

```csharp
public ConsumeContextRetryPolicy(IRetryPolicy retryPolicy, CancellationToken cancellationToken, Func<TFilter, IRetryPolicy, RetryContext, TContext> contextFactory)
```

#### Parameters

`retryPolicy` [IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`contextFactory` [Func\<TFilter, IRetryPolicy, RetryContext, TContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-4)<br/>

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **IsHandled(Exception)**

```csharp
public bool IsHandled(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
