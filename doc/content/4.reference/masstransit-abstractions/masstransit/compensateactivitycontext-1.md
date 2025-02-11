---

title: CompensateActivityContext<TLog>

---

# CompensateActivityContext\<TLog\>

Namespace: MassTransit

```csharp
public interface CompensateActivityContext<TLog> : CompensateContext<TLog>, CompensateContext, CourierContext, ConsumeContext<RoutingSlip>, ConsumeContext, PipeContext, MessageContext, IPublishEndpoint, IPublishObserverConnector, ISendEndpointProvider, ISendObserverConnector
```

#### Type Parameters

`TLog`<br/>

Implements [CompensateContext\<TLog\>](../masstransit/compensatecontext-1), [CompensateContext](../masstransit/compensatecontext), [CourierContext](../masstransit/couriercontext), [ConsumeContext\<RoutingSlip\>](../masstransit/consumecontext-1), [ConsumeContext](../masstransit/consumecontext), [PipeContext](../masstransit/pipecontext), [MessageContext](../masstransit/messagecontext), [IPublishEndpoint](../masstransit/ipublishendpoint), [IPublishObserverConnector](../masstransit/ipublishobserverconnector), [ISendEndpointProvider](../masstransit/isendendpointprovider), [ISendObserverConnector](../masstransit/isendobserverconnector)
