---

title: InMemoryReceiveTransport

---

# InMemoryReceiveTransport

Namespace: MassTransit.InMemoryTransport

Support in-memory message queue that is not durable, but supports parallel delivery of messages
 based on TPL usage.

```csharp
public class InMemoryReceiveTransport : IReceiveTransport, IReceiveObserverConnector, IPublishObserverConnector, ISendObserverConnector, IReceiveTransportObserverConnector, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InMemoryReceiveTransport](../masstransit-inmemorytransport/inmemoryreceivetransport)<br/>
Implements [IReceiveTransport](../masstransit-transports/ireceivetransport), [IReceiveObserverConnector](../../masstransit-abstractions/masstransit/ireceiveobserverconnector), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [IReceiveTransportObserverConnector](../../masstransit-abstractions/masstransit/ireceivetransportobserverconnector), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **InMemoryReceiveTransport(InMemoryReceiveEndpointContext, String)**

```csharp
public InMemoryReceiveTransport(InMemoryReceiveEndpointContext context, string queueName)
```

#### Parameters

`context` [InMemoryReceiveEndpointContext](../masstransit-inmemorytransport/inmemoryreceiveendpointcontext)<br/>

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
