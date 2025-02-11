---

title: BehaviorContext<TSaga, TMessage>

---

# BehaviorContext\<TSaga, TMessage\>

Namespace: MassTransit

A behavior context include an event context, along with the behavior for a state instance.

```csharp
public interface BehaviorContext<TSaga, TMessage> : SagaConsumeContext<TSaga, TMessage>, SagaConsumeContext<TSaga>, ConsumeContext, PipeContext, MessageContext, IPublishEndpoint, IPublishObserverConnector, ISendEndpointProvider, ISendObserverConnector, ConsumeContext<TMessage>, BehaviorContext<TSaga>
```

#### Type Parameters

`TSaga`<br/>
The instance type

`TMessage`<br/>
The event type

Implements [SagaConsumeContext\<TSaga, TMessage\>](../masstransit/sagaconsumecontext-2), [SagaConsumeContext\<TSaga\>](../masstransit/sagaconsumecontext-1), [ConsumeContext](../masstransit/consumecontext), [PipeContext](../masstransit/pipecontext), [MessageContext](../masstransit/messagecontext), [IPublishEndpoint](../masstransit/ipublishendpoint), [IPublishObserverConnector](../masstransit/ipublishobserverconnector), [ISendEndpointProvider](../masstransit/isendendpointprovider), [ISendObserverConnector](../masstransit/isendobserverconnector), [ConsumeContext\<TMessage\>](../masstransit/consumecontext-1), [BehaviorContext\<TSaga\>](../masstransit/behaviorcontext-1)

## Properties

### **Event**

```csharp
public abstract Event<TMessage> Event { get; }
```

#### Property Value

[Event\<TMessage\>](../masstransit/event-1)<br/>

### **Data**

#### Caution

Use Message instead. Visit https://masstransit.io/obsolete for details.

---

```csharp
public abstract TMessage Data { get; }
```

#### Property Value

TMessage<br/>

## Methods

### **Init\<T\>(Object)**

```csharp
Task<SendTuple<T>> Init<T>(object values)
```

#### Type Parameters

`T`<br/>

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[Task\<SendTuple\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
