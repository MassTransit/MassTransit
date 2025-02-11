---

title: ExecuteOnFaultedBehavior<TSaga, TException>

---

# ExecuteOnFaultedBehavior\<TSaga, TException\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class ExecuteOnFaultedBehavior<TSaga, TException> : IBehavior<TSaga>, IVisitable, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

`TException`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExecuteOnFaultedBehavior\<TSaga, TException\>](../masstransit-sagastatemachine/executeonfaultedbehavior-2)<br/>
Implements [IBehavior\<TSaga\>](../../masstransit-abstractions/masstransit/ibehavior-1), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ExecuteOnFaultedBehavior(IBehavior\<TSaga\>, BehaviorExceptionContext\<TSaga, TException\>)**

```csharp
public ExecuteOnFaultedBehavior(IBehavior<TSaga> next, BehaviorExceptionContext<TSaga, TException> context)
```

#### Parameters

`next` [IBehavior\<TSaga\>](../../masstransit-abstractions/masstransit/ibehavior-1)<br/>

`context` [BehaviorExceptionContext\<TSaga, TException\>](../../masstransit-abstractions/masstransit/behaviorexceptioncontext-2)<br/>

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
