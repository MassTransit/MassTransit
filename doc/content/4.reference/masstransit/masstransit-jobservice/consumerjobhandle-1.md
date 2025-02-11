---

title: ConsumerJobHandle<T>

---

# ConsumerJobHandle\<T\>

Namespace: MassTransit.JobService

```csharp
public class ConsumerJobHandle<T> : JobHandle, IAsyncDisposable
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumerJobHandle\<T\>](../masstransit-jobservice/consumerjobhandle-1)<br/>
Implements [JobHandle](../masstransit-jobservice/jobhandle), [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Properties

### **JobId**

```csharp
public Guid JobId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **JobTask**

```csharp
public Task JobTask { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

## Constructors

### **ConsumerJobHandle(ConsumeJobContext\<T\>, Task, TimeSpan)**

```csharp
public ConsumerJobHandle(ConsumeJobContext<T> context, Task task, TimeSpan jobCancellationTimeout)
```

#### Parameters

`context` [ConsumeJobContext\<T\>](../masstransit-jobservice/consumejobcontext-1)<br/>

`task` [Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

`jobCancellationTimeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

## Methods

### **Cancel(String)**

```csharp
public Task Cancel(string reason)
```

#### Parameters

`reason` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **DisposeAsync()**

```csharp
public ValueTask DisposeAsync()
```

#### Returns

[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask)<br/>
