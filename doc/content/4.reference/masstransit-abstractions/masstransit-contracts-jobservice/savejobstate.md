---

title: SaveJobState

---

# SaveJobState

Namespace: MassTransit.Contracts.JobService

```csharp
public interface SaveJobState
```

## Properties

### **JobId**

```csharp
public abstract Guid JobId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **AttemptId**

```csharp
public abstract Guid AttemptId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **JobState**

The state of the job, as a dictionary, or null to clear the state

```csharp
public abstract Dictionary<string, object> JobState { get; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>
