---

title: StartJobConsumer<TJob>

---

# StartJobConsumer\<TJob\>

Namespace: MassTransit.JobService

```csharp
public class StartJobConsumer<TJob> : IConsumer<StartJob>, IConsumer
```

#### Type Parameters

`TJob`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [StartJobConsumer\<TJob\>](../masstransit-jobservice/startjobconsumer-1)<br/>
Implements [IConsumer\<StartJob\>](../../masstransit-abstractions/masstransit/iconsumer-1), [IConsumer](../../masstransit-abstractions/masstransit/iconsumer)

## Constructors

### **StartJobConsumer(IJobService, JobOptions\<TJob\>, Guid, IPipe\<ConsumeContext\<TJob\>\>)**

```csharp
public StartJobConsumer(IJobService jobService, JobOptions<TJob> options, Guid jobTypeId, IPipe<ConsumeContext<TJob>> jobPipe)
```

#### Parameters

`jobService` [IJobService](../masstransit-jobservice/ijobservice)<br/>

`options` [JobOptions\<TJob\>](../masstransit/joboptions-1)<br/>

`jobTypeId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`jobPipe` [IPipe\<ConsumeContext\<TJob\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

## Methods

### **Consume(ConsumeContext\<StartJob\>)**

```csharp
public Task Consume(ConsumeContext<StartJob> context)
```

#### Parameters

`context` [ConsumeContext\<StartJob\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
