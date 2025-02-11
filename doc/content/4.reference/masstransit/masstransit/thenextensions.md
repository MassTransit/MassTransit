---

title: ThenExtensions

---

# ThenExtensions

Namespace: MassTransit

```csharp
public static class ThenExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ThenExtensions](../masstransit/thenextensions)

## Methods

### **Then\<TSaga\>(EventActivityBinder\<TSaga\>, Action\<BehaviorContext\<TSaga\>\>)**

Adds a synchronous delegate activity to the event's behavior

```csharp
public static EventActivityBinder<TSaga> Then<TSaga>(EventActivityBinder<TSaga> binder, Action<BehaviorContext<TSaga>> action)
```

#### Type Parameters

`TSaga`<br/>
The state machine instance type

#### Parameters

`binder` [EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>
The event binder

`action` [Action\<BehaviorContext\<TSaga\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The synchronous delegate

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **Then\<TSaga, TException\>(ExceptionActivityBinder\<TSaga, TException\>, Action\<BehaviorExceptionContext\<TSaga, TException\>\>)**

Adds a synchronous delegate activity to the event's behavior

```csharp
public static ExceptionActivityBinder<TSaga, TException> Then<TSaga, TException>(ExceptionActivityBinder<TSaga, TException> binder, Action<BehaviorExceptionContext<TSaga, TException>> action)
```

#### Type Parameters

`TSaga`<br/>
The state machine instance type

`TException`<br/>
The exception type

#### Parameters

`binder` [ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>
The event binder

`action` [Action\<BehaviorExceptionContext\<TSaga, TException\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The synchronous delegate

#### Returns

[ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **ThenAsync\<TSaga, TException\>(ExceptionActivityBinder\<TSaga, TException\>, Func\<BehaviorExceptionContext\<TSaga, TException\>, Task\>)**

Adds a asynchronous delegate activity to the event's behavior

```csharp
public static ExceptionActivityBinder<TSaga, TException> ThenAsync<TSaga, TException>(ExceptionActivityBinder<TSaga, TException> binder, Func<BehaviorExceptionContext<TSaga, TException>, Task> asyncAction)
```

#### Type Parameters

`TSaga`<br/>
The state machine instance type

`TException`<br/>
The exception type

#### Parameters

`binder` [ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>
The event binder

`asyncAction` [Func\<BehaviorExceptionContext\<TSaga, TException\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The asynchronous delegate

#### Returns

[ExceptionActivityBinder\<TSaga, TException\>](../masstransit/exceptionactivitybinder-2)<br/>

### **ThenAsync\<TSaga\>(EventActivityBinder\<TSaga\>, Func\<BehaviorContext\<TSaga\>, Task\>)**

Adds an asynchronous delegate activity to the event's behavior

```csharp
public static EventActivityBinder<TSaga> ThenAsync<TSaga>(EventActivityBinder<TSaga> binder, Func<BehaviorContext<TSaga>, Task> action)
```

#### Type Parameters

`TSaga`<br/>
The state machine instance type

#### Parameters

`binder` [EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>
The event binder

`action` [Func\<BehaviorContext\<TSaga\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The asynchronous delegate

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **Then\<TSaga, TData\>(EventActivityBinder\<TSaga, TData\>, Action\<BehaviorContext\<TSaga, TData\>\>)**

Adds a synchronous delegate activity to the event's behavior

```csharp
public static EventActivityBinder<TSaga, TData> Then<TSaga, TData>(EventActivityBinder<TSaga, TData> binder, Action<BehaviorContext<TSaga, TData>> action)
```

#### Type Parameters

`TSaga`<br/>
The state machine instance type

`TData`<br/>
The event data type

#### Parameters

`binder` [EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>
The event binder

`action` [Action\<BehaviorContext\<TSaga, TData\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The synchronous delegate

#### Returns

[EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Then\<TSaga, TData, TException\>(ExceptionActivityBinder\<TSaga, TData, TException\>, Action\<BehaviorExceptionContext\<TSaga, TData, TException\>\>)**

Adds a synchronous delegate activity to the event's behavior

```csharp
public static ExceptionActivityBinder<TSaga, TData, TException> Then<TSaga, TData, TException>(ExceptionActivityBinder<TSaga, TData, TException> binder, Action<BehaviorExceptionContext<TSaga, TData, TException>> action)
```

#### Type Parameters

`TSaga`<br/>
The state machine instance type

`TData`<br/>
The event data type

`TException`<br/>
The exception type

#### Parameters

`binder` [ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>
The event binder

`action` [Action\<BehaviorExceptionContext\<TSaga, TData, TException\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The synchronous delegate

#### Returns

[ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **ThenAsync\<TSaga, TData, TException\>(ExceptionActivityBinder\<TSaga, TData, TException\>, Func\<BehaviorExceptionContext\<TSaga, TData, TException\>, Task\>)**

Adds a asynchronous delegate activity to the event's behavior

```csharp
public static ExceptionActivityBinder<TSaga, TData, TException> ThenAsync<TSaga, TData, TException>(ExceptionActivityBinder<TSaga, TData, TException> binder, Func<BehaviorExceptionContext<TSaga, TData, TException>, Task> asyncAction)
```

#### Type Parameters

`TSaga`<br/>
The state machine instance type

`TData`<br/>
The event data type

`TException`<br/>
The exception type

#### Parameters

`binder` [ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>
The event binder

`asyncAction` [Func\<BehaviorExceptionContext\<TSaga, TData, TException\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The asynchronous delegate

#### Returns

[ExceptionActivityBinder\<TSaga, TData, TException\>](../masstransit/exceptionactivitybinder-3)<br/>

### **ThenAsync\<TSaga, TData\>(EventActivityBinder\<TSaga, TData\>, Func\<BehaviorContext\<TSaga, TData\>, Task\>)**

Adds an asynchronous delegate activity to the event's behavior

```csharp
public static EventActivityBinder<TSaga, TData> ThenAsync<TSaga, TData>(EventActivityBinder<TSaga, TData> binder, Func<BehaviorContext<TSaga, TData>, Task> action)
```

#### Type Parameters

`TSaga`<br/>
The state machine instance type

`TData`<br/>
The event data type

#### Parameters

`binder` [EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>
The event binder

`action` [Func\<BehaviorContext\<TSaga, TData\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The asynchronous delegate

#### Returns

[EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Execute\<TSaga\>(EventActivityBinder\<TSaga\>, Func\<BehaviorContext\<TSaga\>, IStateMachineActivity\<TSaga\>\>)**

Add an activity execution to the event's behavior

```csharp
public static EventActivityBinder<TSaga> Execute<TSaga>(EventActivityBinder<TSaga> binder, Func<BehaviorContext<TSaga>, IStateMachineActivity<TSaga>> activityFactory)
```

#### Type Parameters

`TSaga`<br/>
The state machine instance type

#### Parameters

`binder` [EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>
The event binder

`activityFactory` [Func\<BehaviorContext\<TSaga\>, IStateMachineActivity\<TSaga\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The factory method which returns the activity to execute

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **Execute\<TSaga\>(EventActivityBinder\<TSaga\>, IStateMachineActivity\<TSaga\>)**

Add an activity execution to the event's behavior

```csharp
public static EventActivityBinder<TSaga> Execute<TSaga>(EventActivityBinder<TSaga> binder, IStateMachineActivity<TSaga> activity)
```

#### Type Parameters

`TSaga`<br/>
The state machine instance type

#### Parameters

`binder` [EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>
The event binder

`activity` [IStateMachineActivity\<TSaga\>](../../masstransit-abstractions/masstransit/istatemachineactivity-1)<br/>
An existing activity

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **ExecuteAsync\<TSaga\>(EventActivityBinder\<TSaga\>, Func\<BehaviorContext\<TSaga\>, Task\<IStateMachineActivity\<TSaga\>\>\>)**

Add an activity execution to the event's behavior

```csharp
public static EventActivityBinder<TSaga> ExecuteAsync<TSaga>(EventActivityBinder<TSaga> binder, Func<BehaviorContext<TSaga>, Task<IStateMachineActivity<TSaga>>> activityFactory)
```

#### Type Parameters

`TSaga`<br/>
The state machine instance type

#### Parameters

`binder` [EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>
The event binder

`activityFactory` [Func\<BehaviorContext\<TSaga\>, Task\<IStateMachineActivity\<TSaga\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The factory method which returns the activity to execute

#### Returns

[EventActivityBinder\<TSaga\>](../masstransit/eventactivitybinder-1)<br/>

### **Execute\<TSaga, TData\>(EventActivityBinder\<TSaga, TData\>, Func\<BehaviorContext\<TSaga, TData\>, IStateMachineActivity\<TSaga, TData\>\>)**

Add an activity execution to the event's behavior

```csharp
public static EventActivityBinder<TSaga, TData> Execute<TSaga, TData>(EventActivityBinder<TSaga, TData> binder, Func<BehaviorContext<TSaga, TData>, IStateMachineActivity<TSaga, TData>> activityFactory)
```

#### Type Parameters

`TSaga`<br/>
The state machine instance type

`TData`<br/>
The event data type

#### Parameters

`binder` [EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>
The event binder

`activityFactory` [Func\<BehaviorContext\<TSaga, TData\>, IStateMachineActivity\<TSaga, TData\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The factory method which returns the activity to execute

#### Returns

[EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **ExecuteAsync\<TSaga, TData\>(EventActivityBinder\<TSaga, TData\>, Func\<BehaviorContext\<TSaga, TData\>, Task\<IStateMachineActivity\<TSaga, TData\>\>\>)**

Add an activity execution to the event's behavior

```csharp
public static EventActivityBinder<TSaga, TData> ExecuteAsync<TSaga, TData>(EventActivityBinder<TSaga, TData> binder, Func<BehaviorContext<TSaga, TData>, Task<IStateMachineActivity<TSaga, TData>>> activityFactory)
```

#### Type Parameters

`TSaga`<br/>
The state machine instance type

`TData`<br/>
The event data type

#### Parameters

`binder` [EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>
The event binder

`activityFactory` [Func\<BehaviorContext\<TSaga, TData\>, Task\<IStateMachineActivity\<TSaga, TData\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The factory method which returns the activity to execute

#### Returns

[EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **Execute\<TSaga, TData\>(EventActivityBinder\<TSaga, TData\>, Func\<BehaviorContext\<TSaga, TData\>, IStateMachineActivity\<TSaga\>\>)**

Add an activity execution to the event's behavior

```csharp
public static EventActivityBinder<TSaga, TData> Execute<TSaga, TData>(EventActivityBinder<TSaga, TData> binder, Func<BehaviorContext<TSaga, TData>, IStateMachineActivity<TSaga>> activityFactory)
```

#### Type Parameters

`TSaga`<br/>
The state machine instance type

`TData`<br/>
The event data type

#### Parameters

`binder` [EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>
The event binder

`activityFactory` [Func\<BehaviorContext\<TSaga, TData\>, IStateMachineActivity\<TSaga\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The factory method which returns the activity to execute

#### Returns

[EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>

### **ExecuteAsync\<TSaga, TData\>(EventActivityBinder\<TSaga, TData\>, Func\<BehaviorContext\<TSaga, TData\>, Task\<IStateMachineActivity\<TSaga\>\>\>)**

Add an activity execution to the event's behavior

```csharp
public static EventActivityBinder<TSaga, TData> ExecuteAsync<TSaga, TData>(EventActivityBinder<TSaga, TData> binder, Func<BehaviorContext<TSaga, TData>, Task<IStateMachineActivity<TSaga>>> activityFactory)
```

#### Type Parameters

`TSaga`<br/>
The state machine instance type

`TData`<br/>
The event data type

#### Parameters

`binder` [EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>
The event binder

`activityFactory` [Func\<BehaviorContext\<TSaga, TData\>, Task\<IStateMachineActivity\<TSaga\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The factory method which returns the activity to execute

#### Returns

[EventActivityBinder\<TSaga, TData\>](../masstransit/eventactivitybinder-2)<br/>
