---

title: RecurringJobScheduleInfo

---

# RecurringJobScheduleInfo

Namespace: MassTransit.JobService.Messages

```csharp
public class RecurringJobScheduleInfo : RecurringJobSchedule, IRecurringJobScheduleConfigurator, ISpecification
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RecurringJobScheduleInfo](../masstransit-jobservice-messages/recurringjobscheduleinfo)<br/>
Implements [RecurringJobSchedule](../../masstransit-abstractions/masstransit-contracts-jobservice/recurringjobschedule), [IRecurringJobScheduleConfigurator](../masstransit/irecurringjobscheduleconfigurator), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **CronExpression**

```csharp
public string CronExpression { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **TimeZoneId**

```csharp
public string TimeZoneId { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Start**

```csharp
public Nullable<DateTimeOffset> Start { get; set; }
```

#### Property Value

[Nullable\<DateTimeOffset\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **End**

```csharp
public Nullable<DateTimeOffset> End { get; set; }
```

#### Property Value

[Nullable\<DateTimeOffset\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **RecurringJobScheduleInfo()**

```csharp
public RecurringJobScheduleInfo()
```

## Methods

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
