---

title: RetryContext<TContext>

---

# RetryContext\<TContext\>

Namespace: MassTransit

The retry context, with the specified context type

```csharp
public interface RetryContext<TContext> : RetryContext
```

#### Type Parameters

`TContext`<br/>
The context type

Implements [RetryContext](../masstransit/retrycontext)

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
