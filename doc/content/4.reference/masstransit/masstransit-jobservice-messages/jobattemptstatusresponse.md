---

title: JobAttemptStatusResponse

---

# JobAttemptStatusResponse

Namespace: MassTransit.JobService.Messages

```csharp
public class JobAttemptStatusResponse : JobAttemptStatus
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobAttemptStatusResponse](../masstransit-jobservice-messages/jobattemptstatusresponse)<br/>
Implements [JobAttemptStatus](../../masstransit-abstractions/masstransit-contracts-jobservice/jobattemptstatus)

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

### **Timestamp**

```csharp
public DateTime Timestamp { get; set; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **Status**

```csharp
public JobStatus Status { get; set; }
```

#### Property Value

[JobStatus](../../masstransit-abstractions/masstransit-contracts-jobservice/jobstatus)<br/>

## Constructors

### **JobAttemptStatusResponse()**

```csharp
public JobAttemptStatusResponse()
```
