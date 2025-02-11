---

title: FactoryActivity<TSaga>

---

# FactoryActivity\<TSaga\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class FactoryActivity<TSaga> : IStateMachineActivity<TSaga>, IStateMachineActivity, IVisitable, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FactoryActivity\<TSaga\>](../masstransit-sagastatemachine/factoryactivity-1)<br/>
Implements [IStateMachineActivity\<TSaga\>](../../masstransit-abstractions/masstransit/istatemachineactivity-1), [IStateMachineActivity](../../masstransit-abstractions/masstransit/istatemachineactivity), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **FactoryActivity(Func\<BehaviorContext\<TSaga\>, IStateMachineActivity\<TSaga\>\>)**

```csharp
public FactoryActivity(Func<BehaviorContext<TSaga>, IStateMachineActivity<TSaga>> activityFactory)
```

#### Parameters

`activityFactory` [Func\<BehaviorContext\<TSaga\>, IStateMachineActivity\<TSaga\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

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
