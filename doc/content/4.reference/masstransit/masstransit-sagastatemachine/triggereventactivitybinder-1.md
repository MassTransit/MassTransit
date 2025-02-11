---

title: TriggerEventActivityBinder<TInstance>

---

# TriggerEventActivityBinder\<TInstance\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class TriggerEventActivityBinder<TInstance> : EventActivityBinder<TInstance>, EventActivities<TInstance>
```

#### Type Parameters

`TInstance`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TriggerEventActivityBinder\<TInstance\>](../masstransit-sagastatemachine/triggereventactivitybinder-1)<br/>
Implements [EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1), [EventActivities\<TInstance\>](../masstransit/eventactivities-1)

## Properties

### **Event**

```csharp
public Event Event { get; }
```

#### Property Value

[Event](../../masstransit-abstractions/masstransit/event)<br/>

## Constructors

### **TriggerEventActivityBinder(StateMachine\<TInstance\>, Event, IActivityBinder`1[])**

```csharp
public TriggerEventActivityBinder(StateMachine<TInstance> machine, Event event, IActivityBinder`1[] activities)
```

#### Parameters

`machine` [StateMachine\<TInstance\>](../../masstransit-abstractions/masstransit/statemachine-1)<br/>

`event` [Event](../../masstransit-abstractions/masstransit/event)<br/>

`activities` [IActivityBinder`1[]](../masstransit-sagastatemachine/iactivitybinder-1)<br/>

### **TriggerEventActivityBinder(StateMachine\<TInstance\>, Event, StateMachineCondition\<TInstance\>, IActivityBinder`1[])**

```csharp
public TriggerEventActivityBinder(StateMachine<TInstance> machine, Event event, StateMachineCondition<TInstance> filter, IActivityBinder`1[] activities)
```

#### Parameters

`machine` [StateMachine\<TInstance\>](../../masstransit-abstractions/masstransit/statemachine-1)<br/>

`event` [Event](../../masstransit-abstractions/masstransit/event)<br/>

`filter` [StateMachineCondition\<TInstance\>](../../masstransit-abstractions/masstransit/statemachinecondition-1)<br/>

`activities` [IActivityBinder`1[]](../masstransit-sagastatemachine/iactivitybinder-1)<br/>

## Methods

### **Retry(Action\<IRetryConfigurator\>, Func\<EventActivityBinder\<TInstance\>, EventActivityBinder\<TInstance\>\>)**

```csharp
public EventActivityBinder<TInstance> Retry(Action<IRetryConfigurator> configure, Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
```

#### Parameters

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

`activityCallback` [Func\<EventActivityBinder\<TInstance\>, EventActivityBinder\<TInstance\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

### **IfElse(StateMachineCondition\<TInstance\>, Func\<EventActivityBinder\<TInstance\>, EventActivityBinder\<TInstance\>\>, Func\<EventActivityBinder\<TInstance\>, EventActivityBinder\<TInstance\>\>)**

```csharp
public EventActivityBinder<TInstance> IfElse(StateMachineCondition<TInstance> condition, Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> thenActivityCallback, Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> elseActivityCallback)
```

#### Parameters

`condition` [StateMachineCondition\<TInstance\>](../../masstransit-abstractions/masstransit/statemachinecondition-1)<br/>

`thenActivityCallback` [Func\<EventActivityBinder\<TInstance\>, EventActivityBinder\<TInstance\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`elseActivityCallback` [Func\<EventActivityBinder\<TInstance\>, EventActivityBinder\<TInstance\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

### **IfElseAsync(StateMachineAsyncCondition\<TInstance\>, Func\<EventActivityBinder\<TInstance\>, EventActivityBinder\<TInstance\>\>, Func\<EventActivityBinder\<TInstance\>, EventActivityBinder\<TInstance\>\>)**

```csharp
public EventActivityBinder<TInstance> IfElseAsync(StateMachineAsyncCondition<TInstance> condition, Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> thenActivityCallback, Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> elseActivityCallback)
```

#### Parameters

`condition` [StateMachineAsyncCondition\<TInstance\>](../../masstransit-abstractions/masstransit/statemachineasynccondition-1)<br/>

`thenActivityCallback` [Func\<EventActivityBinder\<TInstance\>, EventActivityBinder\<TInstance\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`elseActivityCallback` [Func\<EventActivityBinder\<TInstance\>, EventActivityBinder\<TInstance\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

### **GetStateActivityBinders()**

```csharp
public IEnumerable<IActivityBinder<TInstance>> GetStateActivityBinders()
```

#### Returns

[IEnumerable\<IActivityBinder\<TInstance\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
