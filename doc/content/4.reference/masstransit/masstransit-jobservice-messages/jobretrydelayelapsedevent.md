---

title: JobRetryDelayElapsedEvent

---

# JobRetryDelayElapsedEvent

Namespace: MassTransit.JobService.Messages

```csharp
public class JobRetryDelayElapsedEvent : JobRetryDelayElapsed
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobRetryDelayElapsedEvent](../masstransit-jobservice-messages/jobretrydelayelapsedevent)<br/>
Implements [JobRetryDelayElapsed](../../masstransit-abstractions/masstransit-contracts-jobservice/jobretrydelayelapsed)

## Properties

### **JobId**

```csharp
public Guid JobId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

## Constructors

### **JobRetryDelayElapsedEvent()**

```csharp
public JobRetryDelayElapsedEvent()
```
