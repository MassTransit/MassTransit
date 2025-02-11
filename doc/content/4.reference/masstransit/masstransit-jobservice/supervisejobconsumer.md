---

title: SuperviseJobConsumer

---

# SuperviseJobConsumer

Namespace: MassTransit.JobService

```csharp
public class SuperviseJobConsumer : IConsumer<CancelJobAttempt>, IConsumer, IConsumer<GetJobAttemptStatus>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SuperviseJobConsumer](../masstransit-jobservice/supervisejobconsumer)<br/>
Implements [IConsumer\<CancelJobAttempt\>](../../masstransit-abstractions/masstransit/iconsumer-1), [IConsumer](../../masstransit-abstractions/masstransit/iconsumer), [IConsumer\<GetJobAttemptStatus\>](../../masstransit-abstractions/masstransit/iconsumer-1)

## Constructors

### **SuperviseJobConsumer(IJobService)**

```csharp
public SuperviseJobConsumer(IJobService jobService)
```

#### Parameters

`jobService` [IJobService](../masstransit-jobservice/ijobservice)<br/>

## Methods

### **Consume(ConsumeContext\<CancelJobAttempt\>)**

```csharp
public Task Consume(ConsumeContext<CancelJobAttempt> context)
```

#### Parameters

`context` [ConsumeContext\<CancelJobAttempt\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Consume(ConsumeContext\<GetJobAttemptStatus\>)**

```csharp
public Task Consume(ConsumeContext<GetJobAttemptStatus> context)
```

#### Parameters

`context` [ConsumeContext\<GetJobAttemptStatus\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
