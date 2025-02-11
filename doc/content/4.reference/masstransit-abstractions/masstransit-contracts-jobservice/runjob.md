---

title: RunJob

---

# RunJob

Namespace: MassTransit.Contracts.JobService

Run a scheduled job immediately, vs waiting for the next scheduled job time

```csharp
public interface RunJob
```

## Properties

### **JobId**

```csharp
public abstract Guid JobId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>
