---

title: AsyncPipeContextHandle<TContext>

---

# AsyncPipeContextHandle\<TContext\>

Namespace: MassTransit.Agents

An asynchronously pipe context handle, which can be completed.

```csharp
public class AsyncPipeContextHandle<TContext> : IAsyncPipeContextHandle<TContext>, PipeContextHandle<TContext>, IAsyncDisposable
```

#### Type Parameters

`TContext`<br/>
The context type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AsyncPipeContextHandle\<TContext\>](../masstransit-agents/asyncpipecontexthandle-1)<br/>
Implements [IAsyncPipeContextHandle\<TContext\>](../masstransit/iasyncpipecontexthandle-1), [PipeContextHandle\<TContext\>](../masstransit/pipecontexthandle-1), [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Constructors

### **AsyncPipeContextHandle()**

Creates the handle

```csharp
public AsyncPipeContextHandle()
```
