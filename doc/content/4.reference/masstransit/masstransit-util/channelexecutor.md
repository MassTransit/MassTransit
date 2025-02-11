---

title: ChannelExecutor

---

# ChannelExecutor

Namespace: MassTransit.Util

```csharp
public class ChannelExecutor : IAsyncDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ChannelExecutor](../masstransit-util/channelexecutor)<br/>
Implements [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Constructors

### **ChannelExecutor(Int32, Int32)**

```csharp
public ChannelExecutor(int prefetchCount, int concurrencyLimit)
```

#### Parameters

`prefetchCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`concurrencyLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ChannelExecutor(Int32, Boolean)**

```csharp
public ChannelExecutor(int concurrencyLimit, bool allowSynchronousContinuations)
```

#### Parameters

`concurrencyLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`allowSynchronousContinuations` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Methods

### **DisposeAsync()**

```csharp
public ValueTask DisposeAsync()
```

#### Returns

[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask)<br/>

### **PushWithWait(Func\<Task\>, CancellationToken)**

```csharp
public void PushWithWait(Func<Task> method, CancellationToken cancellationToken)
```

#### Parameters

`method` [Func\<Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **Push(Func\<Task\>, CancellationToken)**

```csharp
public Task Push(Func<Task> method, CancellationToken cancellationToken)
```

#### Parameters

`method` [Func\<Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Run(Func\<Task\>, CancellationToken)**

```csharp
public Task Run(Func<Task> method, CancellationToken cancellationToken)
```

#### Parameters

`method` [Func\<Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Run\<T\>(Func\<Task\<T\>\>, CancellationToken)**

```csharp
public Task<T> Run<T>(Func<Task<T>> method, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`method` [Func\<Task\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Run(Action, CancellationToken)**

```csharp
public Task Run(Action method, CancellationToken cancellationToken)
```

#### Parameters

`method` [Action](https://learn.microsoft.com/en-us/dotnet/api/system.action)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Run\<T\>(Func\<T\>, CancellationToken)**

```csharp
public Task<T> Run<T>(Func<T> method, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`method` [Func\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
