---

title: EventActivityBinder<TSaga>

---

# EventActivityBinder\<TSaga\>

Namespace: MassTransit

```csharp
public interface EventActivityBinder<TSaga> : EventActivities<TSaga>
```

#### Type Parameters

`TSaga`<br/>

Implements [EventActivities\<TSaga\>](../masstransit/eventactivities-1)

## Properties

### **StateMachine**

```csharp
public abstract StateMachine<TSaga> StateMachine { get; }
```

#### Property Value

[StateMachine\<TSaga\>](../../masstransit-abstractions/masstransit/statemachine-1)<br/>

### **Event**

```csharp
public abstract Event Event { get; }
```

#### Property Value

[Event](../../masstransit-abstractions/masstransit/event)<br/>

## Methods

### **Add(IStateMachineActivity\<TSaga\>)**

```csharp
EventActivityBinder<TSaga> Add(IStateMachineActivity<TSaga> activity)
```

#### Parameters

`activity` [IStateMachineActivity\<TSaga\>](../../masstransit-abstractions/masstransit/istatemachineactivity-1)<br/>

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **Catch\<T\>(Func\<ExceptionActivityBinder\<TSaga, T\>, ExceptionActivityBinder\<TSaga, T\>\>)**

Catch the exception of type T, and execute the compensation chain

```csharp
EventActivityBinder<TSaga> Catch<T>(Func<ExceptionActivityBinder<TSaga, T>, ExceptionActivityBinder<TSaga, T>> activityCallback)
```

#### Type Parameters

`T`<br/>

#### Parameters

`activityCallback` [Func\<ExceptionActivityBinder\<TSaga, T\>, ExceptionActivityBinder\<TSaga, T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **Retry(Action\<IRetryConfigurator\>, Func\<EventActivityBinder\<TSaga\>, EventActivityBinder\<TSaga\>\>)**

Retry the behavior, using the specified retry policy

```csharp
EventActivityBinder<TSaga> Retry(Action<IRetryConfigurator> configure, Func<EventActivityBinder<TSaga>, EventActivityBinder<TSaga>> activityCallback)
```

#### Parameters

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configures the retry

`activityCallback` [Func\<EventActivityBinder\<TSaga\>, EventActivityBinder\<TSaga\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **If(StateMachineCondition\<TSaga\>, Func\<EventActivityBinder\<TSaga\>, EventActivityBinder\<TSaga\>\>)**

Create a conditional branch of activities for processing

```csharp
EventActivityBinder<TSaga> If(StateMachineCondition<TSaga> condition, Func<EventActivityBinder<TSaga>, EventActivityBinder<TSaga>> activityCallback)
```

#### Parameters

`condition` [StateMachineCondition\<TSaga\>](../../masstransit-abstractions/masstransit/statemachinecondition-1)<br/>

`activityCallback` [Func\<EventActivityBinder\<TSaga\>, EventActivityBinder\<TSaga\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **IfAsync(StateMachineAsyncCondition\<TSaga\>, Func\<EventActivityBinder\<TSaga\>, EventActivityBinder\<TSaga\>\>)**

Create a conditional branch of activities for processing

```csharp
EventActivityBinder<TSaga> IfAsync(StateMachineAsyncCondition<TSaga> condition, Func<EventActivityBinder<TSaga>, EventActivityBinder<TSaga>> activityCallback)
```

#### Parameters

`condition` [StateMachineAsyncCondition\<TSaga\>](../../masstransit-abstractions/masstransit/statemachineasynccondition-1)<br/>

`activityCallback` [Func\<EventActivityBinder\<TSaga\>, EventActivityBinder\<TSaga\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **IfElse(StateMachineCondition\<TSaga\>, Func\<EventActivityBinder\<TSaga\>, EventActivityBinder\<TSaga\>\>, Func\<EventActivityBinder\<TSaga\>, EventActivityBinder\<TSaga\>\>)**

Create a conditional branch of activities for processing

```csharp
EventActivityBinder<TSaga> IfElse(StateMachineCondition<TSaga> condition, Func<EventActivityBinder<TSaga>, EventActivityBinder<TSaga>> thenActivityCallback, Func<EventActivityBinder<TSaga>, EventActivityBinder<TSaga>> elseActivityCallback)
```

#### Parameters

`condition` [StateMachineCondition\<TSaga\>](../../masstransit-abstractions/masstransit/statemachinecondition-1)<br/>

`thenActivityCallback` [Func\<EventActivityBinder\<TSaga\>, EventActivityBinder\<TSaga\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`elseActivityCallback` [Func\<EventActivityBinder\<TSaga\>, EventActivityBinder\<TSaga\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **IfElseAsync(StateMachineAsyncCondition\<TSaga\>, Func\<EventActivityBinder\<TSaga\>, EventActivityBinder\<TSaga\>\>, Func\<EventActivityBinder\<TSaga\>, EventActivityBinder\<TSaga\>\>)**

Create a conditional branch of activities for processing

```csharp
EventActivityBinder<TSaga> IfElseAsync(StateMachineAsyncCondition<TSaga> condition, Func<EventActivityBinder<TSaga>, EventActivityBinder<TSaga>> thenActivityCallback, Func<EventActivityBinder<TSaga>, EventActivityBinder<TSaga>> elseActivityCallback)
```

#### Parameters

`condition` [StateMachineAsyncCondition\<TSaga\>](../../masstransit-abstractions/masstransit/statemachineasynccondition-1)<br/>

`thenActivityCallback` [Func\<EventActivityBinder\<TSaga\>, EventActivityBinder\<TSaga\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`elseActivityCallback` [Func\<EventActivityBinder\<TSaga\>, EventActivityBinder\<TSaga\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>
