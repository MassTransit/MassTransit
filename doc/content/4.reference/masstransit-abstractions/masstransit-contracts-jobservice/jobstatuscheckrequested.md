---

title: JobStatusCheckRequested

---

# JobStatusCheckRequested

Namespace: MassTransit.Contracts.JobService

Signals that the time to supervise a job has expired, and the instance should be checked

```csharp
public interface JobStatusCheckRequested
```

## Properties

### **AttemptId**

Identifies this attempt to run the job

```csharp
public abstract Guid AttemptId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>
