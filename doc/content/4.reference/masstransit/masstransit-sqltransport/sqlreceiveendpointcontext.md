---

title: SqlReceiveEndpointContext

---

# SqlReceiveEndpointContext

Namespace: MassTransit.SqlTransport

```csharp
public interface SqlReceiveEndpointContext : ReceiveEndpointContext, PipeContext, ISendObserverConnector, IPublishObserverConnector, IReceiveTransportObserverConnector, IReceiveObserverConnector, IReceiveEndpointObserverConnector, IProbeSite
```

Implements [ReceiveEndpointContext](../masstransit-transports/receiveendpointcontext), [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [IReceiveTransportObserverConnector](../../masstransit-abstractions/masstransit/ireceivetransportobserverconnector), [IReceiveObserverConnector](../../masstransit-abstractions/masstransit/ireceiveobserverconnector), [IReceiveEndpointObserverConnector](../../masstransit-abstractions/masstransit/ireceiveendpointobserverconnector), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Properties

### **ClientContextSupervisor**

```csharp
public abstract IClientContextSupervisor ClientContextSupervisor { get; }
```

#### Property Value

[IClientContextSupervisor](../masstransit-sqltransport/iclientcontextsupervisor)<br/>

### **BrokerTopology**

```csharp
public abstract BrokerTopology BrokerTopology { get; }
```

#### Property Value

[BrokerTopology](../masstransit-sqltransport-topology/brokertopology)<br/>
