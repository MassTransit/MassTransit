---

title: Behavior<TSaga>

---

# Behavior\<TSaga\>

Namespace: Automatonymous

#### Caution

Deprecated, use IBehavior instead

---

A behavior is a chain of activities invoked by a state

```csharp
public interface Behavior<TSaga> : IBehavior<TSaga>, IVisitable, IProbeSite
```

#### Type Parameters

`TSaga`<br/>
The state type

Implements [IBehavior\<TSaga\>](../../masstransit-abstractions/masstransit/ibehavior-1), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)
