---

title: SubmitJobCommand<T>

---

# SubmitJobCommand\<T\>

Namespace: MassTransit.JobService.Messages

```csharp
public class SubmitJobCommand<T> : SubmitJob<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SubmitJobCommand\<T\>](../masstransit-jobservice-messages/submitjobcommand-1)<br/>
Implements [SubmitJob\<T\>](../../masstransit-abstractions/masstransit-contracts-jobservice/submitjob-1)

## Properties

### **JobId**

```csharp
public Guid JobId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Job**

```csharp
public T Job { get; set; }
```

#### Property Value

T<br/>

### **Schedule**

```csharp
public RecurringJobSchedule Schedule { get; set; }
```

#### Property Value

[RecurringJobSchedule](../../masstransit-abstractions/masstransit-contracts-jobservice/recurringjobschedule)<br/>

### **Properties**

```csharp
public Dictionary<string, object> Properties { get; set; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

## Constructors

### **SubmitJobCommand()**

```csharp
public SubmitJobCommand()
```
