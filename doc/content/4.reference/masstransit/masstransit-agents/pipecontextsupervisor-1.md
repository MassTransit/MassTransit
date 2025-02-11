---

title: PipeContextSupervisor<TContext>

---

# PipeContextSupervisor\<TContext\>

Namespace: MassTransit.Agents

Maintains a cached context, which is created upon first use, and recreated whenever a fault is propagated to the usage.

```csharp
public class PipeContextSupervisor<TContext> : Supervisor, IAgent, ISupervisor, ISupervisor<TContext>, IAgent<TContext>, IPipeContextSource<TContext>, IProbeSite
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Agent](../../masstransit-abstractions/masstransit-middleware/agent) → [Supervisor](../../masstransit-abstractions/masstransit-middleware/supervisor) → [PipeContextSupervisor\<TContext\>](../masstransit-agents/pipecontextsupervisor-1)<br/>
Implements [IAgent](../../masstransit-abstractions/masstransit/iagent), [ISupervisor](../../masstransit-abstractions/masstransit/isupervisor), [ISupervisor\<TContext\>](../../masstransit-abstractions/masstransit/isupervisor-1), [IAgent\<TContext\>](../../masstransit-abstractions/masstransit/iagent-1), [IPipeContextSource\<TContext\>](../../masstransit-abstractions/masstransit/ipipecontextsource-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Properties

### **PeakActiveCount**

```csharp
public int PeakActiveCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **TotalCount**

```csharp
public long TotalCount { get; }
```

#### Property Value

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

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

### **PipeContextSupervisor(IPipeContextFactory\<TContext\>)**

Create the cache

```csharp
public PipeContextSupervisor(IPipeContextFactory<TContext> contextFactory)
```

#### Parameters

`contextFactory` [IPipeContextFactory\<TContext\>](../masstransit-agents/ipipecontextfactory-1)<br/>
Factory used to create the underlying and active contexts

## Methods

### **Send(IPipe\<TContext\>, CancellationToken)**

```csharp
public Task Send(IPipe<TContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`pipe` [IPipe\<TContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **StopSupervisor(StopSupervisorContext)**

```csharp
protected Task StopSupervisor(StopSupervisorContext context)
```

#### Parameters

`context` [StopSupervisorContext](../../masstransit-abstractions/masstransit/stopsupervisorcontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
