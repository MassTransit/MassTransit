---

title: ConcurrencyLimitFilter<TContext>

---

# ConcurrencyLimitFilter\<TContext\>

Namespace: MassTransit.Middleware

Limits the concurrency of the next section of the pipeline based on the concurrency limit
 specified.

```csharp
public class ConcurrencyLimitFilter<TContext> : Agent, IAgent, IFilter<TContext>, IProbeSite, IPipe<CommandContext<SetConcurrencyLimit>>, IDisposable
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Agent](../../masstransit-abstractions/masstransit-middleware/agent) → [ConcurrencyLimitFilter\<TContext\>](../masstransit-middleware/concurrencylimitfilter-1)<br/>
Implements [IAgent](../../masstransit-abstractions/masstransit/iagent), [IFilter\<TContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IPipe\<CommandContext\<SetConcurrencyLimit\>\>](../../masstransit-abstractions/masstransit/ipipe-1), [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)

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

### **ConcurrencyLimitFilter(Int32)**

```csharp
public ConcurrencyLimitFilter(int concurrencyLimit)
```

#### Parameters

`concurrencyLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Methods

### **Dispose()**

```csharp
public void Dispose()
```

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **Send(TContext, IPipe\<TContext\>)**

```csharp
public Task Send(TContext context, IPipe<TContext> next)
```

#### Parameters

`context` TContext<br/>

`next` [IPipe\<TContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Send(CommandContext\<SetConcurrencyLimit\>)**

```csharp
public Task Send(CommandContext<SetConcurrencyLimit> context)
```

#### Parameters

`context` [CommandContext\<SetConcurrencyLimit\>](../../masstransit-abstractions/masstransit-contracts/commandcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **StopAgent(StopContext)**

```csharp
protected Task StopAgent(StopContext context)
```

#### Parameters

`context` [StopContext](../../masstransit-abstractions/masstransit/stopcontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
