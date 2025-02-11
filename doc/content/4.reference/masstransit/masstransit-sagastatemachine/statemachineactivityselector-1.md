---

title: StateMachineActivitySelector<TSaga>

---

# StateMachineActivitySelector\<TSaga\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class StateMachineActivitySelector<TSaga> : IStateMachineActivitySelector<TSaga>
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [StateMachineActivitySelector\<TSaga\>](../masstransit-sagastatemachine/statemachineactivityselector-1)<br/>
Implements [IStateMachineActivitySelector\<TSaga\>](../masstransit/istatemachineactivityselector-1)

## Constructors

### **StateMachineActivitySelector(EventActivityBinder\<TSaga\>)**

```csharp
public StateMachineActivitySelector(EventActivityBinder<TSaga> binder)
```

#### Parameters

`binder` [EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

## Methods

### **OfType\<TActivity\>()**

```csharp
public EventActivityBinder<TSaga> OfType<TActivity>()
```

#### Type Parameters

`TActivity`<br/>

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>
