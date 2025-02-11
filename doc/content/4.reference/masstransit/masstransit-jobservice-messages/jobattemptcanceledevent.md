---

title: JobAttemptCanceledEvent

---

# JobAttemptCanceledEvent

Namespace: MassTransit.JobService.Messages

```csharp
public class JobAttemptCanceledEvent : JobAttemptCanceled
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobAttemptCanceledEvent](../masstransit-jobservice-messages/jobattemptcanceledevent)<br/>
Implements [JobAttemptCanceled](../../masstransit-abstractions/masstransit-contracts-jobservice/jobattemptcanceled)

## Properties

### **JobId**

```csharp
public Guid JobId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **AttemptId**

```csharp
public Guid AttemptId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Timestamp**

```csharp
public DateTime Timestamp { get; set; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **Reason**

```csharp
public string Reason { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **JobAttemptCanceledEvent()**

```csharp
public JobAttemptCanceledEvent()
```
