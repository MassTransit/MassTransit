---

title: JobAttemptCompletedEvent

---

# JobAttemptCompletedEvent

Namespace: MassTransit.JobService.Messages

```csharp
public class JobAttemptCompletedEvent : JobAttemptCompleted
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobAttemptCompletedEvent](../masstransit-jobservice-messages/jobattemptcompletedevent)<br/>
Implements [JobAttemptCompleted](../../masstransit-abstractions/masstransit-contracts-jobservice/jobattemptcompleted)

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

### **Timestamp**

```csharp
public DateTime Timestamp { get; set; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **Duration**

```csharp
public TimeSpan Duration { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

## Constructors

### **JobAttemptCompletedEvent()**

```csharp
public JobAttemptCompletedEvent()
```
