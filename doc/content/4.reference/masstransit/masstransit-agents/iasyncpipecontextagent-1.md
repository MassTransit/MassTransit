---

title: IAsyncPipeContextAgent<TContext>

---

# IAsyncPipeContextAgent\<TContext\>

Namespace: MassTransit.Agents

```csharp
public interface IAsyncPipeContextAgent<TContext> : IAsyncPipeContextHandle<TContext>, PipeContextHandle<TContext>, IAsyncDisposable, IPipeContextAgent<TContext>, IAgent
```

#### Type Parameters

`TContext`<br/>

Implements [IAsyncPipeContextHandle\<TContext\>](../masstransit/iasyncpipecontexthandle-1), [PipeContextHandle\<TContext\>](../masstransit/pipecontexthandle-1), [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable), [IPipeContextAgent\<TContext\>](../masstransit-agents/ipipecontextagent-1), [IAgent](../../masstransit-abstractions/masstransit/iagent)
