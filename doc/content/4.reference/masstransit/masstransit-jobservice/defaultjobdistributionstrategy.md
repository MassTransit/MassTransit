---

title: DefaultJobDistributionStrategy

---

# DefaultJobDistributionStrategy

Namespace: MassTransit.JobService

```csharp
public class DefaultJobDistributionStrategy : IJobDistributionStrategy
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DefaultJobDistributionStrategy](../masstransit-jobservice/defaultjobdistributionstrategy)<br/>
Implements [IJobDistributionStrategy](../masstransit/ijobdistributionstrategy)

## Fields

### **Instance**

```csharp
public static IJobDistributionStrategy Instance;
```

## Constructors

### **DefaultJobDistributionStrategy()**

```csharp
public DefaultJobDistributionStrategy()
```

## Methods

### **IsJobSlotAvailable(ConsumeContext\<AllocateJobSlot\>, JobTypeInfo)**

```csharp
public Task<ActiveJob> IsJobSlotAvailable(ConsumeContext<AllocateJobSlot> context, JobTypeInfo jobTypeInfo)
```

#### Parameters

`context` [ConsumeContext\<AllocateJobSlot\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`jobTypeInfo` [JobTypeInfo](../masstransit/jobtypeinfo)<br/>

#### Returns

[Task\<ActiveJob\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
