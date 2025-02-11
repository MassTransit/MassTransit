---

title: ConsumeContextRetryContext<TFilter, TContext>

---

# ConsumeContextRetryContext\<TFilter, TContext\>

Namespace: MassTransit.RetryPolicies

```csharp
public class ConsumeContextRetryContext<TFilter, TContext> : RetryContext<TFilter>, RetryContext
```

#### Type Parameters

`TFilter`<br/>

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumeContextRetryContext\<TFilter, TContext\>](../masstransit-retrypolicies/consumecontextretrycontext-2)<br/>
Implements [RetryContext\<TFilter\>](../../masstransit-abstractions/masstransit/retrycontext-1), [RetryContext](../../masstransit-abstractions/masstransit/retrycontext)

## Properties

### **CancellationToken**

```csharp
public CancellationToken CancellationToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **Context**

```csharp
public TFilter Context { get; }
```

#### Property Value

TFilter<br/>

### **Exception**

```csharp
public Exception Exception { get; }
```

#### Property Value

[Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

### **RetryCount**

```csharp
public int RetryCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **RetryAttempt**

```csharp
public int RetryAttempt { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ContextType**

```csharp
public Type ContextType { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **Delay**

```csharp
public Nullable<TimeSpan> Delay { get; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **ConsumeContextRetryContext(RetryContext\<TFilter\>, TContext)**

```csharp
public ConsumeContextRetryContext(RetryContext<TFilter> retryContext, TContext context)
```

#### Parameters

`retryContext` [RetryContext\<TFilter\>](../../masstransit-abstractions/masstransit/retrycontext-1)<br/>

`context` TContext<br/>

## Methods

### **PreRetry()**

```csharp
public Task PreRetry()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **RetryFaulted(Exception)**

```csharp
public Task RetryFaulted(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **CanRetry(Exception, RetryContext\<TFilter\>)**

```csharp
public bool CanRetry(Exception exception, out RetryContext<TFilter> retryContext)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`retryContext` [RetryContext\<TFilter\>](../../masstransit-abstractions/masstransit/retrycontext-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
