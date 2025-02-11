---

title: Behavior<TSaga, TMessage>

---

# Behavior\<TSaga, TMessage\>

Namespace: Automatonymous

#### Caution

Deprecated, use IBehavior instead

---

A behavior is a chain of activities invoked by a state

```csharp
public interface Behavior<TSaga, TMessage> : IBehavior<TSaga, TMessage>, IVisitable, IProbeSite
```

#### Type Parameters

`TSaga`<br/>
The state type

`TMessage`<br/>
The data type of the behavior

Implements [IBehavior\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/ibehavior-2), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)
