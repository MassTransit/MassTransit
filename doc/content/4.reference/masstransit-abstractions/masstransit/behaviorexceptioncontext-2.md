---

title: BehaviorExceptionContext<TSaga, TException>

---

# BehaviorExceptionContext\<TSaga, TException\>

Namespace: MassTransit

An exceptional behavior context

```csharp
public interface BehaviorExceptionContext<TSaga, TException> : BehaviorContext<TSaga>, SagaConsumeContext<TSaga>, ConsumeContext, PipeContext, MessageContext, IPublishEndpoint, IPublishObserverConnector, ISendEndpointProvider, ISendObserverConnector
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

Implements [BehaviorContext\<TSaga\>](../masstransit/behaviorcontext-1), [SagaConsumeContext\<TSaga\>](../masstransit/sagaconsumecontext-1), [ConsumeContext](../masstransit/consumecontext), [PipeContext](../masstransit/pipecontext), [MessageContext](../masstransit/messagecontext), [IPublishEndpoint](../masstransit/ipublishendpoint), [IPublishObserverConnector](../masstransit/ipublishobserverconnector), [ISendEndpointProvider](../masstransit/isendendpointprovider), [ISendObserverConnector](../masstransit/isendobserverconnector)

## Properties

### **Exception**

```csharp
public abstract TException Exception { get; }
```

#### Property Value

TException<br/>

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
