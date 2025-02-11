---

title: TransportPipeContextSupervisor<T>

---

# TransportPipeContextSupervisor\<T\>

Namespace: MassTransit.Transports

```csharp
public class TransportPipeContextSupervisor<T> : PipeContextSupervisor<T>, IAgent, ISupervisor, ISupervisor<T>, IAgent<T>, IPipeContextSource<T>, IProbeSite, ITransportSupervisor<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Agent](../../masstransit-abstractions/masstransit-middleware/agent) → [Supervisor](../../masstransit-abstractions/masstransit-middleware/supervisor) → [PipeContextSupervisor\<T\>](../masstransit-agents/pipecontextsupervisor-1) → [TransportPipeContextSupervisor\<T\>](../masstransit-transports/transportpipecontextsupervisor-1)<br/>
Implements [IAgent](../../masstransit-abstractions/masstransit/iagent), [ISupervisor](../../masstransit-abstractions/masstransit/isupervisor), [ISupervisor\<T\>](../../masstransit-abstractions/masstransit/isupervisor-1), [IAgent\<T\>](../../masstransit-abstractions/masstransit/iagent-1), [IPipeContextSource\<T\>](../../masstransit-abstractions/masstransit/ipipecontextsource-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [ITransportSupervisor\<T\>](../../masstransit-abstractions/masstransit-transports/itransportsupervisor-1)

## Properties

### **ConsumeStopping**

```csharp
public CancellationToken ConsumeStopping { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **SendStopping**

```csharp
public CancellationToken SendStopping { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

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

## Methods

### **AddSendAgent\<TAgent\>(TAgent)**

```csharp
public void AddSendAgent<TAgent>(TAgent agent)
```

#### Type Parameters

`TAgent`<br/>

#### Parameters

`agent` TAgent<br/>

### **AddConsumeAgent\<TAgent\>(TAgent)**

```csharp
public void AddConsumeAgent<TAgent>(TAgent agent)
```

#### Type Parameters

`TAgent`<br/>

#### Parameters

`agent` TAgent<br/>

### **StopSupervisor(StopSupervisorContext)**

```csharp
protected Task StopSupervisor(StopSupervisorContext context)
```

#### Parameters

`context` [StopSupervisorContext](../../masstransit-abstractions/masstransit/stopsupervisorcontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
