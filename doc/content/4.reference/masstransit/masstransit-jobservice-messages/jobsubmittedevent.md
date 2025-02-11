---

title: JobSubmittedEvent

---

# JobSubmittedEvent

Namespace: MassTransit.JobService.Messages

```csharp
public class JobSubmittedEvent : JobSubmitted
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobSubmittedEvent](../masstransit-jobservice-messages/jobsubmittedevent)<br/>
Implements [JobSubmitted](../../masstransit-abstractions/masstransit-contracts-jobservice/jobsubmitted)

## Properties

### **JobId**

```csharp
public Guid JobId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **JobTypeId**

```csharp
public Guid JobTypeId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Timestamp**

```csharp
public DateTime Timestamp { get; set; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **JobTimeout**

```csharp
public TimeSpan JobTimeout { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **Job**

```csharp
public Dictionary<string, object> Job { get; set; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

### **JobProperties**

```csharp
public Dictionary<string, object> JobProperties { get; set; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

### **Schedule**

```csharp
public RecurringJobSchedule Schedule { get; set; }
```

#### Property Value

[RecurringJobSchedule](../../masstransit-abstractions/masstransit-contracts-jobservice/recurringjobschedule)<br/>

## Constructors

### **JobSubmittedEvent()**

```csharp
public JobSubmittedEvent()
```
