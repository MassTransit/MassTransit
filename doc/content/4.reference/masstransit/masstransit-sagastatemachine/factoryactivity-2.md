---

title: FactoryActivity<TSaga, TMessage>

---

# FactoryActivity\<TSaga, TMessage\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class FactoryActivity<TSaga, TMessage> : IStateMachineActivity<TSaga, TMessage>, IStateMachineActivity, IVisitable, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FactoryActivity\<TSaga, TMessage\>](../masstransit-sagastatemachine/factoryactivity-2)<br/>
Implements [IStateMachineActivity\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/istatemachineactivity-2), [IStateMachineActivity](../../masstransit-abstractions/masstransit/istatemachineactivity), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **FactoryActivity(Func\<BehaviorContext\<TSaga, TMessage\>, IStateMachineActivity\<TSaga, TMessage\>\>)**

```csharp
public FactoryActivity(Func<BehaviorContext<TSaga, TMessage>, IStateMachineActivity<TSaga, TMessage>> activityFactory)
```

#### Parameters

`activityFactory` [Func\<BehaviorContext\<TSaga, TMessage\>, IStateMachineActivity\<TSaga, TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

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
