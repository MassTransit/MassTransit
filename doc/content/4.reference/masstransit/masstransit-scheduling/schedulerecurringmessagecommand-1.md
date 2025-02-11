---

title: ScheduleRecurringMessageCommand<T>

---

# ScheduleRecurringMessageCommand\<T\>

Namespace: MassTransit.Scheduling

```csharp
public class ScheduleRecurringMessageCommand<T> : ScheduleRecurringMessage
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScheduleRecurringMessageCommand\<T\>](../masstransit-scheduling/schedulerecurringmessagecommand-1)<br/>
Implements [ScheduleRecurringMessage](../../masstransit-abstractions/masstransit-scheduling/schedulerecurringmessage)

## Properties

### **CorrelationId**

```csharp
public Guid CorrelationId { get; private set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Schedule**

```csharp
public RecurringSchedule Schedule { get; private set; }
```

#### Property Value

[RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>

### **PayloadType**

```csharp
public String[] PayloadType { get; private set; }
```

#### Property Value

[String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Destination**

```csharp
public Uri Destination { get; private set; }
```

#### Property Value

Uri<br/>

### **Payload**

```csharp
public object Payload { get; private set; }
```

#### Property Value

[Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

## Constructors

### **ScheduleRecurringMessageCommand(RecurringSchedule, Uri, T)**

```csharp
public ScheduleRecurringMessageCommand(RecurringSchedule schedule, Uri destination, T payload)
```

#### Parameters

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>

`destination` Uri<br/>

`payload` T<br/>

## Methods

### **ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
