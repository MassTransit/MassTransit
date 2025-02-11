---

title: StateMachineSagaTestHarnessExtensions

---

# StateMachineSagaTestHarnessExtensions

Namespace: MassTransit.Testing

```csharp
public static class StateMachineSagaTestHarnessExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [StateMachineSagaTestHarnessExtensions](../masstransit-testing/statemachinesagatestharnessextensions)

## Methods

### **StateMachineSaga\<TInstance, TStateMachine\>(BusTestHarness, TStateMachine, String)**

```csharp
public static ISagaStateMachineTestHarness<TStateMachine, TInstance> StateMachineSaga<TInstance, TStateMachine>(BusTestHarness harness, TStateMachine stateMachine, string queueName)
```

#### Type Parameters

`TInstance`<br/>

`TStateMachine`<br/>

#### Parameters

`harness` [BusTestHarness](../masstransit-testing/bustestharness)<br/>

`stateMachine` TStateMachine<br/>

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[ISagaStateMachineTestHarness\<TStateMachine, TInstance\>](../masstransit-testing/isagastatemachinetestharness-2)<br/>

### **StateMachineSaga\<TInstance, TStateMachine\>(BusTestHarness, TStateMachine, ISagaRepository\<TInstance\>, String)**

```csharp
public static ISagaStateMachineTestHarness<TStateMachine, TInstance> StateMachineSaga<TInstance, TStateMachine>(BusTestHarness harness, TStateMachine stateMachine, ISagaRepository<TInstance> repository, string queueName)
```

#### Type Parameters

`TInstance`<br/>

`TStateMachine`<br/>

#### Parameters

`harness` [BusTestHarness](../masstransit-testing/bustestharness)<br/>

`stateMachine` TStateMachine<br/>

`repository` [ISagaRepository\<TInstance\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[ISagaStateMachineTestHarness\<TStateMachine, TInstance\>](../masstransit-testing/isagastatemachinetestharness-2)<br/>

### **ContainsInState\<TStateMachine, TInstance\>(ISagaList\<TInstance\>, Guid, TStateMachine, Func\<TStateMachine, State\>)**

```csharp
public static TInstance ContainsInState<TStateMachine, TInstance>(ISagaList<TInstance> sagas, Guid correlationId, TStateMachine machine, Func<TStateMachine, State> stateSelector)
```

#### Type Parameters

`TStateMachine`<br/>

`TInstance`<br/>

#### Parameters

`sagas` [ISagaList\<TInstance\>](../masstransit-testing/isagalist-1)<br/>

`correlationId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`machine` TStateMachine<br/>

`stateSelector` [Func\<TStateMachine, State\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

TInstance<br/>

### **ContainsInState\<T\>(ISagaList\<T\>, Guid, SagaStateMachine\<T\>, State)**

```csharp
public static T ContainsInState<T>(ISagaList<T> sagas, Guid correlationId, SagaStateMachine<T> machine, State state)
```

#### Type Parameters

`T`<br/>

#### Parameters

`sagas` [ISagaList\<T\>](../masstransit-testing/isagalist-1)<br/>

`correlationId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`machine` [SagaStateMachine\<T\>](../../masstransit-abstractions/masstransit/sagastatemachine-1)<br/>

`state` [State](../../masstransit-abstractions/masstransit/state)<br/>

#### Returns

T<br/>

### **ShouldContainSagaInState\<TStateMachine, TInstance\>(ISagaRepository\<TInstance\>, Guid, TStateMachine, Func\<TStateMachine, State\>, TimeSpan)**

```csharp
public static Task<Nullable<Guid>> ShouldContainSagaInState<TStateMachine, TInstance>(ISagaRepository<TInstance> repository, Guid correlationId, TStateMachine machine, Func<TStateMachine, State> stateSelector, TimeSpan timeout)
```

#### Type Parameters

`TStateMachine`<br/>

`TInstance`<br/>

#### Parameters

`repository` [ISagaRepository\<TInstance\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>

`correlationId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`machine` TStateMachine<br/>

`stateSelector` [Func\<TStateMachine, State\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[Task\<Nullable\<Guid\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ShouldContainSagaInState\<TStateMachine, TInstance\>(ISagaRepository\<TInstance\>, Guid, TStateMachine, State, TimeSpan)**

```csharp
public static Task<Nullable<Guid>> ShouldContainSagaInState<TStateMachine, TInstance>(ISagaRepository<TInstance> repository, Guid correlationId, TStateMachine machine, State state, TimeSpan timeout)
```

#### Type Parameters

`TStateMachine`<br/>

`TInstance`<br/>

#### Parameters

`repository` [ISagaRepository\<TInstance\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>

`correlationId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`machine` TStateMachine<br/>

`state` [State](../../masstransit-abstractions/masstransit/state)<br/>

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[Task\<Nullable\<Guid\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ShouldContainSagaInState\<TStateMachine, TInstance\>(ISagaRepository\<TInstance\>, Expression\<Func\<TInstance, Boolean\>\>, TStateMachine, Func\<TStateMachine, State\>, TimeSpan)**

```csharp
public static Task<Nullable<Guid>> ShouldContainSagaInState<TStateMachine, TInstance>(ISagaRepository<TInstance> repository, Expression<Func<TInstance, bool>> expression, TStateMachine machine, Func<TStateMachine, State> stateSelector, TimeSpan timeout)
```

#### Type Parameters

`TStateMachine`<br/>

`TInstance`<br/>

#### Parameters

`repository` [ISagaRepository\<TInstance\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>

`expression` Expression\<Func\<TInstance, Boolean\>\><br/>

`machine` TStateMachine<br/>

`stateSelector` [Func\<TStateMachine, State\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[Task\<Nullable\<Guid\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ShouldContainSagaInState\<TStateMachine, TInstance\>(ISagaRepository\<TInstance\>, Expression\<Func\<TInstance, Boolean\>\>, TStateMachine, State, TimeSpan)**

```csharp
public static Task<Nullable<Guid>> ShouldContainSagaInState<TStateMachine, TInstance>(ISagaRepository<TInstance> repository, Expression<Func<TInstance, bool>> expression, TStateMachine machine, State state, TimeSpan timeout)
```

#### Type Parameters

`TStateMachine`<br/>

`TInstance`<br/>

#### Parameters

`repository` [ISagaRepository\<TInstance\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>

`expression` Expression\<Func\<TInstance, Boolean\>\><br/>

`machine` TStateMachine<br/>

`state` [State](../../masstransit-abstractions/masstransit/state)<br/>

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[Task\<Nullable\<Guid\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
