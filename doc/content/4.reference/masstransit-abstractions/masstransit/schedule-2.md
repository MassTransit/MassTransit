---

title: Schedule<TSaga, TMessage>

---

# Schedule\<TSaga, TMessage\>

Namespace: MassTransit

Holds the state of a scheduled message

```csharp
public interface Schedule<TSaga, TMessage> : Schedule<TSaga>
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

Implements [Schedule\<TSaga\>](../masstransit/schedule-1)

## Properties

### **Received**

This event is raised when the scheduled message is received. If a previous message
 was rescheduled, this event is filtered so that only the most recently scheduled
 message is allowed.

```csharp
public abstract Event<TMessage> Received { get; set; }
```

#### Property Value

[Event\<TMessage\>](../masstransit/event-1)<br/>

### **AnyReceived**

This event is raised when any message is directed at the state machine, but it is
 not filtered to the currently scheduled event. So outdated or original events may
 be raised.

```csharp
public abstract Event<TMessage> AnyReceived { get; set; }
```

#### Property Value

[Event\<TMessage\>](../masstransit/event-1)<br/>
