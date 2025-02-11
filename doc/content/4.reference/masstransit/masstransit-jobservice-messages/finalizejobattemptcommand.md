---

title: FinalizeJobAttemptCommand

---

# FinalizeJobAttemptCommand

Namespace: MassTransit.JobService.Messages

```csharp
public class FinalizeJobAttemptCommand : FinalizeJobAttempt
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FinalizeJobAttemptCommand](../masstransit-jobservice-messages/finalizejobattemptcommand)<br/>
Implements [FinalizeJobAttempt](../../masstransit-abstractions/masstransit-contracts-jobservice/finalizejobattempt)

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

## Constructors

### **FinalizeJobAttemptCommand()**

```csharp
public FinalizeJobAttemptCommand()
```
