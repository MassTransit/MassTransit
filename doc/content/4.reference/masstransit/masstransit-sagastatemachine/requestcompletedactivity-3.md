---

title: RequestCompletedActivity<TSaga, TMessage, TResponse>

---

# RequestCompletedActivity\<TSaga, TMessage, TResponse\>

Namespace: MassTransit.SagaStateMachine

Publishes the  event, used by the request state machine to track
 pending requests for a saga instance.

```csharp
public class RequestCompletedActivity<TSaga, TMessage, TResponse> : IStateMachineActivity<TSaga, TMessage>, IStateMachineActivity, IVisitable, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

`TResponse`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RequestCompletedActivity\<TSaga, TMessage, TResponse\>](../masstransit-sagastatemachine/requestcompletedactivity-3)<br/>
Implements [IStateMachineActivity\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/istatemachineactivity-2), [IStateMachineActivity](../../masstransit-abstractions/masstransit/istatemachineactivity), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **RequestCompletedActivity(AsyncEventMessageFactory\<TSaga, TMessage, TResponse\>)**

```csharp
public RequestCompletedActivity(AsyncEventMessageFactory<TSaga, TMessage, TResponse> messageFactory)
```

#### Parameters

`messageFactory` [AsyncEventMessageFactory\<TSaga, TMessage, TResponse\>](../../masstransit-abstractions/masstransit/asynceventmessagefactory-3)<br/>

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **Accept(StateMachineVisitor)**

```csharp
public void Accept(StateMachineVisitor visitor)
```

#### Parameters

`visitor` [StateMachineVisitor](../../masstransit-abstractions/masstransit/statemachinevisitor)<br/>

### **Execute(BehaviorContext\<TSaga, TMessage\>, IBehavior\<TSaga, TMessage\>)**

```csharp
public Task Execute(BehaviorContext<TSaga, TMessage> context, IBehavior<TSaga, TMessage> next)
```

#### Parameters

`context` [BehaviorContext\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

`next` [IBehavior\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/ibehavior-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Faulted\<TException\>(BehaviorExceptionContext\<TSaga, TMessage, TException\>, IBehavior\<TSaga, TMessage\>)**

```csharp
public Task Faulted<TException>(BehaviorExceptionContext<TSaga, TMessage, TException> context, IBehavior<TSaga, TMessage> next)
```

#### Type Parameters

`TException`<br/>

#### Parameters

`context` [BehaviorExceptionContext\<TSaga, TMessage, TException\>](../../masstransit-abstractions/masstransit/behaviorexceptioncontext-3)<br/>

`next` [IBehavior\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/ibehavior-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
