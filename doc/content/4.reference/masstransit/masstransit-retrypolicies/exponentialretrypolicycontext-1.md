---

title: ExponentialRetryPolicyContext<TContext>

---

# ExponentialRetryPolicyContext\<TContext\>

Namespace: MassTransit.RetryPolicies

```csharp
public class ExponentialRetryPolicyContext<TContext> : BaseRetryPolicyContext<TContext>, RetryPolicyContext<TContext>, IDisposable
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BaseRetryPolicyContext\<TContext\>](../masstransit-retrypolicies/baseretrypolicycontext-1) → [ExponentialRetryPolicyContext\<TContext\>](../masstransit-retrypolicies/exponentialretrypolicycontext-1)<br/>
Implements [RetryPolicyContext\<TContext\>](../../masstransit-abstractions/masstransit/retrypolicycontext-1), [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)

## Properties

### **Context**

```csharp
public TContext Context { get; }
```

#### Property Value

TContext<br/>

## Constructors

### **ExponentialRetryPolicyContext(ExponentialRetryPolicy, TContext)**

```csharp
public ExponentialRetryPolicyContext(ExponentialRetryPolicy policy, TContext context)
```

#### Parameters

`policy` [ExponentialRetryPolicy](../masstransit-retrypolicies/exponentialretrypolicy)<br/>

`context` TContext<br/>

## Methods

### **CreateRetryContext(Exception, CancellationToken)**

```csharp
protected RetryContext<TContext> CreateRetryContext(Exception exception, CancellationToken cancellationToken)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[RetryContext\<TContext\>](../../masstransit-abstractions/masstransit/retrycontext-1)<br/>
