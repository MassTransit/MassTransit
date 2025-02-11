---

title: FinalizeJobAttempt

---

# FinalizeJobAttempt

Namespace: MassTransit.Contracts.JobService

```csharp
public interface FinalizeJobAttempt
```

## Properties

### **JobId**

The job identifier

```csharp
public abstract Guid JobId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **AttemptId**

Identifies this attempt to run the job

```csharp
public abstract Guid AttemptId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>
