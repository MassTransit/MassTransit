---

title: RespondActivity<TSaga, TMessage, T>

---

# RespondActivity\<TSaga, TMessage, T\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class RespondActivity<TSaga, TMessage, T> : IStateMachineActivity<TSaga, TMessage>, IStateMachineActivity, IVisitable, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RespondActivity\<TSaga, TMessage, T\>](../masstransit-sagastatemachine/respondactivity-3)<br/>
Implements [IStateMachineActivity\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/istatemachineactivity-2), [IStateMachineActivity](../../masstransit-abstractions/masstransit/istatemachineactivity), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **RespondActivity(ContextMessageFactory\<BehaviorContext\<TSaga, TMessage\>, T\>)**

```csharp
public RespondActivity(ContextMessageFactory<BehaviorContext<TSaga, TMessage>, T> messageFactory)
```

#### Parameters

`messageFactory` [ContextMessageFactory\<BehaviorContext\<TSaga, TMessage\>, T\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

## Methods

### **Accept(StateMachineVisitor)**

```csharp
public void Accept(StateMachineVisitor inspector)
```

#### Parameters

`inspector` [StateMachineVisitor](../../masstransit-abstractions/masstransit/statemachinevisitor)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

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
