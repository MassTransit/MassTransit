---

title: TaskExecutor

---

# TaskExecutor

Namespace: MassTransit.Util

The successor to [ChannelExecutor](../masstransit-util/channelexecutor), now with a more optimized execution pipeline resulting in
 lower memory usage and reduced overhead.

```csharp
public class TaskExecutor : IAsyncDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TaskExecutor](../masstransit-util/taskexecutor)<br/>
Implements [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Constructors

### **TaskExecutor(Int32)**

```csharp
public TaskExecutor(int concurrencyLimit)
```

#### Parameters

`concurrencyLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **TaskExecutor(Int32, Int32)**

```csharp
public TaskExecutor(int prefetchCount, int concurrencyLimit)
```

#### Parameters

`prefetchCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`concurrencyLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Methods

### **DisposeAsync()**

```csharp
public ValueTask DisposeAsync()
```

#### Returns

[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask)<br/>

### **Run(Action, CancellationToken)**

```csharp
public Task Run(Action method, CancellationToken cancellationToken)
```

#### Parameters

`method` [Action](https://learn.microsoft.com/en-us/dotnet/api/system.action)<br/>

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

### **Push(Action, CancellationToken)**

```csharp
public Task Push(Action method, CancellationToken cancellationToken)
```

#### Parameters

`method` [Action](https://learn.microsoft.com/en-us/dotnet/api/system.action)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Push(Func\<Task\>, CancellationToken)**

```csharp
public Task Push(Func<Task> method, CancellationToken cancellationToken)
```

#### Parameters

`method` [Func\<Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

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
