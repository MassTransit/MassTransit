---

title: AsyncPipeContextPipe<TContext>

---

# AsyncPipeContextPipe\<TContext\>

Namespace: MassTransit.Agents

Completes the AsyncPipeContextAgent when the context is sent to the pipe, and doesn't return until the agent completes

```csharp
public class AsyncPipeContextPipe<TContext> : IPipe<TContext>, IProbeSite
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AsyncPipeContextPipe\<TContext\>](../masstransit-agents/asyncpipecontextpipe-1)<br/>
Implements [IPipe\<TContext\>](../../masstransit-abstractions/masstransit/ipipe-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **AsyncPipeContextPipe(IAsyncPipeContextAgent\<TContext\>, IPipe\<TContext\>)**

```csharp
public AsyncPipeContextPipe(IAsyncPipeContextAgent<TContext> agent, IPipe<TContext> pipe)
```

#### Parameters

`agent` [IAsyncPipeContextAgent\<TContext\>](../masstransit-agents/iasyncpipecontextagent-1)<br/>

`pipe` [IPipe\<TContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

## Methods

### **Send(TContext)**

```csharp
public Task Send(TContext context)
```

#### Parameters

`context` TContext<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
