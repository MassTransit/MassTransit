---

title: InMemoryReceiveEndpointContext

---

# InMemoryReceiveEndpointContext

Namespace: MassTransit.InMemoryTransport

```csharp
public interface InMemoryReceiveEndpointContext : ReceiveEndpointContext, PipeContext, ISendObserverConnector, IPublishObserverConnector, IReceiveTransportObserverConnector, IReceiveObserverConnector, IReceiveEndpointObserverConnector, IProbeSite
```

Implements [ReceiveEndpointContext](../masstransit-transports/receiveendpointcontext), [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [IReceiveTransportObserverConnector](../../masstransit-abstractions/masstransit/ireceivetransportobserverconnector), [IReceiveObserverConnector](../../masstransit-abstractions/masstransit/ireceiveobserverconnector), [IReceiveEndpointObserverConnector](../../masstransit-abstractions/masstransit/ireceiveendpointobserverconnector), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Properties

### **Send**

```csharp
public abstract ISendTopology Send { get; }
```

#### Property Value

[ISendTopology](../../masstransit-abstractions/masstransit/isendtopology)<br/>

### **MessageFabric**

```csharp
public abstract IMessageFabric<InMemoryTransportContext, InMemoryTransportMessage> MessageFabric { get; }
```

#### Property Value

[IMessageFabric\<InMemoryTransportContext, InMemoryTransportMessage\>](../masstransit-transports-fabric/imessagefabric-2)<br/>

### **TransportContext**

```csharp
public abstract InMemoryTransportContext TransportContext { get; }
```

#### Property Value

[InMemoryTransportContext](../masstransit-inmemorytransport/inmemorytransportcontext)<br/>

## Methods

### **ConfigureTopology()**

```csharp
void ConfigureTopology()
```
