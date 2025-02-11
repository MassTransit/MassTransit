---

title: BaseRetryPolicyContext<TContext>

---

# BaseRetryPolicyContext\<TContext\>

Namespace: MassTransit.RetryPolicies

```csharp
public abstract class BaseRetryPolicyContext<TContext> : RetryPolicyContext<TContext>, IDisposable
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BaseRetryPolicyContext\<TContext\>](../masstransit-retrypolicies/baseretrypolicycontext-1)<br/>
Implements [RetryPolicyContext\<TContext\>](../../masstransit-abstractions/masstransit/retrypolicycontext-1), [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)

## Properties

### **Context**

```csharp
public TContext Context { get; }
```

#### Property Value

TContext<br/>

## Methods

### **CanRetry(Exception, RetryContext\<TContext\>)**

```csharp
public bool CanRetry(Exception exception, out RetryContext<TContext> retryContext)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`retryContext` [RetryContext\<TContext\>](../../masstransit-abstractions/masstransit/retrycontext-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Cancel()**

```csharp
public void Cancel()
```

### **CreateRetryContext(Exception, CancellationToken)**

```csharp
protected abstract RetryContext<TContext> CreateRetryContext(Exception exception, CancellationToken cancellationToken)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[RetryContext\<TContext\>](../../masstransit-abstractions/masstransit/retrycontext-1)<br/>
