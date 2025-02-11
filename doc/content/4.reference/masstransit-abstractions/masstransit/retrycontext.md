---

title: RetryContext

---

# RetryContext

Namespace: MassTransit

The base context of a retry

```csharp
public interface RetryContext
```

## Properties

### **CancellationToken**

Canceled when the retry should be canceled (not the same as if the underlying context
 is canceled, which is different). This can be used to cancel retry, but not the operation
 itself.

```csharp
public abstract CancellationToken CancellationToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **Exception**

The exception that originally caused the retry to be initiated

```csharp
public abstract Exception Exception { get; }
```

#### Property Value

[Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

### **RetryAttempt**

The retry attempt currently being attempted (should be 1 &gt; than RetryCount)

```csharp
public abstract int RetryAttempt { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **RetryCount**

The number of retries which were attempted beyond the initial attempt

```csharp
public abstract int RetryCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Delay**

The time to wait before the next retry attempt

```csharp
public abstract Nullable<TimeSpan> Delay { get; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ContextType**

The context type of the retry context

```csharp
public abstract Type ContextType { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

## Methods

### **RetryFaulted(Exception)**

Called after the retry attempt has failed

```csharp
Task RetryFaulted(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PreRetry()**

Called before the retry attempt is performed

```csharp
Task PreRetry()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
