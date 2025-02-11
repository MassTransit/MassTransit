---

title: JobAttemptStatus

---

# JobAttemptStatus

Namespace: MassTransit.Contracts.JobService

```csharp
public interface JobAttemptStatus
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

### **Timestamp**

```csharp
public abstract DateTime Timestamp { get; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **Status**

```csharp
public abstract JobStatus Status { get; }
```

#### Property Value

[JobStatus](../masstransit-contracts-jobservice/jobstatus)<br/>
