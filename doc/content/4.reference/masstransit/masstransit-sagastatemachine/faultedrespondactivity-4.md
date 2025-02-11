---

title: FaultedRespondActivity<TSaga, TData, TException, TMessage>

---

# FaultedRespondActivity\<TSaga, TData, TException, TMessage\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class FaultedRespondActivity<TSaga, TData, TException, TMessage> : IStateMachineActivity<TSaga, TData>, IStateMachineActivity, IVisitable, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

`TData`<br/>

`TException`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FaultedRespondActivity\<TSaga, TData, TException, TMessage\>](../masstransit-sagastatemachine/faultedrespondactivity-4)<br/>
Implements [IStateMachineActivity\<TSaga, TData\>](../../masstransit-abstractions/masstransit/istatemachineactivity-2), [IStateMachineActivity](../../masstransit-abstractions/masstransit/istatemachineactivity), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **FaultedRespondActivity(ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TData, TException\>, TMessage\>)**

```csharp
public FaultedRespondActivity(ContextMessageFactory<BehaviorExceptionContext<TSaga, TData, TException>, TMessage> messageFactory)
```

#### Parameters

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
