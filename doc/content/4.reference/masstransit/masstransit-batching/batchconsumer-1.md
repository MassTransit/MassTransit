---

title: BatchConsumer<TMessage>

---

# BatchConsumer\<TMessage\>

Namespace: MassTransit.Batching

```csharp
public class BatchConsumer<TMessage> : IConsumer<TMessage>, IConsumer
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BatchConsumer\<TMessage\>](../masstransit-batching/batchconsumer-1)<br/>
Implements [IConsumer\<TMessage\>](../../masstransit-abstractions/masstransit/iconsumer-1), [IConsumer](../../masstransit-abstractions/masstransit/iconsumer)

## Properties

### **IsCompleted**

```csharp
public bool IsCompleted { get; private set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **BatchConsumer(BatchOptions, TaskExecutor, TaskExecutor, IPipe\<ConsumeContext\<Batch\<TMessage\>\>\>)**

```csharp
public BatchConsumer(BatchOptions options, TaskExecutor executor, TaskExecutor dispatcher, IPipe<ConsumeContext<Batch<TMessage>>> consumerPipe)
```

#### Parameters

`options` [BatchOptions](../../masstransit-abstractions/masstransit/batchoptions)<br/>

`executor` [TaskExecutor](../masstransit-util/taskexecutor)<br/>

`dispatcher` [TaskExecutor](../masstransit-util/taskexecutor)<br/>

`consumerPipe` [IPipe\<ConsumeContext\<Batch\<TMessage\>\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

## Methods

### **Consume(ConsumeContext\<TMessage\>)**

```csharp
public Task Consume(ConsumeContext<TMessage> context)
```

#### Parameters

`context` [ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Add(ConsumeContext\<TMessage\>, Activity)**

```csharp
public Task Add(ConsumeContext<TMessage> context, Activity currentActivity)
```

#### Parameters

`context` [ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`currentActivity` Activity<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ForceComplete()**

```csharp
public Task ForceComplete()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
