---

title: SetJobProgress

---

# SetJobProgress

Namespace: MassTransit.Contracts.JobService

```csharp
public interface SetJobProgress
```

## Properties

### **JobId**

```csharp
public abstract Guid JobId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **AttemptId**

```csharp
public abstract Guid AttemptId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **SequenceNumber**

```csharp
public abstract long SequenceNumber { get; }
```

#### Property Value

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

### **Value**

The current job progress value

```csharp
public abstract long Value { get; }
```

#### Property Value

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

### **Limit**

The maximum value of job progress (optional)

```csharp
public abstract Nullable<long> Limit { get; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
