---

title: JobFaultedEvent

---

# JobFaultedEvent

Namespace: MassTransit.JobService.Messages

```csharp
public class JobFaultedEvent : JobFaulted
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobFaultedEvent](../masstransit-jobservice-messages/jobfaultedevent)<br/>
Implements [JobFaulted](../../masstransit-abstractions/masstransit-contracts-jobservice/jobfaulted)

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
public Nullable<TimeSpan> Duration { get; set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Job**

```csharp
public Dictionary<string, object> Job { get; set; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

### **Exceptions**

```csharp
public ExceptionInfo Exceptions { get; set; }
```

#### Property Value

[ExceptionInfo](../../masstransit-abstractions/masstransit/exceptioninfo)<br/>

## Constructors

### **JobFaultedEvent()**

```csharp
public JobFaultedEvent()
```
