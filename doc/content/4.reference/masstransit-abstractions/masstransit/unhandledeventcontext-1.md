---

title: UnhandledEventContext<TSaga>

---

# UnhandledEventContext\<TSaga\>

Namespace: MassTransit

The context of an unhandled event in the state machine

```csharp
public interface UnhandledEventContext<TSaga> : BehaviorContext<TSaga>, SagaConsumeContext<TSaga>, ConsumeContext, PipeContext, MessageContext, IPublishEndpoint, IPublishObserverConnector, ISendEndpointProvider, ISendObserverConnector
```

#### Type Parameters

`TSaga`<br/>

Implements [BehaviorContext\<TSaga\>](../masstransit/behaviorcontext-1), [SagaConsumeContext\<TSaga\>](../masstransit/sagaconsumecontext-1), [ConsumeContext](../masstransit/consumecontext), [PipeContext](../masstransit/pipecontext), [MessageContext](../masstransit/messagecontext), [IPublishEndpoint](../masstransit/ipublishendpoint), [IPublishObserverConnector](../masstransit/ipublishobserverconnector), [ISendEndpointProvider](../masstransit/isendendpointprovider), [ISendObserverConnector](../masstransit/isendobserverconnector)

## Properties

### **CurrentState**

The current state of the state machine

```csharp
public abstract State CurrentState { get; }
```

#### Property Value

[State](../masstransit/state)<br/>

## Methods

### **Ignore()**

Returns a Task that ignores the unhandled event

```csharp
Task Ignore()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Throw()**

Returns a thrown exception task for the unhandled event

```csharp
Task Throw()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
