---

title: JobCompleted

---

# JobCompleted

Namespace: MassTransit.Contracts.JobService

Published when a job completes

```csharp
public interface JobCompleted
```

## Properties

### **JobId**

The job identifier

```csharp
public abstract Guid JobId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Timestamp**

```csharp
public abstract DateTime Timestamp { get; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **Duration**

```csharp
public abstract TimeSpan Duration { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **Job**

The arguments used to start the job

```csharp
public abstract Dictionary<string, object> Job { get; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>

### **Result**

The result of the job

```csharp
public abstract Dictionary<string, object> Result { get; }
```

#### Property Value

[Dictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br/>
