---

title: JobStarted<T>

---

# JobStarted\<T\>

Namespace: MassTransit.Contracts.JobService

Event published when a node starts processing a job (separately from [JobStarted](../masstransit-contracts-jobservice/jobstarted))

```csharp
public interface JobStarted<T>
```

#### Type Parameters

`T`<br/>

## Properties

### **JobId**

The job identifier

```csharp
public abstract Guid JobId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **AttemptId**

Identifies this attempt to run the job

```csharp
public abstract Guid AttemptId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **RetryAttempt**

Zero if the job is being started for the first time, otherwise, the number of previous failures

```csharp
public abstract int RetryAttempt { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Timestamp**

The time the job was started

```csharp
public abstract DateTime Timestamp { get; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>
