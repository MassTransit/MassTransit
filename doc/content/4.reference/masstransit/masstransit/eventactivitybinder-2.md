---

title: EventActivityBinder<TSaga, TMessage>

---

# EventActivityBinder\<TSaga, TMessage\>

Namespace: MassTransit

```csharp
public interface EventActivityBinder<TSaga, TMessage> : EventActivities<TSaga>
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

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
public abstract Event<TMessage> Event { get; }
```

#### Property Value

[Event\<TMessage\>](../../masstransit-abstractions/masstransit/event-1)<br/>

## Methods

### **Add(IStateMachineActivity\<TSaga\>)**

```csharp
EventActivityBinder<TSaga, TMessage> Add(IStateMachineActivity<TSaga> activity)
```

#### Parameters

`activity` [IStateMachineActivity\<TSaga\>](../../masstransit-abstractions/masstransit/istatemachineactivity-1)<br/>

#### Returns

[EventActivityBinder\<TSaga, TMessage\>](../masstransit/eventactivitybinder-2)<br/>

### **Add(IStateMachineActivity\<TSaga, TMessage\>)**

```csharp
EventActivityBinder<TSaga, TMessage> Add(IStateMachineActivity<TSaga, TMessage> activity)
```

#### Parameters

`activity` [IStateMachineActivity\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/istatemachineactivity-2)<br/>

#### Returns

[EventActivityBinder\<TSaga, TMessage\>](../masstransit/eventactivitybinder-2)<br/>

### **Catch\<T\>(Func\<ExceptionActivityBinder\<TSaga, TMessage, T\>, ExceptionActivityBinder\<TSaga, TMessage, T\>\>)**

Catch the exception of type T, and execute the compensation chain

```csharp
EventActivityBinder<TSaga, TMessage> Catch<T>(Func<ExceptionActivityBinder<TSaga, TMessage, T>, ExceptionActivityBinder<TSaga, TMessage, T>> activityCallback)
```

#### Type Parameters

`T`<br/>

#### Parameters

`activityCallback` [Func\<ExceptionActivityBinder\<TSaga, TMessage, T\>, ExceptionActivityBinder\<TSaga, TMessage, T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[EventActivityBinder\<TSaga, TMessage\>](../masstransit/eventactivitybinder-2)<br/>

### **Retry(Action\<IRetryConfigurator\>, Func\<EventActivityBinder\<TSaga, TMessage\>, EventActivityBinder\<TSaga, TMessage\>\>)**

Retry the behavior, using the specified retry policy

```csharp
EventActivityBinder<TSaga, TMessage> Retry(Action<IRetryConfigurator> configure, Func<EventActivityBinder<TSaga, TMessage>, EventActivityBinder<TSaga, TMessage>> activityCallback)
```

#### Parameters

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configures the retry

`activityCallback` [Func\<EventActivityBinder\<TSaga, TMessage\>, EventActivityBinder\<TSaga, TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[EventActivityBinder\<TSaga, TMessage\>](../masstransit/eventactivitybinder-2)<br/>

### **If(StateMachineCondition\<TSaga, TMessage\>, Func\<EventActivityBinder\<TSaga, TMessage\>, EventActivityBinder\<TSaga, TMessage\>\>)**

Create a conditional branch of activities for processing

```csharp
EventActivityBinder<TSaga, TMessage> If(StateMachineCondition<TSaga, TMessage> condition, Func<EventActivityBinder<TSaga, TMessage>, EventActivityBinder<TSaga, TMessage>> activityCallback)
```

#### Parameters

`condition` [StateMachineCondition\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/statemachinecondition-2)<br/>

`activityCallback` [Func\<EventActivityBinder\<TSaga, TMessage\>, EventActivityBinder\<TSaga, TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[EventActivityBinder\<TSaga, TMessage\>](../masstransit/eventactivitybinder-2)<br/>

### **IfAsync(StateMachineAsyncCondition\<TSaga, TMessage\>, Func\<EventActivityBinder\<TSaga, TMessage\>, EventActivityBinder\<TSaga, TMessage\>\>)**

Create a conditional branch of activities for processing

```csharp
EventActivityBinder<TSaga, TMessage> IfAsync(StateMachineAsyncCondition<TSaga, TMessage> condition, Func<EventActivityBinder<TSaga, TMessage>, EventActivityBinder<TSaga, TMessage>> activityCallback)
```

#### Parameters

`condition` [StateMachineAsyncCondition\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/statemachineasynccondition-2)<br/>

`activityCallback` [Func\<EventActivityBinder\<TSaga, TMessage\>, EventActivityBinder\<TSaga, TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[EventActivityBinder\<TSaga, TMessage\>](../masstransit/eventactivitybinder-2)<br/>

### **IfElse(StateMachineCondition\<TSaga, TMessage\>, Func\<EventActivityBinder\<TSaga, TMessage\>, EventActivityBinder\<TSaga, TMessage\>\>, Func\<EventActivityBinder\<TSaga, TMessage\>, EventActivityBinder\<TSaga, TMessage\>\>)**

Create a conditional branch of activities for processing

```csharp
EventActivityBinder<TSaga, TMessage> IfElse(StateMachineCondition<TSaga, TMessage> condition, Func<EventActivityBinder<TSaga, TMessage>, EventActivityBinder<TSaga, TMessage>> thenActivityCallback, Func<EventActivityBinder<TSaga, TMessage>, EventActivityBinder<TSaga, TMessage>> elseActivityCallback)
```

#### Parameters

`condition` [StateMachineCondition\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/statemachinecondition-2)<br/>

`thenActivityCallback` [Func\<EventActivityBinder\<TSaga, TMessage\>, EventActivityBinder\<TSaga, TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`elseActivityCallback` [Func\<EventActivityBinder\<TSaga, TMessage\>, EventActivityBinder\<TSaga, TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[EventActivityBinder\<TSaga, TMessage\>](../masstransit/eventactivitybinder-2)<br/>

### **IfElseAsync(StateMachineAsyncCondition\<TSaga, TMessage\>, Func\<EventActivityBinder\<TSaga, TMessage\>, EventActivityBinder\<TSaga, TMessage\>\>, Func\<EventActivityBinder\<TSaga, TMessage\>, EventActivityBinder\<TSaga, TMessage\>\>)**

Create a conditional branch of activities for processing

```csharp
EventActivityBinder<TSaga, TMessage> IfElseAsync(StateMachineAsyncCondition<TSaga, TMessage> condition, Func<EventActivityBinder<TSaga, TMessage>, EventActivityBinder<TSaga, TMessage>> thenActivityCallback, Func<EventActivityBinder<TSaga, TMessage>, EventActivityBinder<TSaga, TMessage>> elseActivityCallback)
```

#### Parameters

`condition` [StateMachineAsyncCondition\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/statemachineasynccondition-2)<br/>

`thenActivityCallback` [Func\<EventActivityBinder\<TSaga, TMessage\>, EventActivityBinder\<TSaga, TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`elseActivityCallback` [Func\<EventActivityBinder\<TSaga, TMessage\>, EventActivityBinder\<TSaga, TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[EventActivityBinder\<TSaga, TMessage\>](../masstransit/eventactivitybinder-2)<br/>
