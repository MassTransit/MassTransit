---

title: ISagaStateMachineTestHarness<TStateMachine, TInstance>

---

# ISagaStateMachineTestHarness\<TStateMachine, TInstance\>

Namespace: MassTransit.Testing

```csharp
public interface ISagaStateMachineTestHarness<TStateMachine, TInstance> : ISagaTestHarness<TInstance>
```

#### Type Parameters

`TStateMachine`<br/>

`TInstance`<br/>

Implements [ISagaTestHarness\<TInstance\>](../masstransit-testing/isagatestharness-1)

## Properties

### **StateMachine**

```csharp
public abstract TStateMachine StateMachine { get; }
```

#### Property Value

TStateMachine<br/>

## Methods

### **Exists(Guid, Func\<TStateMachine, State\>, Nullable\<TimeSpan\>)**

Waits until a saga exists with the specified correlationId in the specified state

```csharp
Task<Nullable<Guid>> Exists(Guid correlationId, Func<TStateMachine, State> stateSelector, Nullable<TimeSpan> timeout)
```

#### Parameters

`correlationId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`stateSelector` [Func\<TStateMachine, State\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`timeout` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Task\<Nullable\<Guid\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Exists(Guid, State, Nullable\<TimeSpan\>)**

Waits until a saga exists with the specified correlationId in the specified state

```csharp
Task<Nullable<Guid>> Exists(Guid correlationId, State state, Nullable<TimeSpan> timeout)
```

#### Parameters

`correlationId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`state` [State](../../masstransit-abstractions/masstransit/state)<br/>
The expected state

`timeout` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Task\<Nullable\<Guid\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Exists(Expression\<Func\<TInstance, Boolean\>\>, Func\<TStateMachine, State\>, Nullable\<TimeSpan\>)**

Waits until a saga exists with the specified correlationId in the specified state

```csharp
Task<IList<Guid>> Exists(Expression<Func<TInstance, bool>> expression, Func<TStateMachine, State> stateSelector, Nullable<TimeSpan> timeout)
```

#### Parameters

`expression` Expression\<Func\<TInstance, Boolean\>\><br/>

`stateSelector` [Func\<TStateMachine, State\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`timeout` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Task\<IList\<Guid\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Exists(Expression\<Func\<TInstance, Boolean\>\>, State, Nullable\<TimeSpan\>)**

Waits until a saga exists with the specified correlationId in the specified state

```csharp
Task<IList<Guid>> Exists(Expression<Func<TInstance, bool>> expression, State state, Nullable<TimeSpan> timeout)
```

#### Parameters

`expression` Expression\<Func\<TInstance, Boolean\>\><br/>

`state` [State](../../masstransit-abstractions/masstransit/state)<br/>
The expected state

`timeout` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Task\<IList\<Guid\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
