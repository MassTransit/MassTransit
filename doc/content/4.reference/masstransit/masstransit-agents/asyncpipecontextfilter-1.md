---

title: AsyncPipeContextFilter<TContext>

---

# AsyncPipeContextFilter\<TContext\>

Namespace: MassTransit.Agents

Completes the AsyncPipeContextAgent when the context is sent to the pipe, and doesn't return until the agent completes

```csharp
public class AsyncPipeContextFilter<TContext> : IFilter<TContext>, IProbeSite
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AsyncPipeContextFilter\<TContext\>](../masstransit-agents/asyncpipecontextfilter-1)<br/>
Implements [IFilter\<TContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **AsyncPipeContextFilter(IAsyncPipeContextAgent\<TContext\>)**

```csharp
public AsyncPipeContextFilter(IAsyncPipeContextAgent<TContext> agent)
```

#### Parameters

`agent` [IAsyncPipeContextAgent\<TContext\>](../masstransit-agents/iasyncpipecontextagent-1)<br/>

## Methods

### **Send(TContext, IPipe\<TContext\>)**

```csharp
public Task Send(TContext context, IPipe<TContext> next)
```

#### Parameters

`context` TContext<br/>

`next` [IPipe\<TContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
