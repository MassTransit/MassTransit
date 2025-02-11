---

title: ClientContextSupervisor

---

# ClientContextSupervisor

Namespace: MassTransit.SqlTransport

```csharp
public class ClientContextSupervisor : TransportPipeContextSupervisor<ClientContext>, IAgent, ISupervisor, ISupervisor<ClientContext>, IAgent<ClientContext>, IPipeContextSource<ClientContext>, IProbeSite, ITransportSupervisor<ClientContext>, IClientContextSupervisor
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Agent](../../masstransit-abstractions/masstransit-middleware/agent) → [Supervisor](../../masstransit-abstractions/masstransit-middleware/supervisor) → [PipeContextSupervisor\<ClientContext\>](../masstransit-agents/pipecontextsupervisor-1) → [TransportPipeContextSupervisor\<ClientContext\>](../masstransit-transports/transportpipecontextsupervisor-1) → [ClientContextSupervisor](../masstransit-sqltransport/clientcontextsupervisor)<br/>
Implements [IAgent](../../masstransit-abstractions/masstransit/iagent), [ISupervisor](../../masstransit-abstractions/masstransit/isupervisor), [ISupervisor\<ClientContext\>](../../masstransit-abstractions/masstransit/isupervisor-1), [IAgent\<ClientContext\>](../../masstransit-abstractions/masstransit/iagent-1), [IPipeContextSource\<ClientContext\>](../../masstransit-abstractions/masstransit/ipipecontextsource-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [ITransportSupervisor\<ClientContext\>](../../masstransit-abstractions/masstransit-transports/itransportsupervisor-1), [IClientContextSupervisor](../masstransit-sqltransport/iclientcontextsupervisor)

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

## Constructors

### **ClientContextSupervisor(IConnectionContextSupervisor)**

```csharp
public ClientContextSupervisor(IConnectionContextSupervisor connectionContextSupervisor)
```

#### Parameters

`connectionContextSupervisor` [IConnectionContextSupervisor](../masstransit-sqltransport/iconnectioncontextsupervisor)<br/>

### **ClientContextSupervisor(IClientContextSupervisor)**

```csharp
public ClientContextSupervisor(IClientContextSupervisor clientContextSupervisor)
```

#### Parameters

`clientContextSupervisor` [IClientContextSupervisor](../masstransit-sqltransport/iclientcontextsupervisor)<br/>
