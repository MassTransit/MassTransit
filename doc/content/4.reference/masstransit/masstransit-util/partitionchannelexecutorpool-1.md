---

title: PartitionChannelExecutorPool<T>

---

# PartitionChannelExecutorPool\<T\>

Namespace: MassTransit.Util

```csharp
public class PartitionChannelExecutorPool<T> : IChannelExecutorPool<T>, IAsyncDisposable
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PartitionChannelExecutorPool\<T\>](../masstransit-util/partitionchannelexecutorpool-1)<br/>
Implements [IChannelExecutorPool\<T\>](../masstransit-util/ichannelexecutorpool-1), [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Constructors

### **PartitionChannelExecutorPool(PartitionKeyProvider\<T\>, IHashGenerator, Int32, Int32)**

```csharp
public PartitionChannelExecutorPool(PartitionKeyProvider<T> partitionKeyProvider, IHashGenerator hashGenerator, int concurrencyLimit, int concurrentDeliveryLimit)
```

#### Parameters

`partitionKeyProvider` [PartitionKeyProvider\<T\>](../masstransit/partitionkeyprovider-1)<br/>

`hashGenerator` [IHashGenerator](../masstransit-middleware/ihashgenerator)<br/>

`concurrencyLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`concurrentDeliveryLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Methods

### **DisposeAsync()**

```csharp
public ValueTask DisposeAsync()
```

#### Returns

[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask)<br/>

### **Push(T, Func\<Task\>, CancellationToken)**

```csharp
public Task Push(T partition, Func<Task> method, CancellationToken cancellationToken)
```

#### Parameters

`partition` T<br/>

`method` [Func\<Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Run(T, Func\<Task\>, CancellationToken)**

```csharp
public Task Run(T partition, Func<Task> method, CancellationToken cancellationToken)
```

#### Parameters

`partition` T<br/>

`method` [Func\<Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
