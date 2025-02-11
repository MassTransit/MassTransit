---

title: IReceiveTransport

---

# IReceiveTransport

Namespace: MassTransit.Transports

```csharp
public interface IReceiveTransport : IReceiveObserverConnector, IPublishObserverConnector, ISendObserverConnector, IReceiveTransportObserverConnector, IProbeSite
```

Implements [IReceiveObserverConnector](../../masstransit-abstractions/masstransit/ireceiveobserverconnector), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [IReceiveTransportObserverConnector](../../masstransit-abstractions/masstransit/ireceivetransportobserverconnector), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Methods

### **Start()**

Start receiving on a transport, sending messages to the specified pipe.

```csharp
ReceiveTransportHandle Start()
```

#### Returns

[ReceiveTransportHandle](../masstransit-transports/receivetransporthandle)<br/>
