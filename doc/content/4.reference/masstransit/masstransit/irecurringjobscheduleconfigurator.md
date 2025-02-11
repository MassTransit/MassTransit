---

title: IRecurringJobScheduleConfigurator

---

# IRecurringJobScheduleConfigurator

Namespace: MassTransit

Configure the optional settings of a recurring job

```csharp
public interface IRecurringJobScheduleConfigurator
```

## Properties

### **CronExpression**

A valid cron expression specifying the job schedule

```csharp
public abstract string CronExpression { set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Start**

If specified, the start date for the job. Otherwise, the current date/time will be used.

```csharp
public abstract Nullable<DateTimeOffset> Start { set; }
```

#### Property Value

[Nullable\<DateTimeOffset\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **End**

If specified, the end date for the job after which it will be removed from the job scheduler

```csharp
public abstract Nullable<DateTimeOffset> End { set; }
```

#### Property Value

[Nullable\<DateTimeOffset\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **TimeZoneId**

If specified, the time zone in which the cron expression should be evaluated, otherwise UTC is used.

```csharp
public abstract string TimeZoneId { set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
