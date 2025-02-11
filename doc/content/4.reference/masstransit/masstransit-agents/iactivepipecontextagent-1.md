---

title: IActivePipeContextAgent<TContext>

---

# IActivePipeContextAgent\<TContext\>

Namespace: MassTransit.Agents

An active use of a pipe context as an agent.

```csharp
public interface IActivePipeContextAgent<TContext> : ActivePipeContextHandle<TContext>, PipeContextHandle<TContext>, IAsyncDisposable, IAgent
```

#### Type Parameters

`TContext`<br/>

Implements [ActivePipeContextHandle\<TContext\>](../masstransit-agents/activepipecontexthandle-1), [PipeContextHandle\<TContext\>](../masstransit/pipecontexthandle-1), [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable), [IAgent](../../masstransit-abstractions/masstransit/iagent)
