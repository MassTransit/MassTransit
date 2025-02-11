---

title: ConditionalActivityBinder<TSaga, TMessage>

---

# ConditionalActivityBinder\<TSaga, TMessage\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class ConditionalActivityBinder<TSaga, TMessage> : IActivityBinder<TSaga>
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConditionalActivityBinder\<TSaga, TMessage\>](../masstransit-sagastatemachine/conditionalactivitybinder-2)<br/>
Implements [IActivityBinder\<TSaga\>](../masstransit-sagastatemachine/iactivitybinder-1)

## Properties

### **Event**

```csharp
public Event Event { get; }
```

#### Property Value

[Event](../../masstransit-abstractions/masstransit/event)<br/>

## Constructors

### **ConditionalActivityBinder(Event, StateMachineCondition\<TSaga, TMessage\>, EventActivities\<TSaga\>, EventActivities\<TSaga\>)**

```csharp
public ConditionalActivityBinder(Event event, StateMachineCondition<TSaga, TMessage> condition, EventActivities<TSaga> thenActivities, EventActivities<TSaga> elseActivities)
```

#### Parameters

`event` [Event](../../masstransit-abstractions/masstransit/event)<br/>

`condition` [StateMachineCondition\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/statemachinecondition-2)<br/>

`thenActivities` [EventActivities\<TSaga\>](../masstransit/eventactivities-1)<br/>

`elseActivities` [EventActivities\<TSaga\>](../masstransit/eventactivities-1)<br/>

### **ConditionalActivityBinder(Event, StateMachineAsyncCondition\<TSaga, TMessage\>, EventActivities\<TSaga\>, EventActivities\<TSaga\>)**

```csharp
public ConditionalActivityBinder(Event event, StateMachineAsyncCondition<TSaga, TMessage> condition, EventActivities<TSaga> thenActivities, EventActivities<TSaga> elseActivities)
```

#### Parameters

`event` [Event](../../masstransit-abstractions/masstransit/event)<br/>

`condition` [StateMachineAsyncCondition\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/statemachineasynccondition-2)<br/>

`thenActivities` [EventActivities\<TSaga\>](../masstransit/eventactivities-1)<br/>

`elseActivities` [EventActivities\<TSaga\>](../masstransit/eventactivities-1)<br/>

## Methods

### **IsStateTransitionEvent(State)**

```csharp
public bool IsStateTransitionEvent(State state)
```

#### Parameters

`state` [State](../../masstransit-abstractions/masstransit/state)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Bind(State\<TSaga\>)**

```csharp
public void Bind(State<TSaga> state)
```

#### Parameters

`state` [State\<TSaga\>](../../masstransit-abstractions/masstransit/state-1)<br/>

### **Bind(IBehaviorBuilder\<TSaga\>)**

```csharp
public void Bind(IBehaviorBuilder<TSaga> builder)
```

#### Parameters

`builder` [IBehaviorBuilder\<TSaga\>](../masstransit-sagastatemachine/ibehaviorbuilder-1)<br/>
