---

title: IRoutingSlipSendEndpointTarget

---

# IRoutingSlipSendEndpointTarget

Namespace: MassTransit.Courier

```csharp
public interface IRoutingSlipSendEndpointTarget
```

## Methods

### **AddSubscription(Uri, RoutingSlipEvents, RoutingSlipEventContents, String, MessageEnvelope)**

Adds a custom subscription message to the routing slip which is sent at the specified events

```csharp
void AddSubscription(Uri address, RoutingSlipEvents events, RoutingSlipEventContents contents, string activityName, MessageEnvelope message)
```

#### Parameters

`address` Uri<br/>
The destination address where the events are sent

`events` [RoutingSlipEvents](../../masstransit-abstractions/masstransit-courier-contracts/routingslipevents)<br/>
The events to include in the subscription

`contents` [RoutingSlipEventContents](../../masstransit-abstractions/masstransit-courier-contracts/routingslipeventcontents)<br/>
The contents of the routing slip event

`activityName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`message` [MessageEnvelope](../../masstransit-abstractions/masstransit-serialization/messageenvelope)<br/>
The custom message to be sent
