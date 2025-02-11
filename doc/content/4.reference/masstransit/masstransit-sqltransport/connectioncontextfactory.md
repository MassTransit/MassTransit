---

title: ConnectionContextFactory

---

# ConnectionContextFactory

Namespace: MassTransit.SqlTransport

```csharp
public abstract class ConnectionContextFactory : IPipeContextFactory<ConnectionContext>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConnectionContextFactory](../masstransit-sqltransport/connectioncontextfactory)<br/>
Implements [IPipeContextFactory\<ConnectionContext\>](../masstransit-agents/ipipecontextfactory-1)

## Methods

### **CreateContext(ISupervisor)**

```csharp
public IPipeContextAgent<ConnectionContext> CreateContext(ISupervisor supervisor)
```

#### Parameters

`supervisor` [ISupervisor](../../masstransit-abstractions/masstransit/isupervisor)<br/>

#### Returns

[IPipeContextAgent\<ConnectionContext\>](../masstransit-agents/ipipecontextagent-1)<br/>

### **CreateActiveContext(ISupervisor, PipeContextHandle\<ConnectionContext\>, CancellationToken)**

```csharp
public IActivePipeContextAgent<ConnectionContext> CreateActiveContext(ISupervisor supervisor, PipeContextHandle<ConnectionContext> context, CancellationToken cancellationToken)
```

#### Parameters

`supervisor` [ISupervisor](../../masstransit-abstractions/masstransit/isupervisor)<br/>

`context` [PipeContextHandle\<ConnectionContext\>](../masstransit/pipecontexthandle-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IActivePipeContextAgent\<ConnectionContext\>](../masstransit-agents/iactivepipecontextagent-1)<br/>

### **CreateConnection(ITransportSupervisor\<ConnectionContext\>)**

```csharp
protected abstract ConnectionContext CreateConnection(ITransportSupervisor<ConnectionContext> supervisor)
```

#### Parameters

`supervisor` [ITransportSupervisor\<ConnectionContext\>](../../masstransit-abstractions/masstransit-transports/itransportsupervisor-1)<br/>

#### Returns

[ConnectionContext](../masstransit-sqltransport/connectioncontext)<br/>
