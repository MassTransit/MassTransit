---

title: FaultedScheduleActivity<TSaga, TData, TException, TMessage>

---

# FaultedScheduleActivity\<TSaga, TData, TException, TMessage\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class FaultedScheduleActivity<TSaga, TData, TException, TMessage> : IStateMachineActivity<TSaga, TData>, IStateMachineActivity, IVisitable, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FaultedScheduleActivity\<TSaga, TData, TException, TMessage\>](../masstransit-sagastatemachine/faultedscheduleactivity-4)<br/>
Implements [IStateMachineActivity\<TSaga, TData\>](../../masstransit-abstractions/masstransit/istatemachineactivity-2), [IStateMachineActivity](../../masstransit-abstractions/masstransit/istatemachineactivity), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **FaultedScheduleActivity(Schedule\<TSaga, TMessage\>, ScheduleTimeExceptionProvider\<TSaga, TData, TException\>, ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TData, TException\>, TMessage\>)**

```csharp
public FaultedScheduleActivity(Schedule<TSaga, TMessage> schedule, ScheduleTimeExceptionProvider<TSaga, TData, TException> timeProvider, ContextMessageFactory<BehaviorExceptionContext<TSaga, TData, TException>, TMessage> messageFactory)
```

#### Parameters

`schedule` [Schedule\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

`timeProvider` [ScheduleTimeExceptionProvider\<TSaga, TData, TException\>](../../masstransit-abstractions/masstransit/scheduletimeexceptionprovider-3)<br/>

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
