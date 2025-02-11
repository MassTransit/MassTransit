---

title: PipeContextAgent<TContext>

---

# PipeContextAgent\<TContext\>

Namespace: MassTransit.Agents

A PipeContext, which as an agent can be Stopped, which disposes of the context making it unavailable

```csharp
public class PipeContextAgent<TContext> : Agent, IAgent, IPipeContextAgent<TContext>, PipeContextHandle<TContext>, IAsyncDisposable
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Agent](../../masstransit-abstractions/masstransit-middleware/agent) → [PipeContextAgent\<TContext\>](../masstransit-agents/pipecontextagent-1)<br/>
Implements [IAgent](../../masstransit-abstractions/masstransit/iagent), [IPipeContextAgent\<TContext\>](../masstransit-agents/ipipecontextagent-1), [PipeContextHandle\<TContext\>](../masstransit/pipecontexthandle-1), [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Properties

### **Ready**

```csharp
public Task Ready { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Completed**

```csharp
public Task Completed { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Stopping**

```csharp
public CancellationToken Stopping { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **Stopped**

```csharp
public CancellationToken Stopped { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Constructors

### **PipeContextAgent(TContext)**

```csharp
public PipeContextAgent(TContext context)
```

#### Parameters

`context` TContext<br/>

### **PipeContextAgent(Task\<TContext\>)**

```csharp
public PipeContextAgent(Task<TContext> context)
```

#### Parameters

`context` [Task\<TContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

## Methods

### **DisposeAsync()**

```csharp
public ValueTask DisposeAsync()
```

#### Returns

[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask)<br/>

### **StopAgent(StopContext)**

```csharp
protected Task StopAgent(StopContext context)
```

#### Parameters

`context` [StopContext](../../masstransit-abstractions/masstransit/stopcontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
