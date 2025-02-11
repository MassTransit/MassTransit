---

title: Activity<TSaga>

---

# Activity\<TSaga\>

Namespace: Automatonymous

#### Caution

Deprecated, use IStateMachineActivity<TSaga> instead

---

An activity is part of a behavior that is executed in order

```csharp
public interface Activity<TSaga> : IStateMachineActivity<TSaga>, IStateMachineActivity, IVisitable, IProbeSite, Activity
```

#### Type Parameters

`TSaga`<br/>

Implements [IStateMachineActivity\<TSaga\>](../../masstransit-abstractions/masstransit/istatemachineactivity-1), [IStateMachineActivity](../../masstransit-abstractions/masstransit/istatemachineactivity), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [Activity](../automatonymous/activity)
