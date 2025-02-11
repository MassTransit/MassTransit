---

title: SaveJobStateCommand

---

# SaveJobStateCommand

Namespace: MassTransit.JobService.Messages

```csharp
public class SaveJobStateCommand : SaveJobState
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SaveJobStateCommand](../masstransit-jobservice-messages/savejobstatecommand)<br/>
Implements [SaveJobState](../../masstransit-abstractions/masstransit-contracts-jobservice/savejobstate)

## Properties

### **JobId**

```csharp
public Guid JobId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **AttemptId**

```csharp
public Guid AttemptId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **JobState**

```csharp
public Dictionary<string, object> JobState { get; set; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

## Constructors

### **SaveJobStateCommand()**

```csharp
public SaveJobStateCommand()
```
