---

title: ConditionalExceptionActivityBinder<TInstance, TData, TException>

---

# ConditionalExceptionActivityBinder\<TInstance, TData, TException\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class ConditionalExceptionActivityBinder<TInstance, TData, TException> : IActivityBinder<TInstance>
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TException`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConditionalExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit-sagastatemachine/conditionalexceptionactivitybinder-3)<br/>
Implements [IActivityBinder\<TInstance\>](../masstransit-sagastatemachine/iactivitybinder-1)

## Properties

### **Event**

```csharp
public Event Event { get; }
```

#### Property Value

[Event](../../masstransit-abstractions/masstransit/event)<br/>

## Constructors

### **ConditionalExceptionActivityBinder(Event, StateMachineExceptionCondition\<TInstance, TData, TException\>, EventActivities\<TInstance\>, EventActivities\<TInstance\>)**

```csharp
public ConditionalExceptionActivityBinder(Event event, StateMachineExceptionCondition<TInstance, TData, TException> condition, EventActivities<TInstance> thenActivities, EventActivities<TInstance> elseActivities)
```

#### Parameters

`event` [Event](../../masstransit-abstractions/masstransit/event)<br/>

`condition` [StateMachineExceptionCondition\<TInstance, TData, TException\>](../../masstransit-abstractions/masstransit/statemachineexceptioncondition-3)<br/>

`thenActivities` [EventActivities\<TInstance\>](../masstransit/eventactivities-1)<br/>

`elseActivities` [EventActivities\<TInstance\>](../masstransit/eventactivities-1)<br/>

### **ConditionalExceptionActivityBinder(Event, StateMachineAsyncExceptionCondition\<TInstance, TData, TException\>, EventActivities\<TInstance\>, EventActivities\<TInstance\>)**

```csharp
public ConditionalExceptionActivityBinder(Event event, StateMachineAsyncExceptionCondition<TInstance, TData, TException> condition, EventActivities<TInstance> thenActivities, EventActivities<TInstance> elseActivities)
```

#### Parameters

`event` [Event](../../masstransit-abstractions/masstransit/event)<br/>

`condition` [StateMachineAsyncExceptionCondition\<TInstance, TData, TException\>](../../masstransit-abstractions/masstransit/statemachineasyncexceptioncondition-3)<br/>

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
