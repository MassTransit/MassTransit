---

title: ConditionExceptionActivity<TSaga, TConditionException>

---

# ConditionExceptionActivity\<TSaga, TConditionException\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class ConditionExceptionActivity<TSaga, TConditionException> : IStateMachineActivity<TSaga>, IStateMachineActivity, IVisitable, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

`TConditionException`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConditionExceptionActivity\<TSaga, TConditionException\>](../masstransit-sagastatemachine/conditionexceptionactivity-2)<br/>
Implements [IStateMachineActivity\<TSaga\>](../../masstransit-abstractions/masstransit/istatemachineactivity-1), [IStateMachineActivity](../../masstransit-abstractions/masstransit/istatemachineactivity), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ConditionExceptionActivity(StateMachineAsyncExceptionCondition\<TSaga, TConditionException\>, IBehavior\<TSaga\>, IBehavior\<TSaga\>)**

```csharp
public ConditionExceptionActivity(StateMachineAsyncExceptionCondition<TSaga, TConditionException> condition, IBehavior<TSaga> thenBehavior, IBehavior<TSaga> elseBehavior)
```

#### Parameters

`condition` [StateMachineAsyncExceptionCondition\<TSaga, TConditionException\>](../../masstransit-abstractions/masstransit/statemachineasyncexceptioncondition-2)<br/>

`thenBehavior` [IBehavior\<TSaga\>](../../masstransit-abstractions/masstransit/ibehavior-1)<br/>

`elseBehavior` [IBehavior\<TSaga\>](../../masstransit-abstractions/masstransit/ibehavior-1)<br/>

## Methods

### **Accept(StateMachineVisitor)**

```csharp
public void Accept(StateMachineVisitor visitor)
```

#### Parameters

`visitor` [StateMachineVisitor](../../masstransit-abstractions/masstransit/statemachinevisitor)<br/>

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

### **Faulted\<TException\>(BehaviorExceptionContext\<TSaga, TException\>, IBehavior\<TSaga\>)**

```csharp
public Task Faulted<TException>(BehaviorExceptionContext<TSaga, TException> context, IBehavior<TSaga> next)
```

#### Type Parameters

`TException`<br/>

#### Parameters

`context` [BehaviorExceptionContext\<TSaga, TException\>](../../masstransit-abstractions/masstransit/behaviorexceptioncontext-2)<br/>

`next` [IBehavior\<TSaga\>](../../masstransit-abstractions/masstransit/ibehavior-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Faulted\<T, TException\>(BehaviorExceptionContext\<TSaga, T, TException\>, IBehavior\<TSaga, T\>)**

```csharp
public Task Faulted<T, TException>(BehaviorExceptionContext<TSaga, T, TException> context, IBehavior<TSaga, T> next)
```

#### Type Parameters

`T`<br/>

`TException`<br/>

#### Parameters

`context` [BehaviorExceptionContext\<TSaga, T, TException\>](../../masstransit-abstractions/masstransit/behaviorexceptioncontext-3)<br/>

`next` [IBehavior\<TSaga, T\>](../../masstransit-abstractions/masstransit/ibehavior-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
