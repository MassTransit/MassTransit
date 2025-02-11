---

title: DefaultRecurringSchedule

---

# DefaultRecurringSchedule

Namespace: MassTransit.Scheduling

```csharp
public abstract class DefaultRecurringSchedule : RecurringSchedule
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DefaultRecurringSchedule](../masstransit-scheduling/defaultrecurringschedule)<br/>
Implements [RecurringSchedule](../../masstransit-abstractions/masstransit-scheduling/recurringschedule)

## Properties

### **MisfirePolicy**

```csharp
public MissedEventPolicy MisfirePolicy { get; protected set; }
```

#### Property Value

[MissedEventPolicy](../../masstransit-abstractions/masstransit-scheduling/missedeventpolicy)<br/>

### **TimeZoneId**

```csharp
public string TimeZoneId { get; protected set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **StartTime**

```csharp
public DateTimeOffset StartTime { get; protected set; }
```

#### Property Value

[DateTimeOffset](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset)<br/>

### **EndTime**

```csharp
public Nullable<DateTimeOffset> EndTime { get; protected set; }
```

#### Property Value

[Nullable\<DateTimeOffset\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ScheduleId**

```csharp
public string ScheduleId { get; private set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ScheduleGroup**

```csharp
public string ScheduleGroup { get; private set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **CronExpression**

```csharp
public string CronExpression { get; protected set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Description**

```csharp
public string Description { get; protected set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
