---

title: ConsumeRetryContext

---

# ConsumeRetryContext

Namespace: MassTransit

```csharp
public interface ConsumeRetryContext
```

## Properties

### **RetryAttempt**

The retry attempt in progress, or zero if this is the first time through

```csharp
public abstract int RetryAttempt { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **RetryCount**

The number of retries that have already been attempted, note that this is zero
 on the first retry attempt

```csharp
public abstract int RetryCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Methods

### **CreateNext\<TContext\>(RetryContext)**

```csharp
TContext CreateNext<TContext>(RetryContext retryContext)
```

#### Type Parameters

`TContext`<br/>

#### Parameters

`retryContext` [RetryContext](../masstransit/retrycontext)<br/>

#### Returns

TContext<br/>

### **NotifyPendingFaults()**

```csharp
Task NotifyPendingFaults()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
