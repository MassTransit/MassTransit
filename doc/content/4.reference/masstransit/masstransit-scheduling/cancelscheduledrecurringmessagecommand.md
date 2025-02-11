---

title: CancelScheduledRecurringMessageCommand

---

# CancelScheduledRecurringMessageCommand

Namespace: MassTransit.Scheduling

```csharp
public class CancelScheduledRecurringMessageCommand : CancelScheduledRecurringMessage
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CancelScheduledRecurringMessageCommand](../masstransit-scheduling/cancelscheduledrecurringmessagecommand)<br/>
Implements [CancelScheduledRecurringMessage](../../masstransit-abstractions/masstransit-scheduling/cancelscheduledrecurringmessage)

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

### **ScheduleId**

```csharp
public string ScheduleId { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ScheduleGroup**

```csharp
public string ScheduleGroup { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **CancelScheduledRecurringMessageCommand()**

```csharp
public CancelScheduledRecurringMessageCommand()
```

### **CancelScheduledRecurringMessageCommand(String, String)**

```csharp
public CancelScheduledRecurringMessageCommand(string scheduleId, string scheduleGroup)
```

#### Parameters

`scheduleId` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`scheduleGroup` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
