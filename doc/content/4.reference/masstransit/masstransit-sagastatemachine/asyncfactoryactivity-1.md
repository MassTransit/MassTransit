---

title: AsyncFactoryActivity<TSaga>

---

# AsyncFactoryActivity\<TSaga\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class AsyncFactoryActivity<TSaga> : IStateMachineActivity<TSaga>, IStateMachineActivity, IVisitable, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AsyncFactoryActivity\<TSaga\>](../masstransit-sagastatemachine/asyncfactoryactivity-1)<br/>
Implements [IStateMachineActivity\<TSaga\>](../../masstransit-abstractions/masstransit/istatemachineactivity-1), [IStateMachineActivity](../../masstransit-abstractions/masstransit/istatemachineactivity), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **AsyncFactoryActivity(Func\<BehaviorContext\<TSaga\>, Task\<IStateMachineActivity\<TSaga\>\>\>)**

```csharp
public AsyncFactoryActivity(Func<BehaviorContext<TSaga>, Task<IStateMachineActivity<TSaga>>> activityFactory)
```

#### Parameters

`activityFactory` [Func\<BehaviorContext\<TSaga\>, Task\<IStateMachineActivity\<TSaga\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

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
