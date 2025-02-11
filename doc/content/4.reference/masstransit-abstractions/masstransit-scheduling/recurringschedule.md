---

title: RecurringSchedule

---

# RecurringSchedule

Namespace: MassTransit.Scheduling

```csharp
public interface RecurringSchedule
```

## Properties

### **TimeZoneId**

The timezone of the schedule

```csharp
public abstract string TimeZoneId { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **StartTime**

The time the recurring schedule is enabled

```csharp
public abstract DateTimeOffset StartTime { get; }
```

#### Property Value

[DateTimeOffset](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset)<br/>

### **EndTime**

The time the recurring schedule is disabled
 If null then the job is repeated forever

```csharp
public abstract Nullable<DateTimeOffset> EndTime { get; }
```

#### Property Value

[Nullable\<DateTimeOffset\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ScheduleId**

A unique name that identifies this schedule.

```csharp
public abstract string ScheduleId { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ScheduleGroup**

A

```csharp
public abstract string ScheduleGroup { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **CronExpression**

The Cron Schedule Expression in Cron Syntax

```csharp
public abstract string CronExpression { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Description**

Schedule description

```csharp
public abstract string Description { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **MisfirePolicy**

```csharp
public abstract MissedEventPolicy MisfirePolicy { get; }
```

#### Property Value

[MissedEventPolicy](../masstransit-scheduling/missedeventpolicy)<br/>
