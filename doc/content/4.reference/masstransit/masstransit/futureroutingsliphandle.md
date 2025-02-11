---

title: FutureRoutingSlipHandle

---

# FutureRoutingSlipHandle

Namespace: MassTransit

```csharp
public interface FutureRoutingSlipHandle
```

## Properties

### **Faulted**

The fault state machine event

```csharp
public abstract Event<RoutingSlipFaulted> Faulted { get; }
```

#### Property Value

[Event\<RoutingSlipFaulted\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **Completed**

The response state machine event

```csharp
public abstract Event<RoutingSlipCompleted> Completed { get; }
```

#### Property Value

[Event\<RoutingSlipCompleted\>](../../masstransit-abstractions/masstransit/event-1)<br/>
