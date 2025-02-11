---

title: JobStartedEvent<T>

---

# JobStartedEvent\<T\>

Namespace: MassTransit.JobService.Messages

```csharp
public class JobStartedEvent<T> : JobStarted<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobStartedEvent\<T\>](../masstransit-jobservice-messages/jobstartedevent-1)<br/>
Implements [JobStarted\<T\>](../../masstransit-abstractions/masstransit-contracts-jobservice/jobstarted-1)

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

### **RetryAttempt**

```csharp
public int RetryAttempt { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Timestamp**

```csharp
public DateTime Timestamp { get; set; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

## Constructors

### **JobStartedEvent()**

```csharp
public JobStartedEvent()
```
