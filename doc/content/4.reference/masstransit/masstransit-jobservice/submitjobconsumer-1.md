---

title: SubmitJobConsumer<TJob>

---

# SubmitJobConsumer\<TJob\>

Namespace: MassTransit.JobService

Handles the  command

```csharp
public class SubmitJobConsumer<TJob> : IConsumer<TJob>, IConsumer, IConsumer<SubmitJob<TJob>>
```

#### Type Parameters

`TJob`<br/>
The job type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SubmitJobConsumer\<TJob\>](../masstransit-jobservice/submitjobconsumer-1)<br/>
Implements [IConsumer\<TJob\>](../../masstransit-abstractions/masstransit/iconsumer-1), [IConsumer](../../masstransit-abstractions/masstransit/iconsumer), [IConsumer\<SubmitJob\<TJob\>\>](../../masstransit-abstractions/masstransit/iconsumer-1)

## Constructors

### **SubmitJobConsumer(JobOptions\<TJob\>, Guid)**

```csharp
public SubmitJobConsumer(JobOptions<TJob> options, Guid jobTypeId)
```

#### Parameters

`options` [JobOptions\<TJob\>](../masstransit/joboptions-1)<br/>

`jobTypeId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

## Methods

### **Consume(ConsumeContext\<SubmitJob\<TJob\>\>)**

```csharp
public Task Consume(ConsumeContext<SubmitJob<TJob>> context)
```

#### Parameters

`context` [ConsumeContext\<SubmitJob\<TJob\>\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Consume(ConsumeContext\<TJob\>)**

```csharp
public Task Consume(ConsumeContext<TJob> context)
```

#### Parameters

`context` [ConsumeContext\<TJob\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
