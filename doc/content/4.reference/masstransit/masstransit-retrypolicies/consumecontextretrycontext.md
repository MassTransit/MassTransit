---

title: ConsumeContextRetryContext

---

# ConsumeContextRetryContext

Namespace: MassTransit.RetryPolicies

```csharp
public class ConsumeContextRetryContext : RetryContext<ConsumeContext>, RetryContext
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumeContextRetryContext](../masstransit-retrypolicies/consumecontextretrycontext)<br/>
Implements [RetryContext\<ConsumeContext\>](../../masstransit-abstractions/masstransit/retrycontext-1), [RetryContext](../../masstransit-abstractions/masstransit/retrycontext)

## Properties

### **CancellationToken**

```csharp
public CancellationToken CancellationToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **Context**

```csharp
public ConsumeContext Context { get; }
```

#### Property Value

[ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

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

### **ConsumeContextRetryContext(RetryContext\<ConsumeContext\>, RetryConsumeContext)**

```csharp
public ConsumeContextRetryContext(RetryContext<ConsumeContext> retryContext, RetryConsumeContext context)
```

#### Parameters

`retryContext` [RetryContext\<ConsumeContext\>](../../masstransit-abstractions/masstransit/retrycontext-1)<br/>

`context` [RetryConsumeContext](../masstransit-retrypolicies/retryconsumecontext)<br/>

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

### **CanRetry(Exception, RetryContext\<ConsumeContext\>)**

```csharp
public bool CanRetry(Exception exception, out RetryContext<ConsumeContext> retryContext)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`retryContext` [RetryContext\<ConsumeContext\>](../../masstransit-abstractions/masstransit/retrycontext-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
