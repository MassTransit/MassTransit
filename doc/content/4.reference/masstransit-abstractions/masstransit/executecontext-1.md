---

title: ExecuteContext<TArguments>

---

# ExecuteContext\<TArguments\>

Namespace: MassTransit

```csharp
public interface ExecuteContext<TArguments> : ExecuteContext, CourierContext, ConsumeContext<RoutingSlip>, ConsumeContext, PipeContext, MessageContext, IPublishEndpoint, IPublishObserverConnector, ISendEndpointProvider, ISendObserverConnector
```

#### Type Parameters

`TArguments`<br/>

Implements [ExecuteContext](../masstransit/executecontext), [CourierContext](../masstransit/couriercontext), [ConsumeContext\<RoutingSlip\>](../masstransit/consumecontext-1), [ConsumeContext](../masstransit/consumecontext), [PipeContext](../masstransit/pipecontext), [MessageContext](../masstransit/messagecontext), [IPublishEndpoint](../masstransit/ipublishendpoint), [IPublishObserverConnector](../masstransit/ipublishobserverconnector), [ISendEndpointProvider](../masstransit/isendendpointprovider), [ISendObserverConnector](../masstransit/isendobserverconnector)

## Properties

### **Arguments**

The arguments from the routing slip for this activity

```csharp
public abstract TArguments Arguments { get; }
```

#### Property Value

TArguments<br/>

## Methods

### **CreateActivityContext\<TActivity\>(TActivity)**

```csharp
ExecuteActivityContext<TActivity, TArguments> CreateActivityContext<TActivity>(TActivity activity)
```

#### Type Parameters

`TActivity`<br/>

#### Parameters

`activity` TActivity<br/>

#### Returns

[ExecuteActivityContext\<TActivity, TArguments\>](../masstransit/executeactivitycontext-2)<br/>
