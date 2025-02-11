---

title: ScheduleRecurringMessage

---

# ScheduleRecurringMessage

Namespace: MassTransit.Scheduling

```csharp
public interface ScheduleRecurringMessage
```

## Properties

### **CorrelationId**

```csharp
public abstract Guid CorrelationId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Schedule**

```csharp
public abstract RecurringSchedule Schedule { get; }
```

#### Property Value

[RecurringSchedule](../masstransit-scheduling/recurringschedule)<br/>

### **PayloadType**

The message types implemented by the message

```csharp
public abstract String[] PayloadType { get; }
```

#### Property Value

[String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Destination**

The destination where the message should be sent

```csharp
public abstract Uri Destination { get; }
```

#### Property Value

Uri<br/>

### **Payload**

The actual scheduled message payload

```csharp
public abstract object Payload { get; }
```

#### Property Value

[Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
