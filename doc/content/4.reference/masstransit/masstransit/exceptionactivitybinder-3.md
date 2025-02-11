---

title: ExceptionActivityBinder<TSaga, TMessage, TException>

---

# ExceptionActivityBinder\<TSaga, TMessage, TException\>

Namespace: MassTransit

```csharp
public interface ExceptionActivityBinder<TSaga, TMessage, TException> : EventActivities<TSaga>
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

`TException`<br/>

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
ExceptionActivityBinder<TSaga, TMessage, TException> Add(IStateMachineActivity<TSaga> activity)
```

#### Parameters

`activity` [IStateMachineActivity\<TSaga\>](../../masstransit-abstractions/masstransit/istatemachineactivity-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TMessage, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Add(IStateMachineActivity\<TSaga, TMessage\>)**

```csharp
ExceptionActivityBinder<TSaga, TMessage, TException> Add(IStateMachineActivity<TSaga, TMessage> activity)
```

#### Parameters

`activity` [IStateMachineActivity\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/istatemachineactivity-2)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TMessage, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Catch\<T\>(Func\<ExceptionActivityBinder\<TSaga, TMessage, T\>, ExceptionActivityBinder\<TSaga, TMessage, T\>\>)**

Catch an exception and execute the compensating activities

```csharp
ExceptionActivityBinder<TSaga, TMessage, TException> Catch<T>(Func<ExceptionActivityBinder<TSaga, TMessage, T>, ExceptionActivityBinder<TSaga, TMessage, T>> activityCallback)
```

#### Type Parameters

`T`<br/>

#### Parameters

`activityCallback` [Func\<ExceptionActivityBinder\<TSaga, TMessage, T\>, ExceptionActivityBinder\<TSaga, TMessage, T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TMessage, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **If(StateMachineExceptionCondition\<TSaga, TMessage, TException\>, Func\<ExceptionActivityBinder\<TSaga, TMessage, TException\>, ExceptionActivityBinder\<TSaga, TMessage, TException\>\>)**

Create a conditional branch of activities for processing

```csharp
ExceptionActivityBinder<TSaga, TMessage, TException> If(StateMachineExceptionCondition<TSaga, TMessage, TException> condition, Func<ExceptionActivityBinder<TSaga, TMessage, TException>, ExceptionActivityBinder<TSaga, TMessage, TException>> activityCallback)
```

#### Parameters

`condition` [StateMachineExceptionCondition\<TSaga, TMessage, TException\>](../../masstransit-abstractions/masstransit/statemachineexceptioncondition-3)<br/>

`activityCallback` [Func\<ExceptionActivityBinder\<TSaga, TMessage, TException\>, ExceptionActivityBinder\<TSaga, TMessage, TException\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TMessage, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **IfAsync(StateMachineAsyncExceptionCondition\<TSaga, TMessage, TException\>, Func\<ExceptionActivityBinder\<TSaga, TMessage, TException\>, ExceptionActivityBinder\<TSaga, TMessage, TException\>\>)**

Create a conditional branch of activities for processing

```csharp
ExceptionActivityBinder<TSaga, TMessage, TException> IfAsync(StateMachineAsyncExceptionCondition<TSaga, TMessage, TException> condition, Func<ExceptionActivityBinder<TSaga, TMessage, TException>, ExceptionActivityBinder<TSaga, TMessage, TException>> activityCallback)
```

#### Parameters

`condition` [StateMachineAsyncExceptionCondition\<TSaga, TMessage, TException\>](../../masstransit-abstractions/masstransit/statemachineasyncexceptioncondition-3)<br/>

`activityCallback` [Func\<ExceptionActivityBinder\<TSaga, TMessage, TException\>, ExceptionActivityBinder\<TSaga, TMessage, TException\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TMessage, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **IfElse(StateMachineExceptionCondition\<TSaga, TMessage, TException\>, Func\<ExceptionActivityBinder\<TSaga, TMessage, TException\>, ExceptionActivityBinder\<TSaga, TMessage, TException\>\>, Func\<ExceptionActivityBinder\<TSaga, TMessage, TException\>, ExceptionActivityBinder\<TSaga, TMessage, TException\>\>)**

Create a conditional branch of activities for processing

```csharp
ExceptionActivityBinder<TSaga, TMessage, TException> IfElse(StateMachineExceptionCondition<TSaga, TMessage, TException> condition, Func<ExceptionActivityBinder<TSaga, TMessage, TException>, ExceptionActivityBinder<TSaga, TMessage, TException>> thenActivityCallback, Func<ExceptionActivityBinder<TSaga, TMessage, TException>, ExceptionActivityBinder<TSaga, TMessage, TException>> elseActivityCallback)
```

#### Parameters

`condition` [StateMachineExceptionCondition\<TSaga, TMessage, TException\>](../../masstransit-abstractions/masstransit/statemachineexceptioncondition-3)<br/>

`thenActivityCallback` [Func\<ExceptionActivityBinder\<TSaga, TMessage, TException\>, ExceptionActivityBinder\<TSaga, TMessage, TException\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`elseActivityCallback` [Func\<ExceptionActivityBinder\<TSaga, TMessage, TException\>, ExceptionActivityBinder\<TSaga, TMessage, TException\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TMessage, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **IfElseAsync(StateMachineAsyncExceptionCondition\<TSaga, TMessage, TException\>, Func\<ExceptionActivityBinder\<TSaga, TMessage, TException\>, ExceptionActivityBinder\<TSaga, TMessage, TException\>\>, Func\<ExceptionActivityBinder\<TSaga, TMessage, TException\>, ExceptionActivityBinder\<TSaga, TMessage, TException\>\>)**

Create a conditional branch of activities for processing

```csharp
ExceptionActivityBinder<TSaga, TMessage, TException> IfElseAsync(StateMachineAsyncExceptionCondition<TSaga, TMessage, TException> condition, Func<ExceptionActivityBinder<TSaga, TMessage, TException>, ExceptionActivityBinder<TSaga, TMessage, TException>> thenActivityCallback, Func<ExceptionActivityBinder<TSaga, TMessage, TException>, ExceptionActivityBinder<TSaga, TMessage, TException>> elseActivityCallback)
```

#### Parameters

`condition` [StateMachineAsyncExceptionCondition\<TSaga, TMessage, TException\>](../../masstransit-abstractions/masstransit/statemachineasyncexceptioncondition-3)<br/>

`thenActivityCallback` [Func\<ExceptionActivityBinder\<TSaga, TMessage, TException\>, ExceptionActivityBinder\<TSaga, TMessage, TException\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`elseActivityCallback` [Func\<ExceptionActivityBinder\<TSaga, TMessage, TException\>, ExceptionActivityBinder\<TSaga, TMessage, TException\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TMessage, TException\>](../masstransit/exceptionactivitybinder-3)<br/>
