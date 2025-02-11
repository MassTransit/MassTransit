---

title: BatchCollector<TMessage, TKey>

---

# BatchCollector\<TMessage, TKey\>

Namespace: MassTransit.Batching

```csharp
public class BatchCollector<TMessage, TKey> : IBatchCollector<TMessage>, IAsyncDisposable, IProbeSite
```

#### Type Parameters

`TMessage`<br/>

`TKey`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BatchCollector\<TMessage, TKey\>](../masstransit-batching/batchcollector-2)<br/>
Implements [IBatchCollector\<TMessage\>](../masstransit-batching/ibatchcollector-1), [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **BatchCollector(BatchOptions, IPipe\<ConsumeContext\<Batch\<TMessage\>\>\>, IGroupKeyProvider\<TMessage, TKey\>)**

```csharp
public BatchCollector(BatchOptions options, IPipe<ConsumeContext<Batch<TMessage>>> consumerPipe, IGroupKeyProvider<TMessage, TKey> keyProvider)
```

#### Parameters

`options` [BatchOptions](../../masstransit-abstractions/masstransit/batchoptions)<br/>

`consumerPipe` [IPipe\<ConsumeContext\<Batch\<TMessage\>\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`keyProvider` [IGroupKeyProvider\<TMessage, TKey\>](../../masstransit-abstractions/masstransit/igroupkeyprovider-2)<br/>

## Methods

### **DisposeAsync()**

```csharp
public ValueTask DisposeAsync()
```

#### Returns

[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask)<br/>

### **Collect(ConsumeContext\<TMessage\>)**

```csharp
public Task<BatchConsumer<TMessage>> Collect(ConsumeContext<TMessage> context)
```

#### Parameters

`context` [ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Task\<BatchConsumer\<TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Complete(ConsumeContext\<TMessage\>, BatchConsumer\<TMessage\>)**

```csharp
public Task Complete(ConsumeContext<TMessage> context, BatchConsumer<TMessage> consumer)
```

#### Parameters

`context` [ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`consumer` [BatchConsumer\<TMessage\>](../masstransit-batching/batchconsumer-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
