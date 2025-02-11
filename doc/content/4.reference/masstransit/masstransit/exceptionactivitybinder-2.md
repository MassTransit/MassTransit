---

title: ExceptionActivityBinder<TSaga, TException>

---

# ExceptionActivityBinder\<TSaga, TException\>

Namespace: MassTransit

```csharp
public interface ExceptionActivityBinder<TSaga, TException> : EventActivities<TSaga>
```

#### Type Parameters

`TSaga`<br/>

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
public abstract Event Event { get; }
```

#### Property Value

[Event](../../masstransit-abstractions/masstransit/event)<br/>

## Methods

### **Add(IStateMachineActivity\<TSaga\>)**

```csharp
ExceptionActivityBinder<TSaga, TException> Add(IStateMachineActivity<TSaga> activity)
```

#### Parameters

`activity` [IStateMachineActivity\<TSaga\>](../../masstransit-abstractions/masstransit/istatemachineactivity-1)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Catch\<T\>(Func\<ExceptionActivityBinder\<TSaga, T\>, ExceptionActivityBinder\<TSaga, T\>\>)**

Catch an exception and execute the compensating activities

```csharp
ExceptionActivityBinder<TSaga, TException> Catch<T>(Func<ExceptionActivityBinder<TSaga, T>, ExceptionActivityBinder<TSaga, T>> activityCallback)
```

#### Type Parameters

`T`<br/>

#### Parameters

`activityCallback` [Func\<ExceptionActivityBinder\<TSaga, T\>, ExceptionActivityBinder\<TSaga, T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **If(StateMachineExceptionCondition\<TSaga, TException\>, Func\<ExceptionActivityBinder\<TSaga, TException\>, ExceptionActivityBinder\<TSaga, TException\>\>)**

Create a conditional branch of activities for processing

```csharp
ExceptionActivityBinder<TSaga, TException> If(StateMachineExceptionCondition<TSaga, TException> condition, Func<ExceptionActivityBinder<TSaga, TException>, ExceptionActivityBinder<TSaga, TException>> activityCallback)
```

#### Parameters

`condition` [StateMachineExceptionCondition\<TSaga, TException\>](../../masstransit-abstractions/masstransit/statemachineexceptioncondition-2)<br/>

`activityCallback` [Func\<ExceptionActivityBinder\<TSaga, TException\>, ExceptionActivityBinder\<TSaga, TException\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **IfAsync(StateMachineAsyncExceptionCondition\<TSaga, TException\>, Func\<ExceptionActivityBinder\<TSaga, TException\>, ExceptionActivityBinder\<TSaga, TException\>\>)**

Create a conditional branch of activities for processing

```csharp
ExceptionActivityBinder<TSaga, TException> IfAsync(StateMachineAsyncExceptionCondition<TSaga, TException> condition, Func<ExceptionActivityBinder<TSaga, TException>, ExceptionActivityBinder<TSaga, TException>> activityCallback)
```

#### Parameters

`condition` [StateMachineAsyncExceptionCondition\<TSaga, TException\>](../../masstransit-abstractions/masstransit/statemachineasyncexceptioncondition-2)<br/>

`activityCallback` [Func\<ExceptionActivityBinder\<TSaga, TException\>, ExceptionActivityBinder\<TSaga, TException\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **IfElse(StateMachineExceptionCondition\<TSaga, TException\>, Func\<ExceptionActivityBinder\<TSaga, TException\>, ExceptionActivityBinder\<TSaga, TException\>\>, Func\<ExceptionActivityBinder\<TSaga, TException\>, ExceptionActivityBinder\<TSaga, TException\>\>)**

Create a conditional branch of activities for processing

```csharp
ExceptionActivityBinder<TSaga, TException> IfElse(StateMachineExceptionCondition<TSaga, TException> condition, Func<ExceptionActivityBinder<TSaga, TException>, ExceptionActivityBinder<TSaga, TException>> thenActivityCallback, Func<ExceptionActivityBinder<TSaga, TException>, ExceptionActivityBinder<TSaga, TException>> elseActivityCallback)
```

#### Parameters

`condition` [StateMachineExceptionCondition\<TSaga, TException\>](../../masstransit-abstractions/masstransit/statemachineexceptioncondition-2)<br/>

`thenActivityCallback` [Func\<ExceptionActivityBinder\<TSaga, TException\>, ExceptionActivityBinder\<TSaga, TException\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`elseActivityCallback` [Func\<ExceptionActivityBinder\<TSaga, TException\>, ExceptionActivityBinder\<TSaga, TException\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **IfElseAsync(StateMachineAsyncExceptionCondition\<TSaga, TException\>, Func\<ExceptionActivityBinder\<TSaga, TException\>, ExceptionActivityBinder\<TSaga, TException\>\>, Func\<ExceptionActivityBinder\<TSaga, TException\>, ExceptionActivityBinder\<TSaga, TException\>\>)**

Create a conditional branch of activities for processing

```csharp
ExceptionActivityBinder<TSaga, TException> IfElseAsync(StateMachineAsyncExceptionCondition<TSaga, TException> condition, Func<ExceptionActivityBinder<TSaga, TException>, ExceptionActivityBinder<TSaga, TException>> thenActivityCallback, Func<ExceptionActivityBinder<TSaga, TException>, ExceptionActivityBinder<TSaga, TException>> elseActivityCallback)
```

#### Parameters

`condition` [StateMachineAsyncExceptionCondition\<TSaga, TException\>](../../masstransit-abstractions/masstransit/statemachineasyncexceptioncondition-2)<br/>

`thenActivityCallback` [Func\<ExceptionActivityBinder\<TSaga, TException\>, ExceptionActivityBinder\<TSaga, TException\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`elseActivityCallback` [Func\<ExceptionActivityBinder\<TSaga, TException\>, ExceptionActivityBinder\<TSaga, TException\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>
