---

title: IncrementalRetryContext<TContext>

---

# IncrementalRetryContext\<TContext\>

Namespace: MassTransit.RetryPolicies

```csharp
public class IncrementalRetryContext<TContext> : BaseRetryContext<TContext>, RetryContext, RetryContext<TContext>
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BaseRetryContext\<TContext\>](../masstransit-retrypolicies/baseretrycontext-1) → [IncrementalRetryContext\<TContext\>](../masstransit-retrypolicies/incrementalretrycontext-1)<br/>
Implements [RetryContext](../../masstransit-abstractions/masstransit/retrycontext), [RetryContext\<TContext\>](../../masstransit-abstractions/masstransit/retrycontext-1)

## Properties

### **Delay**

```csharp
public Nullable<TimeSpan> Delay { get; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Context**

```csharp
public TContext Context { get; }
```

#### Property Value

TContext<br/>

### **CancellationToken**

```csharp
public CancellationToken CancellationToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **Exception**

```csharp
public Exception Exception { get; }
```

#### Property Value

[Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

### **RetryAttempt**

```csharp
public int RetryAttempt { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **RetryCount**

```csharp
public int RetryCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **IncrementalRetryContext(IncrementalRetryPolicy, TContext, Exception, Int32, TimeSpan, TimeSpan, CancellationToken)**

```csharp
public IncrementalRetryContext(IncrementalRetryPolicy policy, TContext context, Exception exception, int retryCount, TimeSpan delay, TimeSpan delayIncrement, CancellationToken cancellationToken)
```

#### Parameters

`policy` [IncrementalRetryPolicy](../masstransit-retrypolicies/incrementalretrypolicy)<br/>

`context` TContext<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`retryCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`delayIncrement` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
