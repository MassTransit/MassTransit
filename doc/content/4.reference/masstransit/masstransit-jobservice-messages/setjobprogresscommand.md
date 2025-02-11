---

title: SetJobProgressCommand

---

# SetJobProgressCommand

Namespace: MassTransit.JobService.Messages

```csharp
public class SetJobProgressCommand : SetJobProgress
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SetJobProgressCommand](../masstransit-jobservice-messages/setjobprogresscommand)<br/>
Implements [SetJobProgress](../../masstransit-abstractions/masstransit-contracts-jobservice/setjobprogress)

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

### **SequenceNumber**

```csharp
public long SequenceNumber { get; set; }
```

#### Property Value

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

### **Value**

```csharp
public long Value { get; set; }
```

#### Property Value

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

### **Limit**

```csharp
public Nullable<long> Limit { get; set; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **SetJobProgressCommand()**

```csharp
public SetJobProgressCommand()
```
