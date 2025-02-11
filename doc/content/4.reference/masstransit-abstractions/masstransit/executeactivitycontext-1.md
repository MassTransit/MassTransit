---

title: ExecuteActivityContext<TArguments>

---

# ExecuteActivityContext\<TArguments\>

Namespace: MassTransit

```csharp
public interface ExecuteActivityContext<TArguments> : ExecuteContext<TArguments>, ExecuteContext, CourierContext, ConsumeContext<RoutingSlip>, ConsumeContext, PipeContext, MessageContext, IPublishEndpoint, IPublishObserverConnector, ISendEndpointProvider, ISendObserverConnector
```

#### Type Parameters

`TArguments`<br/>

Implements [ExecuteContext\<TArguments\>](../masstransit/executecontext-1), [ExecuteContext](../masstransit/executecontext), [CourierContext](../masstransit/couriercontext), [ConsumeContext\<RoutingSlip\>](../masstransit/consumecontext-1), [ConsumeContext](../masstransit/consumecontext), [PipeContext](../masstransit/pipecontext), [MessageContext](../masstransit/messagecontext), [IPublishEndpoint](../masstransit/ipublishendpoint), [IPublishObserverConnector](../masstransit/ipublishobserverconnector), [ISendEndpointProvider](../masstransit/isendendpointprovider), [ISendObserverConnector](../masstransit/isendobserverconnector)
