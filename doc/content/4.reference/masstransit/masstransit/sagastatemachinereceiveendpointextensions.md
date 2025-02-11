---

title: SagaStateMachineReceiveEndpointExtensions

---

# SagaStateMachineReceiveEndpointExtensions

Namespace: MassTransit

```csharp
public static class SagaStateMachineReceiveEndpointExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SagaStateMachineReceiveEndpointExtensions](../masstransit/sagastatemachinereceiveendpointextensions)

## Methods

### **StateMachineSaga\<TInstance\>(IReceiveEndpointConfigurator, SagaStateMachine\<TInstance\>, ISagaRepository\<TInstance\>, Action\<ISagaConfigurator\<TInstance\>\>)**

Subscribe a state machine saga to the endpoint

```csharp
public static void StateMachineSaga<TInstance>(IReceiveEndpointConfigurator configurator, SagaStateMachine<TInstance> stateMachine, ISagaRepository<TInstance> repository, Action<ISagaConfigurator<TInstance>> configure)
```

#### Type Parameters

`TInstance`<br/>
The state machine instance type

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`stateMachine` [SagaStateMachine\<TInstance\>](../../masstransit-abstractions/masstransit/sagastatemachine-1)<br/>
The state machine

`repository` [ISagaRepository\<TInstance\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>
The saga repository for the instances

`configure` [Action\<ISagaConfigurator\<TInstance\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Optionally configure the saga

### **ConnectStateMachineSaga\<TInstance\>(IConsumePipeConnector, SagaStateMachine\<TInstance\>, ISagaRepository\<TInstance\>, Action\<ISagaConfigurator\<TInstance\>\>)**

```csharp
public static ConnectHandle ConnectStateMachineSaga<TInstance>(IConsumePipeConnector bus, SagaStateMachine<TInstance> stateMachine, ISagaRepository<TInstance> repository, Action<ISagaConfigurator<TInstance>> configure)
```

#### Type Parameters

`TInstance`<br/>

#### Parameters

`bus` [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector)<br/>

`stateMachine` [SagaStateMachine\<TInstance\>](../../masstransit-abstractions/masstransit/sagastatemachine-1)<br/>

`repository` [ISagaRepository\<TInstance\>](../../masstransit-abstractions/masstransit/isagarepository-1)<br/>

`configure` [Action\<ISagaConfigurator\<TInstance\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
