---

title: GetJobAttemptStatusRequest

---

# GetJobAttemptStatusRequest

Namespace: MassTransit.JobService.Messages

```csharp
public class GetJobAttemptStatusRequest : GetJobAttemptStatus
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [GetJobAttemptStatusRequest](../masstransit-jobservice-messages/getjobattemptstatusrequest)<br/>
Implements [GetJobAttemptStatus](../../masstransit-abstractions/masstransit-contracts-jobservice/getjobattemptstatus)

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

## Constructors

### **GetJobAttemptStatusRequest()**

```csharp
public GetJobAttemptStatusRequest()
```
