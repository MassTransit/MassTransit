---

title: IJobDistributionStrategy

---

# IJobDistributionStrategy

Namespace: MassTransit

Used by the [JobTypeStateMachine](../masstransit/jobtypestatemachine) to determine if a job slot should be allocated to a job

```csharp
public interface IJobDistributionStrategy
```

## Methods

### **IsJobSlotAvailable(ConsumeContext\<AllocateJobSlot\>, JobTypeInfo)**

Determine if a job slot is available and return an [ActiveJob](../masstransit/activejob) instance if the job can be assigned to a job consumer instance.
 If no instance is available or the concurrency limits would be exceeded, return null.

```csharp
Task<ActiveJob> IsJobSlotAvailable(ConsumeContext<AllocateJobSlot> context, JobTypeInfo jobTypeInfo)
```

#### Parameters

`context` [ConsumeContext\<AllocateJobSlot\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`jobTypeInfo` [JobTypeInfo](../masstransit/jobtypeinfo)<br/>

#### Returns

[Task\<ActiveJob\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
An [ActiveJob](../masstransit/activejob) if the job can be assigned to a job consumer instance, or null
