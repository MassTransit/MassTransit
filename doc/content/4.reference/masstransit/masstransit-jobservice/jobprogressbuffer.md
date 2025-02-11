---

title: JobProgressBuffer

---

# JobProgressBuffer

Namespace: MassTransit.JobService

```csharp
public class JobProgressBuffer
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobProgressBuffer](../masstransit-jobservice/jobprogressbuffer)

## Constructors

### **JobProgressBuffer(INotifyJobContext, ProgressBufferSettings)**

```csharp
public JobProgressBuffer(INotifyJobContext notifyJobContext, ProgressBufferSettings settings)
```

#### Parameters

`notifyJobContext` [INotifyJobContext](../../masstransit-abstractions/masstransit/inotifyjobcontext)<br/>

`settings` [ProgressBufferSettings](../masstransit/progressbuffersettings)<br/>

## Methods

### **Flush()**

```csharp
public Task Flush()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Update(ProgressUpdate, CancellationToken)**

```csharp
public Task Update(ProgressUpdate progress, CancellationToken cancellationToken)
```

#### Parameters

`progress` [ProgressUpdate](../masstransit-jobservice/progressupdate)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
