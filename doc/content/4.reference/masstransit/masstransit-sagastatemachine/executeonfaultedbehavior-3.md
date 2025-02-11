---

title: ExecuteOnFaultedBehavior<TSaga, TMessage, TException>

---

# ExecuteOnFaultedBehavior\<TSaga, TMessage, TException\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class ExecuteOnFaultedBehavior<TSaga, TMessage, TException> : IBehavior<TSaga>, IVisitable, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

`TException`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExecuteOnFaultedBehavior\<TSaga, TMessage, TException\>](../masstransit-sagastatemachine/executeonfaultedbehavior-3)<br/>
Implements [IBehavior\<TSaga\>](../../masstransit-abstractions/masstransit/ibehavior-1), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ExecuteOnFaultedBehavior(IBehavior\<TSaga, TMessage\>, BehaviorExceptionContext\<TSaga, TMessage, TException\>)**

```csharp
public ExecuteOnFaultedBehavior(IBehavior<TSaga, TMessage> next, BehaviorExceptionContext<TSaga, TMessage, TException> context)
```

#### Parameters

`next` [IBehavior\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/ibehavior-2)<br/>

`context` [BehaviorExceptionContext\<TSaga, TMessage, TException\>](../../masstransit-abstractions/masstransit/behaviorexceptioncontext-3)<br/>

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
