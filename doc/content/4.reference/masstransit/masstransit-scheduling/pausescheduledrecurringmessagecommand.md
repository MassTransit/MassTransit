---

title: PauseScheduledRecurringMessageCommand

---

# PauseScheduledRecurringMessageCommand

Namespace: MassTransit.Scheduling

```csharp
public class PauseScheduledRecurringMessageCommand : PauseScheduledRecurringMessage
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PauseScheduledRecurringMessageCommand](../masstransit-scheduling/pausescheduledrecurringmessagecommand)<br/>
Implements [PauseScheduledRecurringMessage](../../masstransit-abstractions/masstransit-scheduling/pausescheduledrecurringmessage)

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

### **PauseScheduledRecurringMessageCommand()**

```csharp
public PauseScheduledRecurringMessageCommand()
```

### **PauseScheduledRecurringMessageCommand(String, String)**

```csharp
public PauseScheduledRecurringMessageCommand(string scheduleId, string scheduleGroup)
```

#### Parameters

`scheduleId` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`scheduleGroup` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
