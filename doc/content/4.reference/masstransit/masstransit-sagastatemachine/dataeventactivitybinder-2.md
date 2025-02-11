---

title: DataEventActivityBinder<TInstance, TData>

---

# DataEventActivityBinder\<TInstance, TData\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class DataEventActivityBinder<TInstance, TData> : EventActivityBinder<TInstance, TData>, EventActivities<TInstance>
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DataEventActivityBinder\<TInstance, TData\>](../masstransit-sagastatemachine/dataeventactivitybinder-2)<br/>
Implements [EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2), [EventActivities\<TInstance\>](../masstransit/eventactivities-1)

## Constructors

### **DataEventActivityBinder(StateMachine\<TInstance\>, Event\<TData\>, IActivityBinder`1[])**

```csharp
public DataEventActivityBinder(StateMachine<TInstance> machine, Event<TData> event, IActivityBinder`1[] activities)
```

#### Parameters

`machine` [StateMachine\<TInstance\>](../../masstransit-abstractions/masstransit/statemachine-1)<br/>

`event` [Event\<TData\>](../../masstransit-abstractions/masstransit/event-1)<br/>

`activities` [IActivityBinder`1[]](../masstransit-sagastatemachine/iactivitybinder-1)<br/>

### **DataEventActivityBinder(StateMachine\<TInstance\>, Event\<TData\>, StateMachineCondition\<TInstance, TData\>, IActivityBinder`1[])**

```csharp
public DataEventActivityBinder(StateMachine<TInstance> machine, Event<TData> event, StateMachineCondition<TInstance, TData> filter, IActivityBinder`1[] activities)
```

#### Parameters

`machine` [StateMachine\<TInstance\>](../../masstransit-abstractions/masstransit/statemachine-1)<br/>

`event` [Event\<TData\>](../../masstransit-abstractions/masstransit/event-1)<br/>

`filter` [StateMachineCondition\<TInstance, TData\>](../../masstransit-abstractions/masstransit/statemachinecondition-2)<br/>

`activities` [IActivityBinder`1[]](../masstransit-sagastatemachine/iactivitybinder-1)<br/>

## Methods

### **Retry(Action\<IRetryConfigurator\>, Func\<EventActivityBinder\<TInstance, TData\>, EventActivityBinder\<TInstance, TData\>\>)**

```csharp
public EventActivityBinder<TInstance, TData> Retry(Action<IRetryConfigurator> configure, Func<EventActivityBinder<TInstance, TData>, EventActivityBinder<TInstance, TData>> activityCallback)
```

#### Parameters

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

`activityCallback` [Func\<EventActivityBinder\<TInstance, TData\>, EventActivityBinder\<TInstance, TData\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **IfElse(StateMachineCondition\<TInstance, TData\>, Func\<EventActivityBinder\<TInstance, TData\>, EventActivityBinder\<TInstance, TData\>\>, Func\<EventActivityBinder\<TInstance, TData\>, EventActivityBinder\<TInstance, TData\>\>)**

```csharp
public EventActivityBinder<TInstance, TData> IfElse(StateMachineCondition<TInstance, TData> condition, Func<EventActivityBinder<TInstance, TData>, EventActivityBinder<TInstance, TData>> thenActivityCallback, Func<EventActivityBinder<TInstance, TData>, EventActivityBinder<TInstance, TData>> elseActivityCallback)
```

#### Parameters

`condition` [StateMachineCondition\<TInstance, TData\>](../../masstransit-abstractions/masstransit/statemachinecondition-2)<br/>

`thenActivityCallback` [Func\<EventActivityBinder\<TInstance, TData\>, EventActivityBinder\<TInstance, TData\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`elseActivityCallback` [Func\<EventActivityBinder\<TInstance, TData\>, EventActivityBinder\<TInstance, TData\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **IfElseAsync(StateMachineAsyncCondition\<TInstance, TData\>, Func\<EventActivityBinder\<TInstance, TData\>, EventActivityBinder\<TInstance, TData\>\>, Func\<EventActivityBinder\<TInstance, TData\>, EventActivityBinder\<TInstance, TData\>\>)**

```csharp
public EventActivityBinder<TInstance, TData> IfElseAsync(StateMachineAsyncCondition<TInstance, TData> condition, Func<EventActivityBinder<TInstance, TData>, EventActivityBinder<TInstance, TData>> thenActivityCallback, Func<EventActivityBinder<TInstance, TData>, EventActivityBinder<TInstance, TData>> elseActivityCallback)
```

#### Parameters

`condition` [StateMachineAsyncCondition\<TInstance, TData\>](../../masstransit-abstractions/masstransit/statemachineasynccondition-2)<br/>

`thenActivityCallback` [Func\<EventActivityBinder\<TInstance, TData\>, EventActivityBinder\<TInstance, TData\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`elseActivityCallback` [Func\<EventActivityBinder\<TInstance, TData\>, EventActivityBinder\<TInstance, TData\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[EventActivityBinder\<TInstance, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **GetStateActivityBinders()**

```csharp
public IEnumerable<IActivityBinder<TInstance>> GetStateActivityBinders()
```

#### Returns

[IEnumerable\<IActivityBinder\<TInstance\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
