---

title: IJobConsumer<TJob>

---

# IJobConsumer\<TJob\>

Namespace: MassTransit

Defines a message consumer which runs a job asynchronously, without waiting, which is monitored by Conductor
 services, to monitor the job, limit concurrency, etc.

```csharp
public interface IJobConsumer<TJob> : IConsumer
```

#### Type Parameters

`TJob`<br/>
The job message type

Implements [IConsumer](../masstransit/iconsumer)

## Methods

### **Run(JobContext\<TJob\>)**

```csharp
Task Run(JobContext<TJob> context)
```

#### Parameters

`context` [JobContext\<TJob\>](../masstransit/jobcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
