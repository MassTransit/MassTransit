---

title: CatchExceptionActivityBinder<TInstance, TData, TException>

---

# CatchExceptionActivityBinder\<TInstance, TData, TException\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class CatchExceptionActivityBinder<TInstance, TData, TException> : ExceptionActivityBinder<TInstance, TData, TException>, EventActivities<TInstance>
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

`TException`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CatchExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit-sagastatemachine/catchexceptionactivitybinder-3)<br/>
Implements [ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3), [EventActivities\<TInstance\>](../masstransit/eventactivities-1)

## Properties

### **Event**

```csharp
public Event<TData> Event { get; }
```

#### Property Value

[Event\<TData\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **StateMachine**

```csharp
public StateMachine<TInstance> StateMachine { get; }
```

#### Property Value

[StateMachine\<TInstance\>](../../masstransit-abstractions/masstransit/statemachine-1)<br/>

## Constructors

### **CatchExceptionActivityBinder(StateMachine\<TInstance\>, Event\<TData\>)**

```csharp
public CatchExceptionActivityBinder(StateMachine<TInstance> machine, Event<TData> event)
```

#### Parameters

`machine` [StateMachine\<TInstance\>](../../masstransit-abstractions/masstransit/statemachine-1)<br/>

`event` [Event\<TData\>](../../masstransit-abstractions/masstransit/event-1)<br/>

## Methods

### **GetStateActivityBinders()**

```csharp
public IEnumerable<IActivityBinder<TInstance>> GetStateActivityBinders()
```

#### Returns

[IEnumerable\<IActivityBinder\<TInstance\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Add(IStateMachineActivity\<TInstance\>)**

```csharp
public ExceptionActivityBinder<TInstance, TData, TException> Add(IStateMachineActivity<TInstance> activity)
```

#### Parameters

`activity` [IStateMachineActivity\<TInstance\>](../../masstransit-abstractions/masstransit/istatemachineactivity-1)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Add(IStateMachineActivity\<TInstance, TData\>)**

```csharp
public ExceptionActivityBinder<TInstance, TData, TException> Add(IStateMachineActivity<TInstance, TData> activity)
```

#### Parameters

`activity` [IStateMachineActivity\<TInstance, TData\>](../../masstransit-abstractions/masstransit/istatemachineactivity-2)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **Catch\<T\>(Func\<ExceptionActivityBinder\<TInstance, TData, T\>, ExceptionActivityBinder\<TInstance, TData, T\>\>)**

```csharp
public ExceptionActivityBinder<TInstance, TData, TException> Catch<T>(Func<ExceptionActivityBinder<TInstance, TData, T>, ExceptionActivityBinder<TInstance, TData, T>> activityCallback)
```

#### Type Parameters

`T`<br/>

#### Parameters

`activityCallback` [Func\<ExceptionActivityBinder\<TInstance, TData, T\>, ExceptionActivityBinder\<TInstance, TData, T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **If(StateMachineExceptionCondition\<TInstance, TData, TException\>, Func\<ExceptionActivityBinder\<TInstance, TData, TException\>, ExceptionActivityBinder\<TInstance, TData, TException\>\>)**

```csharp
public ExceptionActivityBinder<TInstance, TData, TException> If(StateMachineExceptionCondition<TInstance, TData, TException> condition, Func<ExceptionActivityBinder<TInstance, TData, TException>, ExceptionActivityBinder<TInstance, TData, TException>> activityCallback)
```

#### Parameters

`condition` [StateMachineExceptionCondition\<TInstance, TData, TException\>](../../masstransit-abstractions/masstransit/statemachineexceptioncondition-3)<br/>

`activityCallback` [Func\<ExceptionActivityBinder\<TInstance, TData, TException\>, ExceptionActivityBinder\<TInstance, TData, TException\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **IfAsync(StateMachineAsyncExceptionCondition\<TInstance, TData, TException\>, Func\<ExceptionActivityBinder\<TInstance, TData, TException\>, ExceptionActivityBinder\<TInstance, TData, TException\>\>)**

```csharp
public ExceptionActivityBinder<TInstance, TData, TException> IfAsync(StateMachineAsyncExceptionCondition<TInstance, TData, TException> condition, Func<ExceptionActivityBinder<TInstance, TData, TException>, ExceptionActivityBinder<TInstance, TData, TException>> activityCallback)
```

#### Parameters

`condition` [StateMachineAsyncExceptionCondition\<TInstance, TData, TException\>](../../masstransit-abstractions/masstransit/statemachineasyncexceptioncondition-3)<br/>

`activityCallback` [Func\<ExceptionActivityBinder\<TInstance, TData, TException\>, ExceptionActivityBinder\<TInstance, TData, TException\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **IfElse(StateMachineExceptionCondition\<TInstance, TData, TException\>, Func\<ExceptionActivityBinder\<TInstance, TData, TException\>, ExceptionActivityBinder\<TInstance, TData, TException\>\>, Func\<ExceptionActivityBinder\<TInstance, TData, TException\>, ExceptionActivityBinder\<TInstance, TData, TException\>\>)**

```csharp
public ExceptionActivityBinder<TInstance, TData, TException> IfElse(StateMachineExceptionCondition<TInstance, TData, TException> condition, Func<ExceptionActivityBinder<TInstance, TData, TException>, ExceptionActivityBinder<TInstance, TData, TException>> thenActivityCallback, Func<ExceptionActivityBinder<TInstance, TData, TException>, ExceptionActivityBinder<TInstance, TData, TException>> elseActivityCallback)
```

#### Parameters

`condition` [StateMachineExceptionCondition\<TInstance, TData, TException\>](../../masstransit-abstractions/masstransit/statemachineexceptioncondition-3)<br/>

`thenActivityCallback` [Func\<ExceptionActivityBinder\<TInstance, TData, TException\>, ExceptionActivityBinder\<TInstance, TData, TException\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`elseActivityCallback` [Func\<ExceptionActivityBinder\<TInstance, TData, TException\>, ExceptionActivityBinder\<TInstance, TData, TException\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **IfElseAsync(StateMachineAsyncExceptionCondition\<TInstance, TData, TException\>, Func\<ExceptionActivityBinder\<TInstance, TData, TException\>, ExceptionActivityBinder\<TInstance, TData, TException\>\>, Func\<ExceptionActivityBinder\<TInstance, TData, TException\>, ExceptionActivityBinder\<TInstance, TData, TException\>\>)**

```csharp
public ExceptionActivityBinder<TInstance, TData, TException> IfElseAsync(StateMachineAsyncExceptionCondition<TInstance, TData, TException> condition, Func<ExceptionActivityBinder<TInstance, TData, TException>, ExceptionActivityBinder<TInstance, TData, TException>> thenActivityCallback, Func<ExceptionActivityBinder<TInstance, TData, TException>, ExceptionActivityBinder<TInstance, TData, TException>> elseActivityCallback)
```

#### Parameters

`condition` [StateMachineAsyncExceptionCondition\<TInstance, TData, TException\>](../../masstransit-abstractions/masstransit/statemachineasyncexceptioncondition-3)<br/>

`thenActivityCallback` [Func\<ExceptionActivityBinder\<TInstance, TData, TException\>, ExceptionActivityBinder\<TInstance, TData, TException\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`elseActivityCallback` [Func\<ExceptionActivityBinder\<TInstance, TData, TException\>, ExceptionActivityBinder\<TInstance, TData, TException\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>
