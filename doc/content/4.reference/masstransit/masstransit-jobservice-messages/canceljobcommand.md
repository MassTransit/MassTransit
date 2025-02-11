---

title: CancelJobCommand

---

# CancelJobCommand

Namespace: MassTransit.JobService.Messages

```csharp
public class CancelJobCommand : CancelJob
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CancelJobCommand](../masstransit-jobservice-messages/canceljobcommand)<br/>
Implements [CancelJob](../../masstransit-abstractions/masstransit-contracts-jobservice/canceljob)

## Properties

### **JobId**

```csharp
public Guid JobId { get; set; }
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

### **CancelJobCommand()**

```csharp
public CancelJobCommand()
```
