---

title: CancelJobAttempt

---

# CancelJobAttempt

Namespace: MassTransit.Contracts.JobService

```csharp
public interface CancelJobAttempt
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

### **Reason**

The supplied reason for the job cancellation

```csharp
public abstract string Reason { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
