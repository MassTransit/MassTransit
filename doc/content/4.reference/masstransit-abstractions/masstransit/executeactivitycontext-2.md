---

title: ExecuteActivityContext<TActivity, TArguments>

---

# ExecuteActivityContext\<TActivity, TArguments\>

Namespace: MassTransit

An activity and execution context combined into a single container from the factory

```csharp
public interface ExecuteActivityContext<TActivity, TArguments> : ExecuteActivityContext<TArguments>, ExecuteContext<TArguments>, ExecuteContext, CourierContext, ConsumeContext<RoutingSlip>, ConsumeContext, PipeContext, MessageContext, IPublishEndpoint, IPublishObserverConnector, ISendEndpointProvider, ISendObserverConnector
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

Implements [ExecuteActivityContext\<TArguments\>](../masstransit/executeactivitycontext-1), [ExecuteContext\<TArguments\>](../masstransit/executecontext-1), [ExecuteContext](../masstransit/executecontext), [CourierContext](../masstransit/couriercontext), [ConsumeContext\<RoutingSlip\>](../masstransit/consumecontext-1), [ConsumeContext](../masstransit/consumecontext), [PipeContext](../masstransit/pipecontext), [MessageContext](../masstransit/messagecontext), [IPublishEndpoint](../masstransit/ipublishendpoint), [IPublishObserverConnector](../masstransit/ipublishobserverconnector), [ISendEndpointProvider](../masstransit/isendendpointprovider), [ISendObserverConnector](../masstransit/isendobserverconnector)

## Properties

### **Activity**

The activity that was created/used for this execution

```csharp
public abstract TActivity Activity { get; }
```

#### Property Value

TActivity<br/>
