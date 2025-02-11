---

title: ConnectionContextSupervisor

---

# ConnectionContextSupervisor

Namespace: MassTransit.SqlTransport

```csharp
public class ConnectionContextSupervisor : TransportPipeContextSupervisor<ConnectionContext>, IAgent, ISupervisor, ISupervisor<ConnectionContext>, IAgent<ConnectionContext>, IPipeContextSource<ConnectionContext>, IProbeSite, ITransportSupervisor<ConnectionContext>, IConnectionContextSupervisor
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Agent](../../masstransit-abstractions/masstransit-middleware/agent) → [Supervisor](../../masstransit-abstractions/masstransit-middleware/supervisor) → [PipeContextSupervisor\<ConnectionContext\>](../masstransit-agents/pipecontextsupervisor-1) → [TransportPipeContextSupervisor\<ConnectionContext\>](../masstransit-transports/transportpipecontextsupervisor-1) → [ConnectionContextSupervisor](../masstransit-sqltransport/connectioncontextsupervisor)<br/>
Implements [IAgent](../../masstransit-abstractions/masstransit/iagent), [ISupervisor](../../masstransit-abstractions/masstransit/isupervisor), [ISupervisor\<ConnectionContext\>](../../masstransit-abstractions/masstransit/isupervisor-1), [IAgent\<ConnectionContext\>](../../masstransit-abstractions/masstransit/iagent-1), [IPipeContextSource\<ConnectionContext\>](../../masstransit-abstractions/masstransit/ipipecontextsource-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [ITransportSupervisor\<ConnectionContext\>](../../masstransit-abstractions/masstransit-transports/itransportsupervisor-1), [IConnectionContextSupervisor](../masstransit-sqltransport/iconnectioncontextsupervisor)

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

### **ConnectionContextSupervisor(ISqlHostConfiguration, ISqlTopologyConfiguration, IPipeContextFactory\<ConnectionContext\>)**

```csharp
public ConnectionContextSupervisor(ISqlHostConfiguration hostConfiguration, ISqlTopologyConfiguration topologyConfiguration, IPipeContextFactory<ConnectionContext> connectionContextFactory)
```

#### Parameters

`hostConfiguration` [ISqlHostConfiguration](../masstransit-sqltransport-configuration/isqlhostconfiguration)<br/>

`topologyConfiguration` [ISqlTopologyConfiguration](../masstransit-sqltransport-configuration/isqltopologyconfiguration)<br/>

`connectionContextFactory` [IPipeContextFactory\<ConnectionContext\>](../masstransit-agents/ipipecontextfactory-1)<br/>

## Methods

### **NormalizeAddress(Uri)**

```csharp
public Uri NormalizeAddress(Uri address)
```

#### Parameters

`address` Uri<br/>

#### Returns

Uri<br/>

### **CreatePublishTransport\<T\>(SqlReceiveEndpointContext, Uri)**

```csharp
public Task<ISendTransport> CreatePublishTransport<T>(SqlReceiveEndpointContext context, Uri publishAddress)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [SqlReceiveEndpointContext](../masstransit-sqltransport/sqlreceiveendpointcontext)<br/>

`publishAddress` Uri<br/>

#### Returns

[Task\<ISendTransport\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **CreateSendTransport(SqlReceiveEndpointContext, Uri)**

```csharp
public Task<ISendTransport> CreateSendTransport(SqlReceiveEndpointContext context, Uri address)
```

#### Parameters

`context` [SqlReceiveEndpointContext](../masstransit-sqltransport/sqlreceiveendpointcontext)<br/>

`address` Uri<br/>

#### Returns

[Task\<ISendTransport\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
