---

title: JobSlotReleasedEvent

---

# JobSlotReleasedEvent

Namespace: MassTransit.JobService.Messages

```csharp
public class JobSlotReleasedEvent : JobSlotReleased
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobSlotReleasedEvent](../masstransit-jobservice-messages/jobslotreleasedevent)<br/>
Implements [JobSlotReleased](../../masstransit-abstractions/masstransit-contracts-jobservice/jobslotreleased)

## Properties

### **JobTypeId**

```csharp
public Guid JobTypeId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **JobId**

```csharp
public Guid JobId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Disposition**

```csharp
public JobSlotDisposition Disposition { get; set; }
```

#### Property Value

[JobSlotDisposition](../../masstransit-abstractions/masstransit-contracts-jobservice/jobslotdisposition)<br/>

## Constructors

### **JobSlotReleasedEvent()**

```csharp
public JobSlotReleasedEvent()
```
