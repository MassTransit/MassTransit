---

title: AllocateJobSlotCommand

---

# AllocateJobSlotCommand

Namespace: MassTransit.JobService.Messages

```csharp
public class AllocateJobSlotCommand : AllocateJobSlot
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AllocateJobSlotCommand](../masstransit-jobservice-messages/allocatejobslotcommand)<br/>
Implements [AllocateJobSlot](../../masstransit-abstractions/masstransit-contracts-jobservice/allocatejobslot)

## Properties

### **JobTypeId**

```csharp
public Guid JobTypeId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **JobTimeout**

```csharp
public TimeSpan JobTimeout { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **JobId**

```csharp
public Guid JobId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **JobProperties**

```csharp
public Dictionary<string, object> JobProperties { get; set; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

## Constructors

### **AllocateJobSlotCommand()**

```csharp
public AllocateJobSlotCommand()
```
