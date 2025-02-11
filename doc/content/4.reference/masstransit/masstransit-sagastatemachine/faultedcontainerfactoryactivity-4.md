---

title: FaultedContainerFactoryActivity<TSaga, TMessage, TException, TActivity>

---

# FaultedContainerFactoryActivity\<TSaga, TMessage, TException, TActivity\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class FaultedContainerFactoryActivity<TSaga, TMessage, TException, TActivity> : IStateMachineActivity<TSaga, TMessage>, IStateMachineActivity, IVisitable, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

`TException`<br/>

`TActivity`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FaultedContainerFactoryActivity\<TSaga, TMessage, TException, TActivity\>](../masstransit-sagastatemachine/faultedcontainerfactoryactivity-4)<br/>
Implements [IStateMachineActivity\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/istatemachineactivity-2), [IStateMachineActivity](../../masstransit-abstractions/masstransit/istatemachineactivity), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **FaultedContainerFactoryActivity()**

```csharp
public FaultedContainerFactoryActivity()
```

## Methods

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

### **Faulted\<T\>(BehaviorExceptionContext\<TSaga, TMessage, T\>, IBehavior\<TSaga, TMessage\>)**

```csharp
public Task Faulted<T>(BehaviorExceptionContext<TSaga, TMessage, T> context, IBehavior<TSaga, TMessage> next)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [BehaviorExceptionContext\<TSaga, TMessage, T\>](../../masstransit-abstractions/masstransit/behaviorexceptioncontext-3)<br/>

`next` [IBehavior\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/ibehavior-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Accept(StateMachineVisitor)**

```csharp
public void Accept(StateMachineVisitor visitor)
```

#### Parameters

`visitor` [StateMachineVisitor](../../masstransit-abstractions/masstransit/statemachinevisitor)<br/>
