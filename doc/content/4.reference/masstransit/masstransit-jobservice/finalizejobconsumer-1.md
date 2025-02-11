---

title: FinalizeJobConsumer<TJob>

---

# FinalizeJobConsumer\<TJob\>

Namespace: MassTransit.JobService

```csharp
public class FinalizeJobConsumer<TJob> : IConsumer<FaultJob>, IConsumer, IConsumer<CompleteJob>
```

#### Type Parameters

`TJob`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FinalizeJobConsumer\<TJob\>](../masstransit-jobservice/finalizejobconsumer-1)<br/>
Implements [IConsumer\<FaultJob\>](../../masstransit-abstractions/masstransit/iconsumer-1), [IConsumer](../../masstransit-abstractions/masstransit/iconsumer), [IConsumer\<CompleteJob\>](../../masstransit-abstractions/masstransit/iconsumer-1)

## Constructors

### **FinalizeJobConsumer(Guid, String)**

```csharp
public FinalizeJobConsumer(Guid jobTypeId, string jobConsumerTypeName)
```

#### Parameters

`jobTypeId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`jobConsumerTypeName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **Consume(ConsumeContext\<CompleteJob\>)**

```csharp
public Task Consume(ConsumeContext<CompleteJob> context)
```

#### Parameters

`context` [ConsumeContext\<CompleteJob\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Consume(ConsumeContext\<FaultJob\>)**

```csharp
public Task Consume(ConsumeContext<FaultJob> context)
```

#### Parameters

`context` [ConsumeContext\<FaultJob\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
