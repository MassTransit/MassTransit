---

title: CancelScheduledRecurringMessage

---

# CancelScheduledRecurringMessage

Namespace: MassTransit.Scheduling

```csharp
public interface CancelScheduledRecurringMessage
```

## Properties

### **CorrelationId**

The cancel scheduled message correlationId

```csharp
public abstract Guid CorrelationId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Timestamp**

The date/time this message was created

```csharp
public abstract DateTime Timestamp { get; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **ScheduleId**

```csharp
public abstract string ScheduleId { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ScheduleGroup**

```csharp
public abstract string ScheduleGroup { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
