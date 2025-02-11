---

title: AsyncActivity<TInstance, TData>

---

# AsyncActivity\<TInstance, TData\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class AsyncActivity<TInstance, TData> : IStateMachineActivity<TInstance, TData>, IStateMachineActivity, IVisitable, IProbeSite
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AsyncActivity\<TInstance, TData\>](../masstransit-sagastatemachine/asyncactivity-2)<br/>
Implements [IStateMachineActivity\<TInstance, TData\>](../../masstransit-abstractions/masstransit/istatemachineactivity-2), [IStateMachineActivity](../../masstransit-abstractions/masstransit/istatemachineactivity), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **AsyncActivity(Func\<BehaviorContext\<TInstance, TData\>, Task\>)**

```csharp
public AsyncActivity(Func<BehaviorContext<TInstance, TData>, Task> asyncAction)
```

#### Parameters

`asyncAction` [Func\<BehaviorContext\<TInstance, TData\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

## Methods

### **Accept(StateMachineVisitor)**

```csharp
public void Accept(StateMachineVisitor visitor)
```

#### Parameters

`visitor` [StateMachineVisitor](../../masstransit-abstractions/masstransit/statemachinevisitor)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **Execute(BehaviorContext\<TInstance, TData\>, IBehavior\<TInstance, TData\>)**

```csharp
public Task Execute(BehaviorContext<TInstance, TData> context, IBehavior<TInstance, TData> next)
```

#### Parameters

`context` [BehaviorContext\<TInstance, TData\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

`next` [IBehavior\<TInstance, TData\>](../../masstransit-abstractions/masstransit/ibehavior-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Faulted\<TException\>(BehaviorExceptionContext\<TInstance, TData, TException\>, IBehavior\<TInstance, TData\>)**

```csharp
public Task Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context, IBehavior<TInstance, TData> next)
```

#### Type Parameters

`TException`<br/>

#### Parameters

`context` [BehaviorExceptionContext\<TInstance, TData, TException\>](../../masstransit-abstractions/masstransit/behaviorexceptioncontext-3)<br/>

`next` [IBehavior\<TInstance, TData\>](../../masstransit-abstractions/masstransit/ibehavior-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
