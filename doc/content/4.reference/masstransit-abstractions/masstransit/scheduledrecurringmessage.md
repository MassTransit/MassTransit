---

title: ScheduledRecurringMessage

---

# ScheduledRecurringMessage

Namespace: MassTransit

```csharp
public interface ScheduledRecurringMessage
```

## Properties

### **Schedule**

```csharp
public abstract RecurringSchedule Schedule { get; }
```

#### Property Value

[RecurringSchedule](../masstransit-scheduling/recurringschedule)<br/>

### **Destination**

```csharp
public abstract Uri Destination { get; }
```

#### Property Value

Uri<br/>
