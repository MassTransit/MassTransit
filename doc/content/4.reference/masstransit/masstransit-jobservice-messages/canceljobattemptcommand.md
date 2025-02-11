---

title: CancelJobAttemptCommand

---

# CancelJobAttemptCommand

Namespace: MassTransit.JobService.Messages

```csharp
public class CancelJobAttemptCommand : CancelJobAttempt
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CancelJobAttemptCommand](../masstransit-jobservice-messages/canceljobattemptcommand)<br/>
Implements [CancelJobAttempt](../../masstransit-abstractions/masstransit-contracts-jobservice/canceljobattempt)

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

### **Reason**

```csharp
public string Reason { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **CancelJobAttemptCommand()**

```csharp
public CancelJobAttemptCommand()
```
