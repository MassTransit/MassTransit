---

title: NoRetryContext<TContext>

---

# NoRetryContext\<TContext\>

Namespace: MassTransit.RetryPolicies

```csharp
public class NoRetryContext<TContext> : BaseRetryContext<TContext>, RetryContext, RetryContext<TContext>
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BaseRetryContext\<TContext\>](../masstransit-retrypolicies/baseretrycontext-1) → [NoRetryContext\<TContext\>](../masstransit-retrypolicies/noretrycontext-1)<br/>
Implements [RetryContext](../../masstransit-abstractions/masstransit/retrycontext), [RetryContext\<TContext\>](../../masstransit-abstractions/masstransit/retrycontext-1)

## Properties

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

### **Delay**

```csharp
public Nullable<TimeSpan> Delay { get; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **NoRetryContext(TContext, Exception, CancellationToken)**

```csharp
public NoRetryContext(TContext context, Exception exception, CancellationToken cancellationToken)
```

#### Parameters

`context` TContext<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>
