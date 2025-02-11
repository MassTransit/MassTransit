---

title: RetryJobCommand

---

# RetryJobCommand

Namespace: MassTransit.JobService.Messages

```csharp
public class RetryJobCommand : RetryJob
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RetryJobCommand](../masstransit-jobservice-messages/retryjobcommand)<br/>
Implements [RetryJob](../../masstransit-abstractions/masstransit-contracts-jobservice/retryjob)

## Properties

### **JobId**

```csharp
public Guid JobId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

## Constructors

### **RetryJobCommand()**

```csharp
public RetryJobCommand()
```
