---

title: SqlSendTransportProvider

---

# SqlSendTransportProvider

Namespace: MassTransit.SqlTransport

```csharp
public class SqlSendTransportProvider : ISendTransportProvider
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SqlSendTransportProvider](../masstransit-sqltransport/sqlsendtransportprovider)<br/>
Implements [ISendTransportProvider](../../masstransit-abstractions/masstransit-transports/isendtransportprovider)

## Constructors

### **SqlSendTransportProvider(IConnectionContextSupervisor, SqlReceiveEndpointContext)**

```csharp
public SqlSendTransportProvider(IConnectionContextSupervisor connectionContextSupervisor, SqlReceiveEndpointContext context)
```

#### Parameters

`connectionContextSupervisor` [IConnectionContextSupervisor](../masstransit-sqltransport/iconnectioncontextsupervisor)<br/>

`context` [SqlReceiveEndpointContext](../masstransit-sqltransport/sqlreceiveendpointcontext)<br/>

## Methods

### **NormalizeAddress(Uri)**

```csharp
public Uri NormalizeAddress(Uri address)
```

#### Parameters

`address` Uri<br/>

#### Returns

Uri<br/>

### **GetSendTransport(Uri)**

```csharp
public Task<ISendTransport> GetSendTransport(Uri address)
```

#### Parameters

`address` Uri<br/>

#### Returns

[Task\<ISendTransport\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
