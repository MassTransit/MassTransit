---

title: JobAttemptStartedEvent

---

# JobAttemptStartedEvent

Namespace: MassTransit.JobService.Messages

```csharp
public class JobAttemptStartedEvent : JobAttemptStarted
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobAttemptStartedEvent](../masstransit-jobservice-messages/jobattemptstartedevent)<br/>
Implements [JobAttemptStarted](../../masstransit-abstractions/masstransit-contracts-jobservice/jobattemptstarted)

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

### **InstanceAddress**

```csharp
public Uri InstanceAddress { get; set; }
```

#### Property Value

Uri<br/>

## Constructors

### **JobAttemptStartedEvent()**

```csharp
public JobAttemptStartedEvent()
```
