---

title: ConfigureSqlTopologyFilter<TSettings>

---

# ConfigureSqlTopologyFilter\<TSettings\>

Namespace: MassTransit.SqlTransport.Middleware

Configures the broker with the supplied topology once the model is created, to ensure
 that the exchanges, queues, and bindings for the model are properly configured in SQS.

```csharp
public class ConfigureSqlTopologyFilter<TSettings> : IFilter<ClientContext>, IProbeSite
```

#### Type Parameters

`TSettings`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConfigureSqlTopologyFilter\<TSettings\>](../masstransit-sqltransport-middleware/configuresqltopologyfilter-1)<br/>
Implements [IFilter\<ClientContext\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ConfigureSqlTopologyFilter(TSettings, BrokerTopology, SqlReceiveEndpointContext)**

```csharp
public ConfigureSqlTopologyFilter(TSettings settings, BrokerTopology brokerTopology, SqlReceiveEndpointContext context)
```

#### Parameters

`settings` TSettings<br/>

`brokerTopology` [BrokerTopology](../masstransit-sqltransport-topology/brokertopology)<br/>

`context` [SqlReceiveEndpointContext](../masstransit-sqltransport/sqlreceiveendpointcontext)<br/>

## Methods

### **Send(ClientContext, IPipe\<ClientContext\>)**

```csharp
public Task Send(ClientContext context, IPipe<ClientContext> next)
```

#### Parameters

`context` [ClientContext](../masstransit-sqltransport/clientcontext)<br/>

`next` [IPipe\<ClientContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
