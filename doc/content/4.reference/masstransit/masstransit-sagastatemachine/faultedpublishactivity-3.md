---

title: FaultedPublishActivity<TSaga, TException, TMessage>

---

# FaultedPublishActivity\<TSaga, TException, TMessage\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class FaultedPublishActivity<TSaga, TException, TMessage> : IStateMachineActivity<TSaga>, IStateMachineActivity, IVisitable, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FaultedPublishActivity\<TSaga, TException, TMessage\>](../masstransit-sagastatemachine/faultedpublishactivity-3)<br/>
Implements [IStateMachineActivity\<TSaga\>](../../masstransit-abstractions/masstransit/istatemachineactivity-1), [IStateMachineActivity](../../masstransit-abstractions/masstransit/istatemachineactivity), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **FaultedPublishActivity(ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TException\>, TMessage\>)**

```csharp
public FaultedPublishActivity(ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, TMessage> messageFactory)
```

#### Parameters

`messageFactory` [ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TException\>, TMessage\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

## Methods

### **Accept(StateMachineVisitor)**

```csharp
public void Accept(StateMachineVisitor inspector)
```

#### Parameters

`inspector` [StateMachineVisitor](../../masstransit-abstractions/masstransit/statemachinevisitor)<br/>

### **Execute(BehaviorContext\<TSaga\>, IBehavior\<TSaga\>)**

```csharp
public Task Execute(BehaviorContext<TSaga> context, IBehavior<TSaga> next)
```

#### Parameters

`context` [BehaviorContext\<TSaga\>](../../masstransit-abstractions/masstransit/behaviorcontext-1)<br/>

`next` [IBehavior\<TSaga\>](../../masstransit-abstractions/masstransit/ibehavior-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Execute\<T\>(BehaviorContext\<TSaga, T\>, IBehavior\<TSaga, T\>)**

```csharp
public Task Execute<T>(BehaviorContext<TSaga, T> context, IBehavior<TSaga, T> next)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [BehaviorContext\<TSaga, T\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

`next` [IBehavior\<TSaga, T\>](../../masstransit-abstractions/masstransit/ibehavior-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Faulted\<T\>(BehaviorExceptionContext\<TSaga, T\>, IBehavior\<TSaga\>)**

```csharp
public Task Faulted<T>(BehaviorExceptionContext<TSaga, T> context, IBehavior<TSaga> next)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [BehaviorExceptionContext\<TSaga, T\>](../../masstransit-abstractions/masstransit/behaviorexceptioncontext-2)<br/>

`next` [IBehavior\<TSaga\>](../../masstransit-abstractions/masstransit/ibehavior-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Faulted\<T, TOtherException\>(BehaviorExceptionContext\<TSaga, T, TOtherException\>, IBehavior\<TSaga, T\>)**

```csharp
public Task Faulted<T, TOtherException>(BehaviorExceptionContext<TSaga, T, TOtherException> context, IBehavior<TSaga, T> next)
```

#### Type Parameters

`T`<br/>

`TOtherException`<br/>

#### Parameters

`context` [BehaviorExceptionContext\<TSaga, T, TOtherException\>](../../masstransit-abstractions/masstransit/behaviorexceptioncontext-3)<br/>

`next` [IBehavior\<TSaga, T\>](../../masstransit-abstractions/masstransit/ibehavior-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
