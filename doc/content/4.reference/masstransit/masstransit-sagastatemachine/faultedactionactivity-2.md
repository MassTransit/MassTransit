---

title: FaultedActionActivity<TSaga, TException>

---

# FaultedActionActivity\<TSaga, TException\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class FaultedActionActivity<TSaga, TException> : IStateMachineActivity<TSaga>, IStateMachineActivity, IVisitable, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FaultedActionActivity\<TSaga, TException\>](../masstransit-sagastatemachine/faultedactionactivity-2)<br/>
Implements [IStateMachineActivity\<TSaga\>](../../masstransit-abstractions/masstransit/istatemachineactivity-1), [IStateMachineActivity](../../masstransit-abstractions/masstransit/istatemachineactivity), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **FaultedActionActivity(Action\<BehaviorExceptionContext\<TSaga, TException\>\>)**

```csharp
public FaultedActionActivity(Action<BehaviorExceptionContext<TSaga, TException>> action)
```

#### Parameters

`action` [Action\<BehaviorExceptionContext\<TSaga, TException\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

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

### **Execute(BehaviorContext\<TSaga\>, IBehavior\<TSaga\>)**

```csharp
public Task Execute(BehaviorContext<TSaga> context, IBehavior<TSaga> next)
```

#### Parameters

`context` [BehaviorContext\<TSaga\>](../../masstransit-abstractions/masstransit/behaviorcontext-1)<br/>

`next` [IBehavior\<TSaga\>](../../masstransit-abstractions/masstransit/ibehavior-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Execute\<TData\>(BehaviorContext\<TSaga, TData\>, IBehavior\<TSaga, TData\>)**

```csharp
public Task Execute<TData>(BehaviorContext<TSaga, TData> context, IBehavior<TSaga, TData> next)
```

#### Type Parameters

`TData`<br/>

#### Parameters

`context` [BehaviorContext\<TSaga, TData\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

`next` [IBehavior\<TSaga, TData\>](../../masstransit-abstractions/masstransit/ibehavior-2)<br/>

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

### **Faulted\<TData, T\>(BehaviorExceptionContext\<TSaga, TData, T\>, IBehavior\<TSaga, TData\>)**

```csharp
public Task Faulted<TData, T>(BehaviorExceptionContext<TSaga, TData, T> context, IBehavior<TSaga, TData> next)
```

#### Type Parameters

`TData`<br/>

`T`<br/>

#### Parameters

`context` [BehaviorExceptionContext\<TSaga, TData, T\>](../../masstransit-abstractions/masstransit/behaviorexceptioncontext-3)<br/>

`next` [IBehavior\<TSaga, TData\>](../../masstransit-abstractions/masstransit/ibehavior-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
