---

title: ScheduledRecurringMessageHandle<T>

---

# ScheduledRecurringMessageHandle\<T\>

Namespace: MassTransit.Scheduling

```csharp
public class ScheduledRecurringMessageHandle<T> : ScheduledRecurringMessage<T>, ScheduledRecurringMessage
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScheduledRecurringMessageHandle\<T\>](../masstransit-scheduling/scheduledrecurringmessagehandle-1)<br/>
Implements [ScheduledRecurringMessage\<T\>](../../masstransit-abstractions/masstransit/scheduledrecurringmessage-1), [ScheduledRecurringMessage](../../masstransit-abstractions/masstransit/scheduledrecurringmessage)

## Properties

### **Schedule**

```csharp
public RecurringSchedule Schedule { get; private set; }
```

#### Property Value

[RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>

### **Destination**

```csharp
public Uri Destination { get; private set; }
```

#### Property Value

Uri<br/>

### **Payload**

```csharp
public T Payload { get; private set; }
```

#### Property Value

T<br/>

## Constructors

### **ScheduledRecurringMessageHandle(RecurringSchedule, Uri, T)**

```csharp
public ScheduledRecurringMessageHandle(RecurringSchedule schedule, Uri destination, T payload)
```

#### Parameters

`schedule` [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)<br/>

`destination` Uri<br/>

`payload` T<br/>
