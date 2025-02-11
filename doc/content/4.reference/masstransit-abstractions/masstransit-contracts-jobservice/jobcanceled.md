---

title: JobCanceled

---

# JobCanceled

Namespace: MassTransit.Contracts.JobService

Published when a job faults

```csharp
public interface JobCanceled
```

## Properties

### **JobId**

The job identifier

```csharp
public abstract Guid JobId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Timestamp**

The time the job was cancelled

```csharp
public abstract DateTime Timestamp { get; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>
