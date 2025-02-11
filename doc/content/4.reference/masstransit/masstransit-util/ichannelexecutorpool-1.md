---

title: IChannelExecutorPool<TPartition>

---

# IChannelExecutorPool\<TPartition\>

Namespace: MassTransit.Util

```csharp
public interface IChannelExecutorPool<TPartition> : IAsyncDisposable
```

#### Type Parameters

`TPartition`<br/>

Implements [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Methods

### **Push(TPartition, Func\<Task\>, CancellationToken)**

```csharp
Task Push(TPartition partition, Func<Task> method, CancellationToken cancellationToken)
```

#### Parameters

`partition` TPartition<br/>

`method` [Func\<Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Run(TPartition, Func\<Task\>, CancellationToken)**

```csharp
Task Run(TPartition partition, Func<Task> method, CancellationToken cancellationToken)
```

#### Parameters

`partition` TPartition<br/>

`method` [Func\<Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
