---

title: CatchExceptionActivityBinder<TInstance, TException>

---

# CatchExceptionActivityBinder\<TInstance, TException\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class CatchExceptionActivityBinder<TInstance, TException> : ExceptionActivityBinder<TInstance, TException>, EventActivities<TInstance>
```

#### Type Parameters

`TInstance`<br/>

`TException`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CatchExceptionActivityBinder\<TInstance, TException\>](../masstransit-sagastatemachine/catchexceptionactivitybinder-2)<br/>
Implements [ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2), [EventActivities\<TInstance\>](../masstransit/eventactivities-1)

## Properties

### **Event**

```csharp
public Event Event { get; }
```

#### Property Value

[Event](../../masstransit-abstractions/masstransit/event)<br/>

### **StateMachine**

```csharp
public StateMachine<TInstance> StateMachine { get; }
```

#### Property Value

[StateMachine\<TInstance\>](../../masstransit-abstractions/masstransit/statemachine-1)<br/>

## Constructors

### **CatchExceptionActivityBinder(StateMachine\<TInstance\>, Event)**

```csharp
public CatchExceptionActivityBinder(StateMachine<TInstance> machine, Event event)
```

#### Parameters

`machine` [StateMachine\<TInstance\>](../../masstransit-abstractions/masstransit/statemachine-1)<br/>

`event` [Event](../../masstransit-abstractions/masstransit/event)<br/>

## Methods

### **GetStateActivityBinders()**

```csharp
public IEnumerable<IActivityBinder<TInstance>> GetStateActivityBinders()
```

#### Returns

[IEnumerable\<IActivityBinder\<TInstance\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Add(IStateMachineActivity\<TInstance\>)**

```csharp
public ExceptionActivityBinder<TInstance, TException> Add(IStateMachineActivity<TInstance> activity)
```

#### Parameters

`activity` [IStateMachineActivity\<TInstance\>](../../masstransit-abstractions/masstransit/istatemachineactivity-1)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **Catch\<T\>(Func\<ExceptionActivityBinder\<TInstance, T\>, ExceptionActivityBinder\<TInstance, T\>\>)**

```csharp
public ExceptionActivityBinder<TInstance, TException> Catch<T>(Func<ExceptionActivityBinder<TInstance, T>, ExceptionActivityBinder<TInstance, T>> activityCallback)
```

#### Type Parameters

`T`<br/>

#### Parameters

`activityCallback` [Func\<ExceptionActivityBinder\<TInstance, T\>, ExceptionActivityBinder\<TInstance, T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **If(StateMachineExceptionCondition\<TInstance, TException\>, Func\<ExceptionActivityBinder\<TInstance, TException\>, ExceptionActivityBinder\<TInstance, TException\>\>)**

```csharp
public ExceptionActivityBinder<TInstance, TException> If(StateMachineExceptionCondition<TInstance, TException> condition, Func<ExceptionActivityBinder<TInstance, TException>, ExceptionActivityBinder<TInstance, TException>> activityCallback)
```

#### Parameters

`condition` [StateMachineExceptionCondition\<TInstance, TException\>](../../masstransit-abstractions/masstransit/statemachineexceptioncondition-2)<br/>

`activityCallback` [Func\<ExceptionActivityBinder\<TInstance, TException\>, ExceptionActivityBinder\<TInstance, TException\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **IfAsync(StateMachineAsyncExceptionCondition\<TInstance, TException\>, Func\<ExceptionActivityBinder\<TInstance, TException\>, ExceptionActivityBinder\<TInstance, TException\>\>)**

```csharp
public ExceptionActivityBinder<TInstance, TException> IfAsync(StateMachineAsyncExceptionCondition<TInstance, TException> condition, Func<ExceptionActivityBinder<TInstance, TException>, ExceptionActivityBinder<TInstance, TException>> activityCallback)
```

#### Parameters

`condition` [StateMachineAsyncExceptionCondition\<TInstance, TException\>](../../masstransit-abstractions/masstransit/statemachineasyncexceptioncondition-2)<br/>

`activityCallback` [Func\<ExceptionActivityBinder\<TInstance, TException\>, ExceptionActivityBinder\<TInstance, TException\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **IfElse(StateMachineExceptionCondition\<TInstance, TException\>, Func\<ExceptionActivityBinder\<TInstance, TException\>, ExceptionActivityBinder\<TInstance, TException\>\>, Func\<ExceptionActivityBinder\<TInstance, TException\>, ExceptionActivityBinder\<TInstance, TException\>\>)**

```csharp
public ExceptionActivityBinder<TInstance, TException> IfElse(StateMachineExceptionCondition<TInstance, TException> condition, Func<ExceptionActivityBinder<TInstance, TException>, ExceptionActivityBinder<TInstance, TException>> thenActivityCallback, Func<ExceptionActivityBinder<TInstance, TException>, ExceptionActivityBinder<TInstance, TException>> elseActivityCallback)
```

#### Parameters

`condition` [StateMachineExceptionCondition\<TInstance, TException\>](../../masstransit-abstractions/masstransit/statemachineexceptioncondition-2)<br/>

`thenActivityCallback` [Func\<ExceptionActivityBinder\<TInstance, TException\>, ExceptionActivityBinder\<TInstance, TException\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`elseActivityCallback` [Func\<ExceptionActivityBinder\<TInstance, TException\>, ExceptionActivityBinder\<TInstance, TException\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **IfElseAsync(StateMachineAsyncExceptionCondition\<TInstance, TException\>, Func\<ExceptionActivityBinder\<TInstance, TException\>, ExceptionActivityBinder\<TInstance, TException\>\>, Func\<ExceptionActivityBinder\<TInstance, TException\>, ExceptionActivityBinder\<TInstance, TException\>\>)**

```csharp
public ExceptionActivityBinder<TInstance, TException> IfElseAsync(StateMachineAsyncExceptionCondition<TInstance, TException> condition, Func<ExceptionActivityBinder<TInstance, TException>, ExceptionActivityBinder<TInstance, TException>> thenActivityCallback, Func<ExceptionActivityBinder<TInstance, TException>, ExceptionActivityBinder<TInstance, TException>> elseActivityCallback)
```

#### Parameters

`condition` [StateMachineAsyncExceptionCondition\<TInstance, TException\>](../../masstransit-abstractions/masstransit/statemachineasyncexceptioncondition-2)<br/>

`thenActivityCallback` [Func\<ExceptionActivityBinder\<TInstance, TException\>, ExceptionActivityBinder\<TInstance, TException\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`elseActivityCallback` [Func\<ExceptionActivityBinder\<TInstance, TException\>, ExceptionActivityBinder\<TInstance, TException\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[ExceptionActivityBinder\<TInstance, TException\>](../masstransit/exceptionactivitybinder-2)<br/>
