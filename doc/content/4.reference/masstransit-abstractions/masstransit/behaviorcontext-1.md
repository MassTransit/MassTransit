---

title: BehaviorContext<TSaga>

---

# BehaviorContext\<TSaga\>

Namespace: MassTransit

A behavior context is an event context delivered to a behavior, including the state instance

```csharp
public interface BehaviorContext<TSaga> : SagaConsumeContext<TSaga>, ConsumeContext, PipeContext, MessageContext, IPublishEndpoint, IPublishObserverConnector, ISendEndpointProvider, ISendObserverConnector
```

#### Type Parameters

`TSaga`<br/>
The state instance type

Implements [SagaConsumeContext\<TSaga\>](../masstransit/sagaconsumecontext-1), [ConsumeContext](../masstransit/consumecontext), [PipeContext](../masstransit/pipecontext), [MessageContext](../masstransit/messagecontext), [IPublishEndpoint](../masstransit/ipublishendpoint), [IPublishObserverConnector](../masstransit/ipublishobserverconnector), [ISendEndpointProvider](../masstransit/isendendpointprovider), [ISendObserverConnector](../masstransit/isendobserverconnector)

## Properties

### **StateMachine**

```csharp
public abstract StateMachine<TSaga> StateMachine { get; }
```

#### Property Value

[StateMachine\<TSaga\>](../masstransit/statemachine-1)<br/>

### **Event**

```csharp
public abstract Event Event { get; }
```

#### Property Value

[Event](../masstransit/event)<br/>

### **Instance**

#### Caution

Use Saga instead. Visit https://masstransit.io/obsolete for details.

---

```csharp
public abstract TSaga Instance { get; }
```

#### Property Value

TSaga<br/>

## Methods

### **Raise(Event)**

Raise an event on the current instance, pushing the current event on the stack

```csharp
Task Raise(Event event)
```

#### Parameters

`event` [Event](../masstransit/event)<br/>
The event to raise

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
An awaitable Task

### **Raise\<T\>(Event\<T\>, T)**

Raise an event on the current instance, pushing the current event on the stack

```csharp
Task Raise<T>(Event<T> event, T data)
```

#### Type Parameters

`T`<br/>

#### Parameters

`event` [Event\<T\>](../masstransit/event-1)<br/>
The event to raise

`data` T<br/>
THe event data

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
An awaitable Task

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

### **CreateProxy(Event)**

Return a proxy of the current behavior context with the specified event

```csharp
BehaviorContext<TSaga> CreateProxy(Event event)
```

#### Parameters

`event` [Event](../masstransit/event)<br/>
The event for the new context

#### Returns

[BehaviorContext\<TSaga\>](../masstransit/behaviorcontext-1)<br/>

### **CreateProxy\<T\>(Event\<T\>, T)**

Return a proxy of the current behavior context with the specified event and data

```csharp
BehaviorContext<TSaga, T> CreateProxy<T>(Event<T> event, T data)
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

[BehaviorContext\<TSaga, T\>](../masstransit/behaviorcontext-2)<br/>
