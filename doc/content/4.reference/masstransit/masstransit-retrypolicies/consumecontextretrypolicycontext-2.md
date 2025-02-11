---

title: ConsumeContextRetryPolicyContext<TFilter, TContext>

---

# ConsumeContextRetryPolicyContext\<TFilter, TContext\>

Namespace: MassTransit.RetryPolicies

```csharp
public class ConsumeContextRetryPolicyContext<TFilter, TContext> : RetryPolicyContext<TFilter>, IDisposable
```

#### Type Parameters

`TFilter`<br/>

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumeContextRetryPolicyContext\<TFilter, TContext\>](../masstransit-retrypolicies/consumecontextretrypolicycontext-2)<br/>
Implements [RetryPolicyContext\<TFilter\>](../../masstransit-abstractions/masstransit/retrypolicycontext-1), [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)

## Properties

### **Context**

```csharp
public TFilter Context { get; }
```

#### Property Value

TFilter<br/>

## Constructors

### **ConsumeContextRetryPolicyContext(RetryPolicyContext\<TFilter\>, TContext, CancellationToken)**

```csharp
public ConsumeContextRetryPolicyContext(RetryPolicyContext<TFilter> policyContext, TContext context, CancellationToken cancellationToken)
```

#### Parameters

`policyContext` [RetryPolicyContext\<TFilter\>](../../masstransit-abstractions/masstransit/retrypolicycontext-1)<br/>

`context` TContext<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Methods

### **Cancel()**

```csharp
public void Cancel()
```

### **CanRetry(Exception, RetryContext\<TFilter\>)**

```csharp
public bool CanRetry(Exception exception, out RetryContext<TFilter> retryContext)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`retryContext` [RetryContext\<TFilter\>](../../masstransit-abstractions/masstransit/retrycontext-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **RetryFaulted(Exception)**

```csharp
public Task RetryFaulted(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Dispose()**

```csharp
public void Dispose()
```
