---

title: ScheduleActivity<TSaga, TMessage>

---

# ScheduleActivity\<TSaga, TMessage\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class ScheduleActivity<TSaga, TMessage> : IStateMachineActivity<TSaga>, IStateMachineActivity, IVisitable, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScheduleActivity\<TSaga, TMessage\>](../masstransit-sagastatemachine/scheduleactivity-2)<br/>
Implements [IStateMachineActivity\<TSaga\>](../../masstransit-abstractions/masstransit/istatemachineactivity-1), [IStateMachineActivity](../../masstransit-abstractions/masstransit/istatemachineactivity), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ScheduleActivity(Schedule\<TSaga\>, ScheduleTimeProvider\<TSaga\>, ContextMessageFactory\<BehaviorContext\<TSaga\>, TMessage\>)**

```csharp
public ScheduleActivity(Schedule<TSaga> schedule, ScheduleTimeProvider<TSaga> timeProvider, ContextMessageFactory<BehaviorContext<TSaga>, TMessage> messageFactory)
```

#### Parameters

`schedule` [Schedule\<TSaga\>](../../masstransit-abstractions/masstransit/schedule-1)<br/>

`timeProvider` [ScheduleTimeProvider\<TSaga\>](../../masstransit-abstractions/masstransit/scheduletimeprovider-1)<br/>

`messageFactory` [ContextMessageFactory\<BehaviorContext\<TSaga\>, TMessage\>](../masstransit-sagastatemachine/contextmessagefactory-2)<br/>

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
