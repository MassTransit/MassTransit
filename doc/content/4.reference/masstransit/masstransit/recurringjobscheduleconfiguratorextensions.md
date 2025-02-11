---

title: RecurringJobScheduleConfiguratorExtensions

---

# RecurringJobScheduleConfiguratorExtensions

Namespace: MassTransit

```csharp
public static class RecurringJobScheduleConfiguratorExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RecurringJobScheduleConfiguratorExtensions](../masstransit/recurringjobscheduleconfiguratorextensions)

## Methods

### **DailyAt(IRecurringJobScheduleConfigurator, Int32, Int32, Int32)**

Sets the cron expression to run daily at the specified hour (and optionally, minute and second)

```csharp
public static IRecurringJobScheduleConfigurator DailyAt(IRecurringJobScheduleConfigurator configurator, int hour, int minute, int second)
```

#### Parameters

`configurator` [IRecurringJobScheduleConfigurator](../masstransit/irecurringjobscheduleconfigurator)<br/>

`hour` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`minute` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`second` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[IRecurringJobScheduleConfigurator](../masstransit/irecurringjobscheduleconfigurator)<br/>

### **At(IRecurringJobScheduleConfigurator, Int32, Int32, DayOfWeek[])**

Sets the cron expression to run on the specified days of the week at the specified hour and minute

```csharp
public static IRecurringJobScheduleConfigurator At(IRecurringJobScheduleConfigurator configurator, int hour, int minute, DayOfWeek[] days)
```

#### Parameters

`configurator` [IRecurringJobScheduleConfigurator](../masstransit/irecurringjobscheduleconfigurator)<br/>

`hour` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`minute` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`days` [DayOfWeek[]](https://learn.microsoft.com/en-us/dotnet/api/system.dayofweek)<br/>

#### Returns

[IRecurringJobScheduleConfigurator](../masstransit/irecurringjobscheduleconfigurator)<br/>

### **Every(IRecurringJobScheduleConfigurator, Nullable\<Int32\>, Nullable\<Int32\>, Nullable\<Int32\>, Nullable\<Int32\>, Nullable\<Int32\>, Nullable\<Int32\>, DayOfWeek[])**

Sets the cron expression to run on the specified days of the week at the specified hour and minute

```csharp
public static IRecurringJobScheduleConfigurator Every(IRecurringJobScheduleConfigurator configurator, Nullable<int> hours, Nullable<int> minutes, Nullable<int> seconds, Nullable<int> hour, Nullable<int> minute, Nullable<int> second, DayOfWeek[] days)
```

#### Parameters

`configurator` [IRecurringJobScheduleConfigurator](../masstransit/irecurringjobscheduleconfigurator)<br/>

`hours` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
If specified, job will run every  hours at :

`minutes` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`seconds` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`hour` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`minute` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`second` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`days` [DayOfWeek[]](https://learn.microsoft.com/en-us/dotnet/api/system.dayofweek)<br/>

#### Returns

[IRecurringJobScheduleConfigurator](../masstransit/irecurringjobscheduleconfigurator)<br/>

### **Weekly(IRecurringJobScheduleConfigurator, DayOfWeek, Int32, Int32)**

Sets the cron expression to run weekly on the specified day at the specified hour and minute

```csharp
public static IRecurringJobScheduleConfigurator Weekly(IRecurringJobScheduleConfigurator configurator, DayOfWeek dayOfWeek, int hour, int minute)
```

#### Parameters

`configurator` [IRecurringJobScheduleConfigurator](../masstransit/irecurringjobscheduleconfigurator)<br/>

`dayOfWeek` [DayOfWeek](https://learn.microsoft.com/en-us/dotnet/api/system.dayofweek)<br/>
The day of the week to run

`hour` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`minute` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[IRecurringJobScheduleConfigurator](../masstransit/irecurringjobscheduleconfigurator)<br/>

### **Monthly(IRecurringJobScheduleConfigurator, Int32, Int32, Int32)**

Sets the cron expression to run monthly on the specified day of the month at the specified hour and minute

```csharp
public static IRecurringJobScheduleConfigurator Monthly(IRecurringJobScheduleConfigurator configurator, int dayOfMonth, int hour, int minute)
```

#### Parameters

`configurator` [IRecurringJobScheduleConfigurator](../masstransit/irecurringjobscheduleconfigurator)<br/>

`dayOfMonth` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The day of the month to run

`hour` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`minute` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[IRecurringJobScheduleConfigurator](../masstransit/irecurringjobscheduleconfigurator)<br/>

### **Yearly(IRecurringJobScheduleConfigurator, Int32, Int32, Int32, Int32)**

Sets the cron expression to run annually on the specified day of the month of the specified month at the specified hour and minute

```csharp
public static IRecurringJobScheduleConfigurator Yearly(IRecurringJobScheduleConfigurator configurator, int month, int dayOfMonth, int hour, int minute)
```

#### Parameters

`configurator` [IRecurringJobScheduleConfigurator](../masstransit/irecurringjobscheduleconfigurator)<br/>

`month` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The month of the year to run

`dayOfMonth` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The day of the month to run

`hour` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`minute` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[IRecurringJobScheduleConfigurator](../masstransit/irecurringjobscheduleconfigurator)<br/>

### **SetTimeZone(IRecurringJobScheduleConfigurator, TimeZoneInfo)**

Specify the time zone for the cron expression evaluation

```csharp
public static IRecurringJobScheduleConfigurator SetTimeZone(IRecurringJobScheduleConfigurator configurator, TimeZoneInfo tz)
```

#### Parameters

`configurator` [IRecurringJobScheduleConfigurator](../masstransit/irecurringjobscheduleconfigurator)<br/>

`tz` [TimeZoneInfo](https://learn.microsoft.com/en-us/dotnet/api/system.timezoneinfo)<br/>

#### Returns

[IRecurringJobScheduleConfigurator](../masstransit/irecurringjobscheduleconfigurator)<br/>
