---

title: EventContext<T>

---

# EventContext\<T\>

Namespace: MassTransit.Contracts

```csharp
public interface EventContext<T> : EventContext, PipeContext
```

#### Type Parameters

`T`<br/>

Implements [EventContext](../masstransit-contracts/eventcontext), [PipeContext](../masstransit/pipecontext)

## Properties

### **Event**

The event object

```csharp
public abstract T Event { get; }
```

#### Property Value

T<br/>
