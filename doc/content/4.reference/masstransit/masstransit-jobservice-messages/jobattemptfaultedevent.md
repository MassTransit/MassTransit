---

title: JobAttemptFaultedEvent

---

# JobAttemptFaultedEvent

Namespace: MassTransit.JobService.Messages

```csharp
public class JobAttemptFaultedEvent : JobAttemptFaulted
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobAttemptFaultedEvent](../masstransit-jobservice-messages/jobattemptfaultedevent)<br/>
Implements [JobAttemptFaulted](../../masstransit-abstractions/masstransit-contracts-jobservice/jobattemptfaulted)

## Properties

### **JobId**

```csharp
public Guid JobId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **AttemptId**

```csharp
public Guid AttemptId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **RetryAttempt**

```csharp
public int RetryAttempt { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **RetryDelay**

```csharp
public Nullable<TimeSpan> RetryDelay { get; set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Timestamp**

```csharp
public DateTime Timestamp { get; set; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **Exceptions**

```csharp
public ExceptionInfo Exceptions { get; set; }
```

#### Property Value

[ExceptionInfo](../../masstransit-abstractions/masstransit/exceptioninfo)<br/>

## Constructors

### **JobAttemptFaultedEvent()**

```csharp
public JobAttemptFaultedEvent()
```
