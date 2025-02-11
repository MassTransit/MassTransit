---

title: IConnectionContextSupervisor

---

# IConnectionContextSupervisor

Namespace: MassTransit.SqlTransport

```csharp
public interface IConnectionContextSupervisor : ITransportSupervisor<ConnectionContext>, ISupervisor<ConnectionContext>, ISupervisor, IAgent, IAgent<ConnectionContext>, IPipeContextSource<ConnectionContext>, IProbeSite
```

Implements [ITransportSupervisor\<ConnectionContext\>](../../masstransit-abstractions/masstransit-transports/itransportsupervisor-1), [ISupervisor\<ConnectionContext\>](../../masstransit-abstractions/masstransit/isupervisor-1), [ISupervisor](../../masstransit-abstractions/masstransit/isupervisor), [IAgent](../../masstransit-abstractions/masstransit/iagent), [IAgent\<ConnectionContext\>](../../masstransit-abstractions/masstransit/iagent-1), [IPipeContextSource\<ConnectionContext\>](../../masstransit-abstractions/masstransit/ipipecontextsource-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Methods

### **CreateSendTransport(SqlReceiveEndpointContext, Uri)**

```csharp
Task<ISendTransport> CreateSendTransport(SqlReceiveEndpointContext context, Uri address)
```

#### Parameters

`context` [SqlReceiveEndpointContext](../masstransit-sqltransport/sqlreceiveendpointcontext)<br/>

`address` Uri<br/>

#### Returns

[Task\<ISendTransport\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **CreatePublishTransport\<T\>(SqlReceiveEndpointContext, Uri)**

```csharp
Task<ISendTransport> CreatePublishTransport<T>(SqlReceiveEndpointContext context, Uri publishAddress)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [SqlReceiveEndpointContext](../masstransit-sqltransport/sqlreceiveendpointcontext)<br/>

`publishAddress` Uri<br/>

#### Returns

[Task\<ISendTransport\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **NormalizeAddress(Uri)**

```csharp
Uri NormalizeAddress(Uri address)
```

#### Parameters

`address` Uri<br/>

#### Returns

Uri<br/>
