---

title: WidenBehavior<TSaga, TMessage>

---

# WidenBehavior\<TSaga, TMessage\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class WidenBehavior<TSaga, TMessage> : IBehavior<TSaga>, IVisitable, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [WidenBehavior\<TSaga, TMessage\>](../masstransit-sagastatemachine/widenbehavior-2)<br/>
Implements [IBehavior\<TSaga\>](../../masstransit-abstractions/masstransit/ibehavior-1), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **WidenBehavior(IBehavior\<TSaga, TMessage\>, BehaviorContext\<TSaga, TMessage\>)**

```csharp
public WidenBehavior(IBehavior<TSaga, TMessage> next, BehaviorContext<TSaga, TMessage> context)
```

#### Parameters

`next` [IBehavior\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/ibehavior-2)<br/>

`context` [BehaviorContext\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

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
