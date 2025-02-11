---

title: StateMachineSagaTestHarness<TInstance, TStateMachine>

---

# StateMachineSagaTestHarness\<TInstance, TStateMachine\>

Namespace: MassTransit.Testing.Implementations

```csharp
public class StateMachineSagaTestHarness<TInstance, TStateMachine> : SagaTestHarness<TInstance>, ISagaTestHarness<TInstance>, ISagaStateMachineTestHarness<TStateMachine, TInstance>
```

#### Type Parameters

`TInstance`<br/>

`TStateMachine`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BaseSagaTestHarness\<TInstance\>](../masstransit-testing-implementations/basesagatestharness-1) → [SagaTestHarness\<TInstance\>](../masstransit-testing/sagatestharness-1) → [StateMachineSagaTestHarness\<TInstance, TStateMachine\>](../masstransit-testing-implementations/statemachinesagatestharness-2)<br/>
Implements [ISagaTestHarness\<TInstance\>](../masstransit-testing/isagatestharness-1), [ISagaStateMachineTestHarness\<TStateMachine, TInstance\>](../masstransit-testing/isagastatemachinetestharness-2)

## Properties

### **StateMachine**

```csharp
public TStateMachine StateMachine { get; }
```

#### Property Value

TStateMachine<br/>

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

## Constructors

### **StateMachineSagaTestHarness(BusTestHarness, ISagaRepository\<TInstance\>, IQuerySagaRepository\<TInstance\>, ILoadSagaRepository\<TInstance\>, TStateMachine, String)**

```csharp
public StateMachineSagaTestHarness(BusTestHarness testHarness, ISagaRepository<TInstance> repository, IQuerySagaRepository<TInstance> querySagaRepository, ILoadSagaRepository<TInstance> loadSagaRepository, TStateMachine stateMachine, string queueName)
```

#### Parameters

`testHarness` [BusTestHarness](../masstransit-testing/bustestharness)<br/>

`repository` [ISagaRepository\<TInstance\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>

`querySagaRepository` [IQuerySagaRepository\<TInstance\>](../../masstransit-abstractions/masstransit/iquerysagarepository-1)<br/>

`loadSagaRepository` [ILoadSagaRepository\<TInstance\>](../../masstransit-abstractions/masstransit/iloadsagarepository-1)<br/>

`stateMachine` TStateMachine<br/>

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

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

### **ConfigureReceiveEndpoint(IReceiveEndpointConfigurator)**

```csharp
protected void ConfigureReceiveEndpoint(IReceiveEndpointConfigurator configurator)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

### **ConfigureNamedReceiveEndpoint(IBusFactoryConfigurator, String)**

```csharp
protected void ConfigureNamedReceiveEndpoint(IBusFactoryConfigurator configurator, string queueName)
```

#### Parameters

`configurator` [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator)<br/>

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
