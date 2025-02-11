---

title: StateMachineActivitySelector<TSaga, TMessage>

---

# StateMachineActivitySelector\<TSaga, TMessage\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class StateMachineActivitySelector<TSaga, TMessage> : IStateMachineActivitySelector<TSaga, TMessage>
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [StateMachineActivitySelector\<TSaga, TMessage\>](../masstransit-sagastatemachine/statemachineactivityselector-2)<br/>
Implements [IStateMachineActivitySelector\<TSaga, TMessage\>](../masstransit/istatemachineactivityselector-2)

## Constructors

### **StateMachineActivitySelector(EventActivityBinder\<TSaga, TMessage\>)**

```csharp
public StateMachineActivitySelector(EventActivityBinder<TSaga, TMessage> binder)
```

#### Parameters

`binder` [EventActivityBinder\<TSaga, TMessage\>](../masstransit/eventactivitybinder-2)<br/>

## Methods

### **OfType\<TActivity\>()**

```csharp
public EventActivityBinder<TSaga, TMessage> OfType<TActivity>()
```

#### Type Parameters

`TActivity`<br/>

#### Returns

[EventActivityBinder\<TSaga, TMessage\>](../masstransit/eventactivitybinder-2)<br/>

### **OfInstanceType\<TActivity\>()**

```csharp
public EventActivityBinder<TSaga, TMessage> OfInstanceType<TActivity>()
```

#### Type Parameters

`TActivity`<br/>

#### Returns

[EventActivityBinder\<TSaga, TMessage\>](../masstransit/eventactivitybinder-2)<br/>
