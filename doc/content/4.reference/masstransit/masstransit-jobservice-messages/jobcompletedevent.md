---

title: JobCompletedEvent

---

# JobCompletedEvent

Namespace: MassTransit.JobService.Messages

```csharp
public class JobCompletedEvent : JobCompleted
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobCompletedEvent](../masstransit-jobservice-messages/jobcompletedevent)<br/>
Implements [JobCompleted](../../masstransit-abstractions/masstransit-contracts-jobservice/jobcompleted)

## Properties

### **JobId**

```csharp
public Guid JobId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Timestamp**

```csharp
public DateTime Timestamp { get; set; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **Duration**

```csharp
public TimeSpan Duration { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **Job**

```csharp
public Dictionary<string, object> Job { get; set; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

### **Result**

```csharp
public Dictionary<string, object> Result { get; set; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

## Constructors

### **JobCompletedEvent()**

```csharp
public JobCompletedEvent()
```
