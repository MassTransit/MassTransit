---

title: FaultedPublishActivity<TSaga, TData, TException, TMessage>

---

# FaultedPublishActivity\<TSaga, TData, TException, TMessage\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class FaultedPublishActivity<TSaga, TData, TException, TMessage> : IStateMachineActivity<TSaga, TData>, IStateMachineActivity, IVisitable, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FaultedPublishActivity\<TSaga, TData, TException, TMessage\>](../masstransit-sagastatemachine/faultedpublishactivity-4)<br/>
Implements [IStateMachineActivity\<TSaga, TData\>](../../masstransit-abstractions/masstransit/istatemachineactivity-2), [IStateMachineActivity](../../masstransit-abstractions/masstransit/istatemachineactivity), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **FaultedPublishActivity(ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TData, TException\>, TMessage\>)**

```csharp
public FaultedPublishActivity(ContextMessageFactory<BehaviorExceptionContext<TSaga, TData, TException>, TMessage> messageFactory)
```

#### Parameters

`messageFactory` [ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TData, TException\>, TMessage\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

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

### **Execute(BehaviorContext\<TSaga, TData\>, IBehavior\<TSaga, TData\>)**

```csharp
public Task Execute(BehaviorContext<TSaga, TData> context, IBehavior<TSaga, TData> next)
```

#### Parameters

`context` [BehaviorContext\<TSaga, TData\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

`next` [IBehavior\<TSaga, TData\>](../../masstransit-abstractions/masstransit/ibehavior-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Faulted\<T\>(BehaviorExceptionContext\<TSaga, TData, T\>, IBehavior\<TSaga, TData\>)**

```csharp
public Task Faulted<T>(BehaviorExceptionContext<TSaga, TData, T> context, IBehavior<TSaga, TData> next)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [BehaviorExceptionContext\<TSaga, TData, T\>](../../masstransit-abstractions/masstransit/behaviorexceptioncontext-3)<br/>

`next` [IBehavior\<TSaga, TData\>](../../masstransit-abstractions/masstransit/ibehavior-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
