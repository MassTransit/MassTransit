---

title: RetryContextExtensions

---

# RetryContextExtensions

Namespace: MassTransit

```csharp
public static class RetryContextExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RetryContextExtensions](../masstransit/retrycontextextensions)

## Methods

### **GetRetryAttempt(ConsumeContext)**

If within a retry attempt, the return value is greater than zero and indicates the number of the retry attempt
 in progress.

```csharp
public static int GetRetryAttempt(ConsumeContext context)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The retry attempt number, 0 = first time, &gt;= 1 = retry

### **GetRetryCount(ConsumeContext)**

If within a retry attempt, the return value indicates the number of retry attempts that have already occurred.

```csharp
public static int GetRetryCount(ConsumeContext context)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of retries that have already been attempted, 0 = first time or first retry, &gt;= 1 = subsequent retry

### **GetRedeliveryCount(ConsumeContext)**

If the message is being redelivered, returns the redelivery attempt

```csharp
public static int GetRedeliveryCount(ConsumeContext context)
```

#### Parameters

`context` [ConsumeContext](../masstransit/consumecontext)<br/>

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The retry attempt number, 0 = first time, &gt;= 1 = retry
