---

title: CompensateActivityContext<TActivity, TLog>

---

# CompensateActivityContext\<TActivity, TLog\>

Namespace: MassTransit

```csharp
public interface CompensateActivityContext<TActivity, TLog> : CompensateActivityContext<TLog>, CompensateContext<TLog>, CompensateContext, CourierContext, ConsumeContext<RoutingSlip>, ConsumeContext, PipeContext, MessageContext, IPublishEndpoint, IPublishObserverConnector, ISendEndpointProvider, ISendObserverConnector
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

Implements [CompensateActivityContext\<TLog\>](../masstransit/compensateactivitycontext-1), [CompensateContext\<TLog\>](../masstransit/compensatecontext-1), [CompensateContext](../masstransit/compensatecontext), [CourierContext](../masstransit/couriercontext), [ConsumeContext\<RoutingSlip\>](../masstransit/consumecontext-1), [ConsumeContext](../masstransit/consumecontext), [PipeContext](../masstransit/pipecontext), [MessageContext](../masstransit/messagecontext), [IPublishEndpoint](../masstransit/ipublishendpoint), [IPublishObserverConnector](../masstransit/ipublishobserverconnector), [ISendEndpointProvider](../masstransit/isendendpointprovider), [ISendObserverConnector](../masstransit/isendobserverconnector)

## Properties

### **Activity**

The activity that was created/used for this compensation

```csharp
public abstract TActivity Activity { get; }
```

#### Property Value

TActivity<br/>
