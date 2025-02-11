---

title: ScheduleMessageCommand<T>

---

# ScheduleMessageCommand\<T\>

Namespace: MassTransit.Scheduling

```csharp
public class ScheduleMessageCommand<T> : ScheduleMessage
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScheduleMessageCommand\<T\>](../masstransit-scheduling/schedulemessagecommand-1)<br/>
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

### **ScheduleMessageCommand(DateTime, Uri, T, Guid)**

```csharp
public ScheduleMessageCommand(DateTime scheduledTime, Uri destination, T payload, Guid tokenId)
```

#### Parameters

`scheduledTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`destination` Uri<br/>

`payload` T<br/>

`tokenId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>
