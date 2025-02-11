---

title: JobSlotAllocatedResponse

---

# JobSlotAllocatedResponse

Namespace: MassTransit.JobService.Messages

```csharp
public class JobSlotAllocatedResponse : JobSlotAllocated
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobSlotAllocatedResponse](../masstransit-jobservice-messages/jobslotallocatedresponse)<br/>
Implements [JobSlotAllocated](../../masstransit-abstractions/masstransit-contracts-jobservice/jobslotallocated)

## Properties

### **JobId**

```csharp
public Guid JobId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **InstanceAddress**

```csharp
public Uri InstanceAddress { get; set; }
```

#### Property Value

Uri<br/>

## Constructors

### **JobSlotAllocatedResponse()**

```csharp
public JobSlotAllocatedResponse()
```
