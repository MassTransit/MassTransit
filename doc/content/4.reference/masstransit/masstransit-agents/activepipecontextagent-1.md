---

title: ActivePipeContextAgent<TContext>

---

# ActivePipeContextAgent\<TContext\>

Namespace: MassTransit.Agents

An Agent Provocateur that uses a context handle for the activate state of the agent

```csharp
public class ActivePipeContextAgent<TContext> : Agent, IAgent, IActivePipeContextAgent<TContext>, ActivePipeContextHandle<TContext>, PipeContextHandle<TContext>, IAsyncDisposable
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Agent](../../masstransit-abstractions/masstransit-middleware/agent) → [ActivePipeContextAgent\<TContext\>](../masstransit-agents/activepipecontextagent-1)<br/>
Implements [IAgent](../../masstransit-abstractions/masstransit/iagent), [IActivePipeContextAgent\<TContext\>](../masstransit-agents/iactivepipecontextagent-1), [ActivePipeContextHandle\<TContext\>](../masstransit-agents/activepipecontexthandle-1), [PipeContextHandle\<TContext\>](../masstransit/pipecontexthandle-1), [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

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

### **ActivePipeContextAgent(ActivePipeContextHandle\<TContext\>)**

```csharp
public ActivePipeContextAgent(ActivePipeContextHandle<TContext> context)
```

#### Parameters

`context` [ActivePipeContextHandle\<TContext\>](../masstransit-agents/activepipecontexthandle-1)<br/>

## Methods

### **StopAgent(StopContext)**

```csharp
protected Task StopAgent(StopContext context)
```

#### Parameters

`context` [StopContext](../../masstransit-abstractions/masstransit/stopcontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
