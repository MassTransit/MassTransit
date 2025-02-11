---

title: RetryPolicyContext<TContext>

---

# RetryPolicyContext\<TContext\>

Namespace: MassTransit

An initial context acquired to begin a retry filter

```csharp
public interface RetryPolicyContext<TContext> : IDisposable
```

#### Type Parameters

`TContext`<br/>

Implements [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)

## Properties

### **Context**

The context being managed by the retry policy

```csharp
public abstract TContext Context { get; }
```

#### Property Value

TContext<br/>

## Methods

### **CanRetry(Exception, RetryContext\<TContext\>)**

Determines if the exception can be retried

```csharp
bool CanRetry(Exception exception, out RetryContext<TContext> retryContext)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>
The exception that occurred

`retryContext` [RetryContext\<TContext\>](../masstransit/retrycontext-1)<br/>
The retry context for the retry

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
True if the task should be retried

### **RetryFaulted(Exception)**

Called after the retry attempt has failed

```csharp
Task RetryFaulted(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Cancel()**

Cancel any pending or subsequent retries

```csharp
void Cancel()
```
