---

title: DataBehavior<TSaga, TMessage>

---

# DataBehavior\<TSaga, TMessage\>

Namespace: MassTransit.SagaStateMachine

Splits apart the data from the behavior so it can be invoked properly.

```csharp
public class DataBehavior<TSaga, TMessage> : IBehavior<TSaga, TMessage>, IVisitable, IProbeSite
```

#### Type Parameters

`TSaga`<br/>
The instance type

`TMessage`<br/>
The event data type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DataBehavior\<TSaga, TMessage\>](../masstransit-sagastatemachine/databehavior-2)<br/>
Implements [IBehavior\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/ibehavior-2), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **DataBehavior(IBehavior\<TSaga\>)**

```csharp
public DataBehavior(IBehavior<TSaga> behavior)
```

#### Parameters

`behavior` [IBehavior\<TSaga\>](../../masstransit-abstractions/masstransit/ibehavior-1)<br/>

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
