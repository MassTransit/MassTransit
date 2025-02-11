---

title: BatchConsumerFactory<TMessage>

---

# BatchConsumerFactory\<TMessage\>

Namespace: MassTransit.Batching

```csharp
public class BatchConsumerFactory<TMessage> : IConsumerFactory<BatchConsumer<TMessage>>, IProbeSite, IAsyncDisposable
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BatchConsumerFactory\<TMessage\>](../masstransit-batching/batchconsumerfactory-1)<br/>
Implements [IConsumerFactory\<BatchConsumer\<TMessage\>\>](../../masstransit-abstractions/masstransit/iconsumerfactory-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Constructors

### **BatchConsumerFactory(BatchOptions, IBatchCollector\<TMessage\>)**

```csharp
public BatchConsumerFactory(BatchOptions options, IBatchCollector<TMessage> collector)
```

#### Parameters

`options` [BatchOptions](../../masstransit-abstractions/masstransit/batchoptions)<br/>

`collector` [IBatchCollector\<TMessage\>](../masstransit-batching/ibatchcollector-1)<br/>

## Methods

### **DisposeAsync()**

```csharp
public ValueTask DisposeAsync()
```

#### Returns

[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask)<br/>

### **Send\<T\>(ConsumeContext\<T\>, IPipe\<ConsumerConsumeContext\<BatchConsumer\<TMessage\>, T\>\>)**

```csharp
public Task Send<T>(ConsumeContext<T> context, IPipe<ConsumerConsumeContext<BatchConsumer<TMessage>, T>> next)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`next` [IPipe\<ConsumerConsumeContext\<BatchConsumer\<TMessage\>, T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
