---

title: CronExpression

---

# CronExpression

Namespace: MassTransit.JobService.Scheduling

```csharp
public sealed class CronExpression : IEquatable<CronExpression>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CronExpression](../masstransit-jobservice-scheduling/cronexpression)<br/>
Implements [IEquatable\<CronExpression\>](https://learn.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### **TimeZone**

```csharp
public TimeZoneInfo TimeZone { get; set; }
```

#### Property Value

[TimeZoneInfo](https://learn.microsoft.com/en-us/dotnet/api/system.timezoneinfo)<br/>

## Constructors

### **CronExpression(String)**

```csharp
public CronExpression(string cronExpression)
```

#### Parameters

`cronExpression` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **Equals(CronExpression)**

```csharp
public bool Equals(CronExpression other)
```

#### Parameters

`other` [CronExpression](../masstransit-jobservice-scheduling/cronexpression)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Equals(Object)**

```csharp
public bool Equals(object obj)
```

#### Parameters

`obj` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetHashCode()**

```csharp
public int GetHashCode()
```

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **IsSatisfiedBy(DateTimeOffset)**

```csharp
public bool IsSatisfiedBy(DateTimeOffset date)
```

#### Parameters

`date` [DateTimeOffset](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetNextValidTimeAfter(DateTimeOffset)**

```csharp
public Nullable<DateTimeOffset> GetNextValidTimeAfter(DateTimeOffset date)
```

#### Parameters

`date` [DateTimeOffset](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset)<br/>

#### Returns

[Nullable\<DateTimeOffset\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **IsValidExpression(String)**

```csharp
public static bool IsValidExpression(string cronExpression)
```

#### Parameters

`cronExpression` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **ValidateExpression(String)**

```csharp
public static void ValidateExpression(string cronExpression)
```

#### Parameters

`cronExpression` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **GetExpressionSummary()**

```csharp
public string GetExpressionSummary()
```

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **GetSet(Int32)**

```csharp
public CronField GetSet(int type)
```

#### Parameters

`type` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[CronField](../masstransit-jobservice-scheduling/cronfield)<br/>

### **GetTimeAfter(DateTimeOffset)**

```csharp
public Nullable<DateTimeOffset> GetTimeAfter(DateTimeOffset afterTime)
```

#### Parameters

`afterTime` [DateTimeOffset](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset)<br/>

#### Returns

[Nullable\<DateTimeOffset\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
