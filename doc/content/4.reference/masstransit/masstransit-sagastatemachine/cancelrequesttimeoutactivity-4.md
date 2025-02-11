---

title: CancelRequestTimeoutActivity<TSaga, TMessage, TRequest, TResponse>

---

# CancelRequestTimeoutActivity\<TSaga, TMessage, TRequest, TResponse\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class CancelRequestTimeoutActivity<TSaga, TMessage, TRequest, TResponse> : IStateMachineActivity<TSaga, TMessage>, IStateMachineActivity, IVisitable, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

`TRequest`<br/>

`TResponse`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CancelRequestTimeoutActivity\<TSaga, TMessage, TRequest, TResponse\>](../masstransit-sagastatemachine/cancelrequesttimeoutactivity-4)<br/>
Implements [IStateMachineActivity\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/istatemachineactivity-2), [IStateMachineActivity](../../masstransit-abstractions/masstransit/istatemachineactivity), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **CancelRequestTimeoutActivity(Request\<TSaga, TRequest, TResponse\>, Boolean)**

```csharp
public CancelRequestTimeoutActivity(Request<TSaga, TRequest, TResponse> request, bool completed)
```

#### Parameters

`request` [Request\<TSaga, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/request-3)<br/>

`completed` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

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

### **Execute(BehaviorContext\<TSaga, TMessage\>, IBehavior\<TSaga, TMessage\>)**

```csharp
public Task Execute(BehaviorContext<TSaga, TMessage> context, IBehavior<TSaga, TMessage> next)
```

#### Parameters

`context` [BehaviorContext\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

`next` [IBehavior\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/ibehavior-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Faulted\<TException\>(BehaviorExceptionContext\<TSaga, TMessage, TException\>, IBehavior\<TSaga, TMessage\>)**

```csharp
public Task Faulted<TException>(BehaviorExceptionContext<TSaga, TMessage, TException> context, IBehavior<TSaga, TMessage> next)
```

#### Type Parameters

`TException`<br/>

#### Parameters

`context` [BehaviorExceptionContext\<TSaga, TMessage, TException\>](../../masstransit-abstractions/masstransit/behaviorexceptioncontext-3)<br/>

`next` [IBehavior\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/ibehavior-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
