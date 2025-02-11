---

title: JobAttemptFaulted

---

# JobAttemptFaulted

Namespace: MassTransit.Contracts.JobService

```csharp
public interface JobAttemptFaulted
```

## Properties

### **JobId**

```csharp
public abstract Guid JobId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **AttemptId**

```csharp
public abstract Guid AttemptId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **RetryAttempt**

The retry attempt that faulted. Zero for the first attempt.

```csharp
public abstract int RetryAttempt { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **RetryDelay**

If present, the delay until the next retry

```csharp
public abstract Nullable<TimeSpan> RetryDelay { get; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Timestamp**

```csharp
public abstract DateTime Timestamp { get; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **Exceptions**

```csharp
public abstract ExceptionInfo Exceptions { get; }
```

#### Property Value

[ExceptionInfo](../masstransit/exceptioninfo)<br/>
