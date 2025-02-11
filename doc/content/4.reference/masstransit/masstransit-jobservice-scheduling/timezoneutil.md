---

title: TimeZoneUtil

---

# TimeZoneUtil

Namespace: MassTransit.JobService.Scheduling

```csharp
public static class TimeZoneUtil
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TimeZoneUtil](../masstransit-jobservice-scheduling/timezoneutil)

## Fields

### **CustomResolver**

```csharp
public static Func<string, TimeZoneInfo> CustomResolver;
```

## Methods

### **ConvertTime(DateTimeOffset, TimeZoneInfo)**

TimeZoneInfo.ConvertTime is not supported under mono

```csharp
public static DateTimeOffset ConvertTime(DateTimeOffset dateTimeOffset, TimeZoneInfo timeZoneInfo)
```

#### Parameters

`dateTimeOffset` [DateTimeOffset](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset)<br/>

`timeZoneInfo` [TimeZoneInfo](https://learn.microsoft.com/en-us/dotnet/api/system.timezoneinfo)<br/>

#### Returns

[DateTimeOffset](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset)<br/>

### **GetUtcOffset(DateTimeOffset, TimeZoneInfo)**

TimeZoneInfo.GetUtcOffset(DateTimeOffset) is not supported under mono

```csharp
public static TimeSpan GetUtcOffset(DateTimeOffset dateTimeOffset, TimeZoneInfo timeZoneInfo)
```

#### Parameters

`dateTimeOffset` [DateTimeOffset](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset)<br/>

`timeZoneInfo` [TimeZoneInfo](https://learn.microsoft.com/en-us/dotnet/api/system.timezoneinfo)<br/>

#### Returns

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **GetUtcOffset(DateTime, TimeZoneInfo)**

```csharp
public static TimeSpan GetUtcOffset(DateTime dateTime, TimeZoneInfo timeZoneInfo)
```

#### Parameters

`dateTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`timeZoneInfo` [TimeZoneInfo](https://learn.microsoft.com/en-us/dotnet/api/system.timezoneinfo)<br/>

#### Returns

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **FindTimeZoneById(String)**

Tries to find time zone with given id, has ability do some fallbacks when necessary.

```csharp
public static TimeZoneInfo FindTimeZoneById(string id)
```

#### Parameters

`id` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
System id of the time zone.

#### Returns

[TimeZoneInfo](https://learn.microsoft.com/en-us/dotnet/api/system.timezoneinfo)<br/>
