---

title: ActivePipeContextHandle<TContext>

---

# ActivePipeContextHandle\<TContext\>

Namespace: MassTransit.Agents

An active, in-use reference to a pipe context.

```csharp
public interface ActivePipeContextHandle<TContext> : PipeContextHandle<TContext>, IAsyncDisposable
```

#### Type Parameters

`TContext`<br/>

Implements [PipeContextHandle\<TContext\>](../masstransit/pipecontexthandle-1), [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Methods

### **Faulted(Exception)**

If the use of this context results in a fault which should cause the context to be disposed, this method signals that behavior to occur.

```csharp
Task Faulted(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>
The bad thing that happened

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
