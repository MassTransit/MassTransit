---

title: StateMachineSagaMessageFilter<TInstance, TMessage>

---

# StateMachineSagaMessageFilter\<TInstance, TMessage\>

Namespace: MassTransit.Middleware

Dispatches the ConsumeContext to the consumer method for the specified message type

```csharp
public class StateMachineSagaMessageFilter<TInstance, TMessage> : ISagaMessageFilter<TInstance, TMessage>, IFilter<SagaConsumeContext<TInstance, TMessage>>, IProbeSite
```

#### Type Parameters

`TInstance`<br/>
The consumer type

`TMessage`<br/>
The message type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [StateMachineSagaMessageFilter\<TInstance, TMessage\>](../masstransit-middleware/statemachinesagamessagefilter-2)<br/>
Implements [ISagaMessageFilter\<TInstance, TMessage\>](../masstransit-middleware/isagamessagefilter-2), [IFilter\<SagaConsumeContext\<TInstance, TMessage\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **StateMachineSagaMessageFilter(SagaStateMachine\<TInstance\>, Event\<TMessage\>)**

```csharp
public StateMachineSagaMessageFilter(SagaStateMachine<TInstance> machine, Event<TMessage> event)
```

#### Parameters

`machine` [SagaStateMachine\<TInstance\>](../../masstransit-abstractions/masstransit/sagastatemachine-1)<br/>

`event` [Event\<TMessage\>](../../masstransit-abstractions/masstransit/event-1)<br/>

## Methods

### **Send(SagaConsumeContext\<TInstance, TMessage\>, IPipe\<SagaConsumeContext\<TInstance, TMessage\>\>)**

```csharp
public Task Send(SagaConsumeContext<TInstance, TMessage> context, IPipe<SagaConsumeContext<TInstance, TMessage>> next)
```

#### Parameters

`context` [SagaConsumeContext\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/sagaconsumecontext-2)<br/>

`next` [IPipe\<SagaConsumeContext\<TInstance, TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
