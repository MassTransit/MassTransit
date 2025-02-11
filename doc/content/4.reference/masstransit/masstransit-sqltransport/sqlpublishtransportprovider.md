---

title: SqlPublishTransportProvider

---

# SqlPublishTransportProvider

Namespace: MassTransit.SqlTransport

```csharp
public class SqlPublishTransportProvider : IPublishTransportProvider
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SqlPublishTransportProvider](../masstransit-sqltransport/sqlpublishtransportprovider)<br/>
Implements [IPublishTransportProvider](../../masstransit-abstractions/masstransit-transports/ipublishtransportprovider)

## Constructors

### **SqlPublishTransportProvider(IConnectionContextSupervisor, SqlReceiveEndpointContext)**

```csharp
public SqlPublishTransportProvider(IConnectionContextSupervisor connectionContextSupervisor, SqlReceiveEndpointContext context)
```

#### Parameters

`connectionContextSupervisor` [IConnectionContextSupervisor](../masstransit-sqltransport/iconnectioncontextsupervisor)<br/>

`context` [SqlReceiveEndpointContext](../masstransit-sqltransport/sqlreceiveendpointcontext)<br/>

## Methods

### **GetPublishTransport\<T\>(Uri)**

```csharp
public Task<ISendTransport> GetPublishTransport<T>(Uri publishAddress)
```

#### Type Parameters

`T`<br/>

#### Parameters

`publishAddress` Uri<br/>

#### Returns

[Task\<ISendTransport\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
