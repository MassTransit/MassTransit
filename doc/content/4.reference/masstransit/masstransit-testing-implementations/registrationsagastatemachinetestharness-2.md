---

title: RegistrationSagaStateMachineTestHarness<TStateMachine, TInstance>

---

# RegistrationSagaStateMachineTestHarness\<TStateMachine, TInstance\>

Namespace: MassTransit.Testing.Implementations

```csharp
public class RegistrationSagaStateMachineTestHarness<TStateMachine, TInstance> : BaseSagaTestHarness<TInstance>, ISagaStateMachineTestHarness<TStateMachine, TInstance>, ISagaTestHarness<TInstance>, IStateMachineSagaTestHarness<TInstance, TStateMachine>
```

#### Type Parameters

`TStateMachine`<br/>

`TInstance`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BaseSagaTestHarness\<TInstance\>](../masstransit-testing-implementations/basesagatestharness-1) → [RegistrationSagaStateMachineTestHarness\<TStateMachine, TInstance\>](../masstransit-testing-implementations/registrationsagastatemachinetestharness-2)<br/>
Implements [ISagaStateMachineTestHarness\<TStateMachine, TInstance\>](../masstransit-testing/isagastatemachinetestharness-2), [ISagaTestHarness\<TInstance\>](../masstransit-testing/isagatestharness-1), [IStateMachineSagaTestHarness\<TInstance, TStateMachine\>](../masstransit-testing/istatemachinesagatestharness-2)

## Properties

### **Consumed**

```csharp
public IReceivedMessageList Consumed { get; }
```

#### Property Value

[IReceivedMessageList](../masstransit-testing/ireceivedmessagelist)<br/>

### **Sagas**

```csharp
public ISagaList<TInstance> Sagas { get; }
```

#### Property Value

[ISagaList\<TInstance\>](../masstransit-testing/isagalist-1)<br/>

### **Created**

```csharp
public ISagaList<TInstance> Created { get; }
```

#### Property Value

[ISagaList\<TInstance\>](../masstransit-testing/isagalist-1)<br/>

### **StateMachine**

```csharp
public TStateMachine StateMachine { get; }
```

#### Property Value

TStateMachine<br/>

## Constructors

### **RegistrationSagaStateMachineTestHarness(ISagaRepositoryDecoratorRegistration\<TInstance\>, IQuerySagaRepository\<TInstance\>, ILoadSagaRepository\<TInstance\>, TStateMachine)**

```csharp
public RegistrationSagaStateMachineTestHarness(ISagaRepositoryDecoratorRegistration<TInstance> registration, IQuerySagaRepository<TInstance> querySagaRepository, ILoadSagaRepository<TInstance> loadSagaRepository, TStateMachine stateMachine)
```

#### Parameters

`registration` [ISagaRepositoryDecoratorRegistration\<TInstance\>](../masstransit-configuration/isagarepositorydecoratorregistration-1)<br/>

`querySagaRepository` [IQuerySagaRepository\<TInstance\>](../../masstransit-abstractions/masstransit/iquerysagarepository-1)<br/>

`loadSagaRepository` [ILoadSagaRepository\<TInstance\>](../../masstransit-abstractions/masstransit/iloadsagarepository-1)<br/>

`stateMachine` TStateMachine<br/>

## Methods

### **Exists(Guid, Func\<TStateMachine, State\>, Nullable\<TimeSpan\>)**

Waits until a saga exists with the specified correlationId in the specified state

```csharp
public Task<Nullable<Guid>> Exists(Guid correlationId, Func<TStateMachine, State> stateSelector, Nullable<TimeSpan> timeout)
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
public Task<Nullable<Guid>> Exists(Guid correlationId, State state, Nullable<TimeSpan> timeout)
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
public Task<IList<Guid>> Exists(Expression<Func<TInstance, bool>> expression, Func<TStateMachine, State> stateSelector, Nullable<TimeSpan> timeout)
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
public Task<IList<Guid>> Exists(Expression<Func<TInstance, bool>> expression, State state, Nullable<TimeSpan> timeout)
```

#### Parameters

`expression` Expression\<Func\<TInstance, Boolean\>\><br/>

`state` [State](../../masstransit-abstractions/masstransit/state)<br/>
The expected state

`timeout` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Task\<IList\<Guid\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
