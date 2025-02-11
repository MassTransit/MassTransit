---

title: JobStatusCheckRequestedEvent

---

# JobStatusCheckRequestedEvent

Namespace: MassTransit.JobService.Messages

```csharp
public class JobStatusCheckRequestedEvent : JobStatusCheckRequested
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobStatusCheckRequestedEvent](../masstransit-jobservice-messages/jobstatuscheckrequestedevent)<br/>
Implements [JobStatusCheckRequested](../../masstransit-abstractions/masstransit-contracts-jobservice/jobstatuscheckrequested)

## Properties

### **AttemptId**

```csharp
public Guid AttemptId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

## Constructors

### **JobStatusCheckRequestedEvent()**

```csharp
public JobStatusCheckRequestedEvent()
```
