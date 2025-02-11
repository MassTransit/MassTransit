---

title: JobCanceledEvent

---

# JobCanceledEvent

Namespace: MassTransit.JobService.Messages

```csharp
public class JobCanceledEvent : JobCanceled
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobCanceledEvent](../masstransit-jobservice-messages/jobcanceledevent)<br/>
Implements [JobCanceled](../../masstransit-abstractions/masstransit-contracts-jobservice/jobcanceled)

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

## Constructors

### **JobCanceledEvent()**

```csharp
public JobCanceledEvent()
```
