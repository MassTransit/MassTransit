---

title: IAsyncPipeContextHandle<TContext>

---

# IAsyncPipeContextHandle\<TContext\>

Namespace: MassTransit

Supports the asynchronous notification of a PipeContext becoming available (this is a future of a future, basically)

```csharp
public interface IAsyncPipeContextHandle<TContext> : PipeContextHandle<TContext>, IAsyncDisposable
```

#### Type Parameters

`TContext`<br/>
The context type

Implements [PipeContextHandle\<TContext\>](../masstransit/pipecontexthandle-1), [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Methods

### **Created(TContext)**

Called when the PipeContext has been created and is available for use.

```csharp
Task Created(TContext context)
```

#### Parameters

`context` TContext<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **CreateCanceled()**

Called when the PipeContext creation was canceled

```csharp
Task CreateCanceled()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **CreateFaulted(Exception)**

Called when the PipeContext creation failed

```csharp
Task CreateFaulted(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Faulted(Exception)**

Called when the successfully created PipeContext becomes faulted, indicating that it
 should no longer be used.

```csharp
Task Faulted(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>
The exception which occurred

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
