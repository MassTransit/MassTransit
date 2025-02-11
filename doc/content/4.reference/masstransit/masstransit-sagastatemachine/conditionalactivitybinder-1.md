---

title: ConditionalActivityBinder<TSaga>

---

# ConditionalActivityBinder\<TSaga\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class ConditionalActivityBinder<TSaga> : IActivityBinder<TSaga>
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConditionalActivityBinder\<TSaga\>](../masstransit-sagastatemachine/conditionalactivitybinder-1)<br/>
Implements [IActivityBinder\<TSaga\>](../masstransit-sagastatemachine/iactivitybinder-1)

## Properties

### **Event**

```csharp
public Event Event { get; }
```

#### Property Value

[Event](../../masstransit-abstractions/masstransit/event)<br/>

## Constructors

### **ConditionalActivityBinder(Event, StateMachineCondition\<TSaga\>, EventActivities\<TSaga\>, EventActivities\<TSaga\>)**

```csharp
public ConditionalActivityBinder(Event event, StateMachineCondition<TSaga> condition, EventActivities<TSaga> thenActivities, EventActivities<TSaga> elseActivities)
```

#### Parameters

`event` [Event](../../masstransit-abstractions/masstransit/event)<br/>

`condition` [StateMachineCondition\<TSaga\>](../../masstransit-abstractions/masstransit/statemachinecondition-1)<br/>

`thenActivities` [EventActivities\<TSaga\>](../masstransit/eventactivities-1)<br/>

`elseActivities` [EventActivities\<TSaga\>](../masstransit/eventactivities-1)<br/>

### **ConditionalActivityBinder(Event, StateMachineAsyncCondition\<TSaga\>, EventActivities\<TSaga\>, EventActivities\<TSaga\>)**

```csharp
public ConditionalActivityBinder(Event event, StateMachineAsyncCondition<TSaga> condition, EventActivities<TSaga> thenActivities, EventActivities<TSaga> elseActivities)
```

#### Parameters

`event` [Event](../../masstransit-abstractions/masstransit/event)<br/>

`condition` [StateMachineAsyncCondition\<TSaga\>](../../masstransit-abstractions/masstransit/statemachineasynccondition-1)<br/>

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
