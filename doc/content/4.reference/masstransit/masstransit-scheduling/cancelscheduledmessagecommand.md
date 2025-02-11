---

title: CancelScheduledMessageCommand

---

# CancelScheduledMessageCommand

Namespace: MassTransit.Scheduling

```csharp
public class CancelScheduledMessageCommand : CancelScheduledMessage
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CancelScheduledMessageCommand](../masstransit-scheduling/cancelscheduledmessagecommand)<br/>
Implements [CancelScheduledMessage](../../masstransit-abstractions/masstransit-scheduling/cancelscheduledmessage)

## Properties

### **CorrelationId**

```csharp
public Guid CorrelationId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Timestamp**

```csharp
public DateTime Timestamp { get; set; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **TokenId**

```csharp
public Guid TokenId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

## Constructors

### **CancelScheduledMessageCommand()**

```csharp
public CancelScheduledMessageCommand()
```

### **CancelScheduledMessageCommand(Guid)**

```csharp
public CancelScheduledMessageCommand(Guid tokenId)
```

#### Parameters

`tokenId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>
