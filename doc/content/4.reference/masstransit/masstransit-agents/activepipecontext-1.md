---

title: ActivePipeContext<TContext>

---

# ActivePipeContext\<TContext\>

Namespace: MassTransit.Agents

An active reference to a pipe context, which is managed by an existing pipe context handle.

```csharp
public class ActivePipeContext<TContext> : ActivePipeContextHandle<TContext>, PipeContextHandle<TContext>, IAsyncDisposable
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ActivePipeContext\<TContext\>](../masstransit-agents/activepipecontext-1)<br/>
Implements [ActivePipeContextHandle\<TContext\>](../masstransit-agents/activepipecontexthandle-1), [PipeContextHandle\<TContext\>](../masstransit/pipecontexthandle-1), [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Constructors

### **ActivePipeContext(PipeContextHandle\<TContext\>, Task\<TContext\>)**

Creates the active pipe context handle, which must have completed before this instance is created. Otherwise,
 it would create a pretty nasty async mess that wouldn't handle faults very well (actually, it should, but I haven't tested it).

```csharp
public ActivePipeContext(PipeContextHandle<TContext> contextHandle, Task<TContext> context)
```

#### Parameters

`contextHandle` [PipeContextHandle\<TContext\>](../masstransit/pipecontexthandle-1)<br/>
The context handle of the actual context which is being used

`context` [Task\<TContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The actual context, which should be a completed Task

### **ActivePipeContext(PipeContextHandle\<TContext\>, TContext)**

Creates the active pipe context handle, which must have completed before this instance is created. Otherwise,
 it would create a pretty nasty async mess that wouldn't handle faults very well (actually, it should, but I haven't tested it).

```csharp
public ActivePipeContext(PipeContextHandle<TContext> contextHandle, TContext context)
```

#### Parameters

`contextHandle` [PipeContextHandle\<TContext\>](../masstransit/pipecontexthandle-1)<br/>
The context handle of the actual context which is being used

`context` TContext<br/>
The actual context

## Methods

### **DisposeAsync()**

```csharp
public ValueTask DisposeAsync()
```

#### Returns

[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask)<br/>
