---

title: CompleteRequestActivity

---

# CompleteRequestActivity

Namespace: MassTransit.SagaStateMachine

```csharp
public class CompleteRequestActivity : IStateMachineActivity<RequestState, RequestCompleted>, IStateMachineActivity, IVisitable, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CompleteRequestActivity](../masstransit-sagastatemachine/completerequestactivity)<br/>
Implements [IStateMachineActivity\<RequestState, RequestCompleted\>](../../masstransit-abstractions/masstransit/istatemachineactivity-2), [IStateMachineActivity](../../masstransit-abstractions/masstransit/istatemachineactivity), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **CompleteRequestActivity()**

```csharp
public CompleteRequestActivity()
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

### **Execute(BehaviorContext\<RequestState, RequestCompleted\>, IBehavior\<RequestState, RequestCompleted\>)**

```csharp
public Task Execute(BehaviorContext<RequestState, RequestCompleted> context, IBehavior<RequestState, RequestCompleted> next)
```

#### Parameters

`context` [BehaviorContext\<RequestState, RequestCompleted\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

`next` [IBehavior\<RequestState, RequestCompleted\>](../../masstransit-abstractions/masstransit/ibehavior-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Faulted\<TException\>(BehaviorExceptionContext\<RequestState, RequestCompleted, TException\>, IBehavior\<RequestState, RequestCompleted\>)**

```csharp
public Task Faulted<TException>(BehaviorExceptionContext<RequestState, RequestCompleted, TException> context, IBehavior<RequestState, RequestCompleted> next)
```

#### Type Parameters

`TException`<br/>

#### Parameters

`context` [BehaviorExceptionContext\<RequestState, RequestCompleted, TException\>](../../masstransit-abstractions/masstransit/behaviorexceptioncontext-3)<br/>

`next` [IBehavior\<RequestState, RequestCompleted\>](../../masstransit-abstractions/masstransit/ibehavior-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
