---

title: CompensateContext<TLog>

---

# CompensateContext\<TLog\>

Namespace: MassTransit

```csharp
public interface CompensateContext<TLog> : CompensateContext, CourierContext, ConsumeContext<RoutingSlip>, ConsumeContext, PipeContext, MessageContext, IPublishEndpoint, IPublishObserverConnector, ISendEndpointProvider, ISendObserverConnector
```

#### Type Parameters

`TLog`<br/>

Implements [CompensateContext](../masstransit/compensatecontext), [CourierContext](../masstransit/couriercontext), [ConsumeContext\<RoutingSlip\>](../masstransit/consumecontext-1), [ConsumeContext](../masstransit/consumecontext), [PipeContext](../masstransit/pipecontext), [MessageContext](../masstransit/messagecontext), [IPublishEndpoint](../masstransit/ipublishendpoint), [IPublishObserverConnector](../masstransit/ipublishobserverconnector), [ISendEndpointProvider](../masstransit/isendendpointprovider), [ISendObserverConnector](../masstransit/isendobserverconnector)

## Properties

### **Log**

The execution log from the activity execution

```csharp
public abstract TLog Log { get; }
```

#### Property Value

TLog<br/>

## Methods

### **CreateActivityContext\<TActivity\>(TActivity)**

```csharp
CompensateActivityContext<TActivity, TLog> CreateActivityContext<TActivity>(TActivity activity)
```

#### Type Parameters

`TActivity`<br/>

#### Parameters

`activity` TActivity<br/>

#### Returns

[CompensateActivityContext\<TActivity, TLog\>](../masstransit/compensateactivitycontext-2)<br/>
