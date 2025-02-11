---

title: Activity<TSaga, TMessage>

---

# Activity\<TSaga, TMessage\>

Namespace: Automatonymous

#### Caution

Deprecated, use IStateMachineActivity<TSaga, TMessage> instead

---

```csharp
public interface Activity<TSaga, TMessage> : IStateMachineActivity<TSaga, TMessage>, IStateMachineActivity, IVisitable, IProbeSite, Activity
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

Implements [IStateMachineActivity\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/istatemachineactivity-2), [IStateMachineActivity](../../masstransit-abstractions/masstransit/istatemachineactivity), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [Activity](../automatonymous/activity)
