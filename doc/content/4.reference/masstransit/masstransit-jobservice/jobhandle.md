---

title: JobHandle

---

# JobHandle

Namespace: MassTransit.JobService

A JobHandle contains the JobContext, Task, and provides access to the job control

```csharp
public interface JobHandle : IAsyncDisposable
```

Implements [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Properties

### **JobId**

```csharp
public abstract Guid JobId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **JobTask**

```csharp
public abstract Task JobTask { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

## Methods

### **Cancel(String)**

Cancel the job task

```csharp
Task Cancel(string reason)
```

#### Parameters

`reason` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
