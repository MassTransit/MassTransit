---

title: FaultJobCommand

---

# FaultJobCommand

Namespace: MassTransit.JobService.Messages

```csharp
public class FaultJobCommand : FaultJob
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FaultJobCommand](../masstransit-jobservice-messages/faultjobcommand)<br/>
Implements [FaultJob](../../masstransit-abstractions/masstransit-contracts-jobservice/faultjob)

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

### **Duration**

```csharp
public Nullable<TimeSpan> Duration { get; set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Exceptions**

```csharp
public ExceptionInfo Exceptions { get; set; }
```

#### Property Value

[ExceptionInfo](../../masstransit-abstractions/masstransit/exceptioninfo)<br/>

### **Job**

```csharp
public Dictionary<string, object> Job { get; set; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

### **JobTypeId**

```csharp
public Guid JobTypeId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

## Constructors

### **FaultJobCommand()**

```csharp
public FaultJobCommand()
```
