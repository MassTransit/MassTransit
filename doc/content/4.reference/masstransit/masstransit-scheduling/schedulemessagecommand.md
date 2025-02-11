---

title: ScheduleMessageCommand

---

# ScheduleMessageCommand

Namespace: MassTransit.Scheduling

```csharp
public class ScheduleMessageCommand : ScheduleMessage
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScheduleMessageCommand](../masstransit-scheduling/schedulemessagecommand)<br/>
Implements [ScheduleMessage](../../masstransit-abstractions/masstransit-scheduling/schedulemessage)

## Properties

### **CorrelationId**

```csharp
public Guid CorrelationId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **ScheduledTime**

```csharp
public DateTime ScheduledTime { get; set; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

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

### **ScheduleMessageCommand()**

```csharp
public ScheduleMessageCommand()
```
