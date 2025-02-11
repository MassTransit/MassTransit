---

title: BehaviorExceptionContext<TSaga, TMessage, TException>

---

# BehaviorExceptionContext\<TSaga, TMessage, TException\>

Namespace: MassTransit

An exceptional behavior context

```csharp
public interface BehaviorExceptionContext<TSaga, TMessage, TException> : BehaviorContext<TSaga, TMessage>, SagaConsumeContext<TSaga, TMessage>, SagaConsumeContext<TSaga>, ConsumeContext, PipeContext, MessageContext, IPublishEndpoint, IPublishObserverConnector, ISendEndpointProvider, ISendObserverConnector, ConsumeContext<TMessage>, BehaviorContext<TSaga>, BehaviorExceptionContext<TSaga, TException>
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

`TException`<br/>

Implements [BehaviorContext\<TSaga, TMessage\>](../masstransit/behaviorcontext-2), [SagaConsumeContext\<TSaga, TMessage\>](../masstransit/sagaconsumecontext-2), [SagaConsumeContext\<TSaga\>](../masstransit/sagaconsumecontext-1), [ConsumeContext](../masstransit/consumecontext), [PipeContext](../masstransit/pipecontext), [MessageContext](../masstransit/messagecontext), [IPublishEndpoint](../masstransit/ipublishendpoint), [IPublishObserverConnector](../masstransit/ipublishobserverconnector), [ISendEndpointProvider](../masstransit/isendendpointprovider), [ISendObserverConnector](../masstransit/isendobserverconnector), [ConsumeContext\<TMessage\>](../masstransit/consumecontext-1), [BehaviorContext\<TSaga\>](../masstransit/behaviorcontext-1), [BehaviorExceptionContext\<TSaga, TException\>](../masstransit/behaviorexceptioncontext-2)

## Methods

### **CreateProxy\<T\>(Event\<T\>, T)**

Return a proxy of the current behavior context with the specified event and data

```csharp
BehaviorExceptionContext<TSaga, T, TException> CreateProxy<T>(Event<T> event, T data)
```

#### Type Parameters

`T`<br/>
The data type

#### Parameters

`event` [Event\<T\>](../masstransit/event-1)<br/>
The event for the new context

`data` T<br/>
The data for the event

#### Returns

[BehaviorExceptionContext\<TSaga, T, TException\>](../masstransit/behaviorexceptioncontext-3)<br/>
