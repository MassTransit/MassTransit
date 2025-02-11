---

title: ResumeScheduledRecurringMessageCommand

---

# ResumeScheduledRecurringMessageCommand

Namespace: MassTransit.Scheduling

```csharp
public class ResumeScheduledRecurringMessageCommand : ResumeScheduledRecurringMessage
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ResumeScheduledRecurringMessageCommand](../masstransit-scheduling/resumescheduledrecurringmessagecommand)<br/>
Implements [ResumeScheduledRecurringMessage](../../masstransit-abstractions/masstransit-scheduling/resumescheduledrecurringmessage)

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

### **ResumeScheduledRecurringMessageCommand()**

```csharp
public ResumeScheduledRecurringMessageCommand()
```

### **ResumeScheduledRecurringMessageCommand(String, String)**

```csharp
public ResumeScheduledRecurringMessageCommand(string scheduleId, string scheduleGroup)
```

#### Parameters

`scheduleId` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`scheduleGroup` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
