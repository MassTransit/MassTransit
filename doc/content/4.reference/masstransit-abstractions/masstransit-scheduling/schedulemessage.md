---

title: ScheduleMessage

---

# ScheduleMessage

Namespace: MassTransit.Scheduling

```csharp
public interface ScheduleMessage
```

## Properties

### **CorrelationId**

```csharp
public abstract Guid CorrelationId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **ScheduledTime**

The time at which the message should be published, should be in UTC

```csharp
public abstract DateTime ScheduledTime { get; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

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

The actual message payload to deliver

```csharp
public abstract object Payload { get; }
```

#### Property Value

[Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
