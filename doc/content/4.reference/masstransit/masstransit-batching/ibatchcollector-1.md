---

title: IBatchCollector<TMessage>

---

# IBatchCollector\<TMessage\>

Namespace: MassTransit.Batching

```csharp
public interface IBatchCollector<TMessage> : IAsyncDisposable, IProbeSite
```

#### Type Parameters

`TMessage`<br/>

Implements [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Methods

### **Collect(ConsumeContext\<TMessage\>)**

```csharp
Task<BatchConsumer<TMessage>> Collect(ConsumeContext<TMessage> context)
```

#### Parameters

`context` [ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Task\<BatchConsumer\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Complete(ConsumeContext\<TMessage\>, BatchConsumer\<TMessage\>)**

Complete the consumer, since it's already completed, to clear the dictionary if it matches

```csharp
Task Complete(ConsumeContext<TMessage> context, BatchConsumer<TMessage> consumer)
```

#### Parameters

`context` [ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`consumer` [BatchConsumer\<TMessage\>](../masstransit-batching/batchconsumer-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
