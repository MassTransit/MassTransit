---

title: RecurringJobSchedule

---

# RecurringJobSchedule

Namespace: MassTransit.Contracts.JobService

```csharp
public interface RecurringJobSchedule
```

## Properties

### **CronExpression**

A valid cron expression specifying the job schedule

```csharp
public abstract string CronExpression { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **TimeZoneId**

If specified, the time zone in which the cron expression should be evaluated, otherwise UTC is used.

```csharp
public abstract string TimeZoneId { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Start**

If specified, the start date for the job. Otherwise, the current date/time will be used.

```csharp
public abstract Nullable<DateTimeOffset> Start { get; }
```

#### Property Value

[Nullable\<DateTimeOffset\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **End**

If specified, the end date for the job after which it will be removed from the job scheduler

```csharp
public abstract Nullable<DateTimeOffset> End { get; }
```

#### Property Value

[Nullable\<DateTimeOffset\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
