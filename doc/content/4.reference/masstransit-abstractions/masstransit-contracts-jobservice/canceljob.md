---

title: CancelJob

---

# CancelJob

Namespace: MassTransit.Contracts.JobService

```csharp
public interface CancelJob
```

## Properties

### **JobId**

The job identifier

```csharp
public abstract Guid JobId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Reason**

The reason for cancelling the job

```csharp
public abstract string Reason { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
