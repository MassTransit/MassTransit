---

title: DependencyInjectionReceiveEndpointExtensions

---

# DependencyInjectionReceiveEndpointExtensions

Namespace: MassTransit

```csharp
public static class DependencyInjectionReceiveEndpointExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DependencyInjectionReceiveEndpointExtensions](../masstransit/dependencyinjectionreceiveendpointextensions)

## Methods

### **Consumer\<T\>(IReceiveEndpointConfigurator, IRegistrationContext, Action\<IConsumerConfigurator\<T\>\>)**

Registers a consumer given the lifetime scope specified

```csharp
public static void Consumer<T>(IReceiveEndpointConfigurator configurator, IRegistrationContext context, Action<IConsumerConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>
The consumer type

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>
The service bus configurator

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
The LifetimeScope of the provider

`configure` [Action\<IConsumerConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Consumer\<T\>(IReceiveEndpointConfigurator, IServiceProvider, Action\<IConsumerConfigurator\<T\>\>)**

#### Caution

Use the IRegistrationContext overload instead. Visit https://masstransit.io/obsolete for details.

---

Registers a consumer given the lifetime scope specified

```csharp
public static void Consumer<T>(IReceiveEndpointConfigurator configurator, IServiceProvider provider, Action<IConsumerConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>
The consumer type

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>
The service bus configurator

`provider` IServiceProvider<br/>
The LifetimeScope of the provider

`configure` [Action\<IConsumerConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Consumer\<TConsumer, TMessage\>(IBatchConfigurator\<TMessage\>, IRegistrationContext, Action\<IConsumerMessageConfigurator\<TConsumer, Batch\<TMessage\>\>\>)**

Connect a consumer with a consumer factory method

```csharp
public static void Consumer<TConsumer, TMessage>(IBatchConfigurator<TMessage> configurator, IRegistrationContext context, Action<IConsumerMessageConfigurator<TConsumer, Batch<TMessage>>> configure)
```

#### Type Parameters

`TConsumer`<br/>

`TMessage`<br/>

#### Parameters

`configurator` [IBatchConfigurator\<TMessage\>](../../masstransit-abstractions/masstransit/ibatchconfigurator-1)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

`configure` [Action\<IConsumerMessageConfigurator\<TConsumer, Batch\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Consumer\<TConsumer, TMessage\>(IBatchConfigurator\<TMessage\>, IServiceProvider, Action\<IConsumerMessageConfigurator\<TConsumer, Batch\<TMessage\>\>\>)**

#### Caution

Use the IRegistrationContext overload instead. Visit https://masstransit.io/obsolete for details.

---

Connect a consumer with a consumer factory method

```csharp
public static void Consumer<TConsumer, TMessage>(IBatchConfigurator<TMessage> configurator, IServiceProvider provider, Action<IConsumerMessageConfigurator<TConsumer, Batch<TMessage>>> configure)
```

#### Type Parameters

`TConsumer`<br/>

`TMessage`<br/>

#### Parameters

`configurator` [IBatchConfigurator\<TMessage\>](../../masstransit-abstractions/masstransit/ibatchconfigurator-1)<br/>

`provider` IServiceProvider<br/>

`configure` [Action\<IConsumerMessageConfigurator\<TConsumer, Batch\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ConnectConsumer\<TConsumer\>(IConsumePipeConnector, IRegistrationContext, IPipeSpecification`1[])**

Connect a consumer to the bus/mediator

```csharp
public static ConnectHandle ConnectConsumer<TConsumer>(IConsumePipeConnector connector, IRegistrationContext context, IPipeSpecification`1[] pipeSpecifications)
```

#### Type Parameters

`TConsumer`<br/>

#### Parameters

`connector` [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

`pipeSpecifications` [IPipeSpecification`1[]](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectConsumer\<TConsumer\>(IConsumePipeConnector, IServiceProvider, IPipeSpecification`1[])**

#### Caution

Use the IRegistrationContext overload instead. Visit https://masstransit.io/obsolete for details.

---

Connect a consumer to the bus/mediator

```csharp
public static ConnectHandle ConnectConsumer<TConsumer>(IConsumePipeConnector connector, IServiceProvider provider, IPipeSpecification`1[] pipeSpecifications)
```

#### Type Parameters

`TConsumer`<br/>

#### Parameters

`connector` [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector)<br/>

`provider` IServiceProvider<br/>

`pipeSpecifications` [IPipeSpecification`1[]](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **Saga\<T\>(IReceiveEndpointConfigurator, IRegistrationContext, Action\<ISagaConfigurator\<T\>\>)**

Registers a saga using the container that has the repository resolved from the container

```csharp
public static void Saga<T>(IReceiveEndpointConfigurator configurator, IRegistrationContext context, Action<ISagaConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

`configure` [Action\<ISagaConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Saga\<T\>(IReceiveEndpointConfigurator, IServiceProvider, Action\<ISagaConfigurator\<T\>\>)**

#### Caution

Use the IRegistrationContext overload instead. Visit https://masstransit.io/obsolete for details.

---

Registers a saga using the container that has the repository resolved from the container

```csharp
public static void Saga<T>(IReceiveEndpointConfigurator configurator, IServiceProvider provider, Action<ISagaConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`provider` IServiceProvider<br/>

`configure` [Action\<ISagaConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **StateMachineSaga\<TInstance\>(IReceiveEndpointConfigurator, SagaStateMachine\<TInstance\>, IRegistrationContext, Action\<ISagaConfigurator\<TInstance\>\>)**

Subscribe a state machine saga to the endpoint

```csharp
public static void StateMachineSaga<TInstance>(IReceiveEndpointConfigurator configurator, SagaStateMachine<TInstance> stateMachine, IRegistrationContext context, Action<ISagaConfigurator<TInstance>> configure)
```

#### Type Parameters

`TInstance`<br/>
The state machine instance type

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`stateMachine` [SagaStateMachine\<TInstance\>](../../masstransit-abstractions/masstransit/sagastatemachine-1)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
The Container reference to resolve the repository

`configure` [Action\<ISagaConfigurator\<TInstance\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Optionally configure the saga

### **StateMachineSaga\<TInstance\>(IReceiveEndpointConfigurator, SagaStateMachine\<TInstance\>, IServiceProvider, Action\<ISagaConfigurator\<TInstance\>\>)**

#### Caution

Use the IRegistrationContext overload instead. Visit https://masstransit.io/obsolete for details.

---

Subscribe a state machine saga to the endpoint

```csharp
public static void StateMachineSaga<TInstance>(IReceiveEndpointConfigurator configurator, SagaStateMachine<TInstance> stateMachine, IServiceProvider serviceProvider, Action<ISagaConfigurator<TInstance>> configure)
```

#### Type Parameters

`TInstance`<br/>
The state machine instance type

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`stateMachine` [SagaStateMachine\<TInstance\>](../../masstransit-abstractions/masstransit/sagastatemachine-1)<br/>

`serviceProvider` IServiceProvider<br/>
The Container reference to resolve the repository

`configure` [Action\<ISagaConfigurator\<TInstance\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Optionally configure the saga

### **StateMachineSaga\<TInstance\>(IReceiveEndpointConfigurator, IRegistrationContext, Action\<ISagaConfigurator\<TInstance\>\>)**

Subscribe a state machine saga to the endpoint

```csharp
public static void StateMachineSaga<TInstance>(IReceiveEndpointConfigurator configurator, IRegistrationContext context, Action<ISagaConfigurator<TInstance>> configure)
```

#### Type Parameters

`TInstance`<br/>
The state machine instance type

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
The Container reference to resolve the repository

`configure` [Action\<ISagaConfigurator\<TInstance\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Optionally configure the saga

### **StateMachineSaga\<TInstance\>(IReceiveEndpointConfigurator, IServiceProvider, Action\<ISagaConfigurator\<TInstance\>\>)**

#### Caution

Use the IRegistrationContext overload instead. Visit https://masstransit.io/obsolete for details.

---

Subscribe a state machine saga to the endpoint

```csharp
public static void StateMachineSaga<TInstance>(IReceiveEndpointConfigurator configurator, IServiceProvider provider, Action<ISagaConfigurator<TInstance>> configure)
```

#### Type Parameters

`TInstance`<br/>
The state machine instance type

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`provider` IServiceProvider<br/>
The Container reference to resolve the repository

`configure` [Action\<ISagaConfigurator\<TInstance\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Optionally configure the saga

### **ExecuteActivityHost\<TActivity, TArguments\>(IReceiveEndpointConfigurator, Uri, IRegistrationContext, Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>)**

```csharp
public static void ExecuteActivityHost<TActivity, TArguments>(IReceiveEndpointConfigurator configurator, Uri compensateAddress, IRegistrationContext context, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`compensateAddress` Uri<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

`configure` [Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ExecuteActivityHost\<TActivity, TArguments\>(IReceiveEndpointConfigurator, Uri, IServiceProvider, Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>)**

#### Caution

Use the IRegistrationContext overload instead. Visit https://masstransit.io/obsolete for details.

---

```csharp
public static void ExecuteActivityHost<TActivity, TArguments>(IReceiveEndpointConfigurator configurator, Uri compensateAddress, IServiceProvider provider, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`compensateAddress` Uri<br/>

`provider` IServiceProvider<br/>

`configure` [Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ExecuteActivityHost\<TActivity, TArguments\>(IReceiveEndpointConfigurator, IRegistrationContext, Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>)**

```csharp
public static void ExecuteActivityHost<TActivity, TArguments>(IReceiveEndpointConfigurator configurator, IRegistrationContext context, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

`configure` [Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ExecuteActivityHost\<TActivity, TArguments\>(IReceiveEndpointConfigurator, IServiceProvider, Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>)**

#### Caution

Use the IRegistrationContext overload instead. Visit https://masstransit.io/obsolete for details.

---

```csharp
public static void ExecuteActivityHost<TActivity, TArguments>(IReceiveEndpointConfigurator configurator, IServiceProvider provider, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`provider` IServiceProvider<br/>

`configure` [Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **CompensateActivityHost\<TActivity, TLog\>(IReceiveEndpointConfigurator, IRegistrationContext, Action\<ICompensateActivityConfigurator\<TActivity, TLog\>\>)**

```csharp
public static void CompensateActivityHost<TActivity, TLog>(IReceiveEndpointConfigurator configurator, IRegistrationContext context, Action<ICompensateActivityConfigurator<TActivity, TLog>> configure)
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

`configure` [Action\<ICompensateActivityConfigurator\<TActivity, TLog\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **CompensateActivityHost\<TActivity, TLog\>(IReceiveEndpointConfigurator, IServiceProvider, Action\<ICompensateActivityConfigurator\<TActivity, TLog\>\>)**

#### Caution

Use the IRegistrationContext overload instead. Visit https://masstransit.io/obsolete for details.

---

```csharp
public static void CompensateActivityHost<TActivity, TLog>(IReceiveEndpointConfigurator configurator, IServiceProvider provider, Action<ICompensateActivityConfigurator<TActivity, TLog>> configure)
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`provider` IServiceProvider<br/>

`configure` [Action\<ICompensateActivityConfigurator\<TActivity, TLog\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
