---

title: ScheduleRecurringMessageCommand

---

# ScheduleRecurringMessageCommand

Namespace: MassTransit.Scheduling

```csharp
public class ScheduleRecurringMessageCommand : ScheduleRecurringMessage
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScheduleRecurringMessageCommand](../masstransit-scheduling/schedulerecurringmessagecommand)<br/>
Implements [ScheduleRecurringMessage](../../masstransit-abstractions/masstransit-scheduling/schedulerecurringmessage)

## Properties

### **CorrelationId**

```csharp
public Guid CorrelationId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Schedule**

```csharp
public RecurringSchedule Schedule { get; set; }
```

#### Property Value

[RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>

### **PayloadType**

```csharp
public String[] PayloadType { get; set; }
```

#### Property Value

[String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Destination**

```csharp
public Uri Destination { get; set; }
```

#### Property Value

Uri<br/>

### **Payload**

```csharp
public object Payload { get; set; }
```

#### Property Value

[Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

## Constructors

### **ScheduleRecurringMessageCommand()**

```csharp
public ScheduleRecurringMessageCommand()
```
