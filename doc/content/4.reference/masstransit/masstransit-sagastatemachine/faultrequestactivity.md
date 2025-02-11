---

title: FaultRequestActivity

---

# FaultRequestActivity

Namespace: MassTransit.SagaStateMachine

```csharp
public class FaultRequestActivity : IStateMachineActivity<RequestState, RequestFaulted>, IStateMachineActivity, IVisitable, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FaultRequestActivity](../masstransit-sagastatemachine/faultrequestactivity)<br/>
Implements [IStateMachineActivity\<RequestState, RequestFaulted\>](../../masstransit-abstractions/masstransit/istatemachineactivity-2), [IStateMachineActivity](../../masstransit-abstractions/masstransit/istatemachineactivity), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **FaultRequestActivity()**

```csharp
public FaultRequestActivity()
```

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

### **Execute(BehaviorContext\<RequestState, RequestFaulted\>, IBehavior\<RequestState, RequestFaulted\>)**

```csharp
public Task Execute(BehaviorContext<RequestState, RequestFaulted> context, IBehavior<RequestState, RequestFaulted> next)
```

#### Parameters

`context` [BehaviorContext\<RequestState, RequestFaulted\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

`next` [IBehavior\<RequestState, RequestFaulted\>](../../masstransit-abstractions/masstransit/ibehavior-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Faulted\<TException\>(BehaviorExceptionContext\<RequestState, RequestFaulted, TException\>, IBehavior\<RequestState, RequestFaulted\>)**

```csharp
public Task Faulted<TException>(BehaviorExceptionContext<RequestState, RequestFaulted, TException> context, IBehavior<RequestState, RequestFaulted> next)
```

#### Type Parameters

`TException`<br/>

#### Parameters

`context` [BehaviorExceptionContext\<RequestState, RequestFaulted, TException\>](../../masstransit-abstractions/masstransit/behaviorexceptioncontext-3)<br/>

`next` [IBehavior\<RequestState, RequestFaulted\>](../../masstransit-abstractions/masstransit/ibehavior-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
