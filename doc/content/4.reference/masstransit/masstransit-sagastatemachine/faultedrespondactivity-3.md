---

title: FaultedRespondActivity<TSaga, TException, TMessage>

---

# FaultedRespondActivity\<TSaga, TException, TMessage\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class FaultedRespondActivity<TSaga, TException, TMessage> : IStateMachineActivity<TSaga>, IStateMachineActivity, IVisitable, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FaultedRespondActivity\<TSaga, TException, TMessage\>](../masstransit-sagastatemachine/faultedrespondactivity-3)<br/>
Implements [IStateMachineActivity\<TSaga\>](../../masstransit-abstractions/masstransit/istatemachineactivity-1), [IStateMachineActivity](../../masstransit-abstractions/masstransit/istatemachineactivity), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **FaultedRespondActivity(ContextMessageFactory\<BehaviorExceptionContext\<TSaga, TException\>, TMessage\>)**

```csharp
public FaultedRespondActivity(ContextMessageFactory<BehaviorExceptionContext<TSaga, TException>, TMessage> messageFactory)
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

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
