---

title: AsyncPipeContextAgent<TContext>

---

# AsyncPipeContextAgent\<TContext\>

Namespace: MassTransit.Agents

A PipeContext, which as an agent can be Stopped, which disposes of the context making it unavailable

```csharp
public class AsyncPipeContextAgent<TContext> : IAsyncPipeContextAgent<TContext>, IAsyncPipeContextHandle<TContext>, PipeContextHandle<TContext>, IAsyncDisposable, IPipeContextAgent<TContext>, IAgent
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AsyncPipeContextAgent\<TContext\>](../masstransit-agents/asyncpipecontextagent-1)<br/>
Implements [IAsyncPipeContextAgent\<TContext\>](../masstransit-agents/iasyncpipecontextagent-1), [IAsyncPipeContextHandle\<TContext\>](../masstransit/iasyncpipecontexthandle-1), [PipeContextHandle\<TContext\>](../masstransit/pipecontexthandle-1), [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable), [IPipeContextAgent\<TContext\>](../masstransit-agents/ipipecontextagent-1), [IAgent](../../masstransit-abstractions/masstransit/iagent)

## Constructors

### **AsyncPipeContextAgent()**

```csharp
public AsyncPipeContextAgent()
```

## Methods

### **ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
