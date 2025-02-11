---

title: ConditionalExceptionActivityBinder<TInstance, TException>

---

# ConditionalExceptionActivityBinder\<TInstance, TException\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class ConditionalExceptionActivityBinder<TInstance, TException> : IActivityBinder<TInstance>
```

#### Type Parameters

`TInstance`<br/>

`TException`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConditionalExceptionActivityBinder\<TInstance, TException\>](../masstransit-sagastatemachine/conditionalexceptionactivitybinder-2)<br/>
Implements [IActivityBinder\<TInstance\>](../masstransit-sagastatemachine/iactivitybinder-1)

## Properties

### **Event**

```csharp
public Event Event { get; }
```

#### Property Value

[Event](../../masstransit-abstractions/masstransit/event)<br/>

## Constructors

### **ConditionalExceptionActivityBinder(Event, StateMachineExceptionCondition\<TInstance, TException\>, EventActivities\<TInstance\>, EventActivities\<TInstance\>)**

```csharp
public ConditionalExceptionActivityBinder(Event event, StateMachineExceptionCondition<TInstance, TException> condition, EventActivities<TInstance> thenActivities, EventActivities<TInstance> elseActivities)
```

#### Parameters

`event` [Event](../../masstransit-abstractions/masstransit/event)<br/>

`condition` [StateMachineExceptionCondition\<TInstance, TException\>](../../masstransit-abstractions/masstransit/statemachineexceptioncondition-2)<br/>

`thenActivities` [EventActivities\<TInstance\>](../masstransit/eventactivities-1)<br/>

`elseActivities` [EventActivities\<TInstance\>](../masstransit/eventactivities-1)<br/>

### **ConditionalExceptionActivityBinder(Event, StateMachineAsyncExceptionCondition\<TInstance, TException\>, EventActivities\<TInstance\>, EventActivities\<TInstance\>)**

```csharp
public ConditionalExceptionActivityBinder(Event event, StateMachineAsyncExceptionCondition<TInstance, TException> condition, EventActivities<TInstance> thenActivities, EventActivities<TInstance> elseActivities)
```

#### Parameters

`event` [Event](../../masstransit-abstractions/masstransit/event)<br/>

`condition` [StateMachineAsyncExceptionCondition\<TInstance, TException\>](../../masstransit-abstractions/masstransit/statemachineasyncexceptioncondition-2)<br/>

`thenActivities` [EventActivities\<TInstance\>](../masstransit/eventactivities-1)<br/>

`elseActivities` [EventActivities\<TInstance\>](../masstransit/eventactivities-1)<br/>

## Methods

### **IsStateTransitionEvent(State)**

```csharp
public bool IsStateTransitionEvent(State state)
```

#### Parameters

`state` [State](../../masstransit-abstractions/masstransit/state)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Bind(State\<TInstance\>)**

```csharp
public void Bind(State<TInstance> state)
```

#### Parameters

`state` [State\<TInstance\>](../../masstransit-abstractions/masstransit/state-1)<br/>

### **Bind(IBehaviorBuilder\<TInstance\>)**

```csharp
public void Bind(IBehaviorBuilder<TInstance> builder)
```

#### Parameters

`builder` [IBehaviorBuilder\<TInstance\>](../masstransit-sagastatemachine/ibehaviorbuilder-1)<br/>
