---

title: SagaStateMachineRegistration<TStateMachine, TInstance>

---

# SagaStateMachineRegistration\<TStateMachine, TInstance\>

Namespace: MassTransit.DependencyInjection.Registration

A saga state machine represents a state machine and instance, which will use the container to resolve, as well
 as the saga repository.

```csharp
public class SagaStateMachineRegistration<TStateMachine, TInstance> : ISagaRegistration, IRegistration
```

#### Type Parameters

`TStateMachine`<br/>

`TInstance`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SagaStateMachineRegistration\<TStateMachine, TInstance\>](../masstransit-dependencyinjection-registration/sagastatemachineregistration-2)<br/>
Implements [ISagaRegistration](../masstransit-configuration/isagaregistration), [IRegistration](../masstransit-configuration/iregistration)

## Properties

### **Type**

```csharp
public Type Type { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **IncludeInConfigureEndpoints**

```csharp
public bool IncludeInConfigureEndpoints { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **SagaStateMachineRegistration(IContainerSelector)**

```csharp
public SagaStateMachineRegistration(IContainerSelector selector)
```

#### Parameters

`selector` [IContainerSelector](../masstransit-configuration/icontainerselector)<br/>

## Methods

### **AddConfigureAction\<T\>(Action\<IRegistrationContext, ISagaConfigurator\<T\>\>)**

```csharp
public void AddConfigureAction<T>(Action<IRegistrationContext, ISagaConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configure` [Action\<IRegistrationContext, ISagaConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

### **Configure(IReceiveEndpointConfigurator, IRegistrationContext)**

```csharp
public void Configure(IReceiveEndpointConfigurator configurator, IRegistrationContext context)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
