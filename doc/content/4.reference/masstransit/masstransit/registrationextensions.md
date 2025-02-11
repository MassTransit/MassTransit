---

title: RegistrationExtensions

---

# RegistrationExtensions

Namespace: MassTransit

```csharp
public static class RegistrationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RegistrationExtensions](../masstransit/registrationextensions)

## Methods

### **AddConsumer\<T, TDefinition\>(IRegistrationConfigurator, Action\<IConsumerConfigurator\<T\>\>)**

Adds the consumer, allowing configuration when it is configured on an endpoint

```csharp
public static IConsumerRegistrationConfigurator<T> AddConsumer<T, TDefinition>(IRegistrationConfigurator configurator, Action<IConsumerConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>
The consumer type

`TDefinition`<br/>
The consumer definition type

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`configure` [Action\<IConsumerConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IConsumerRegistrationConfigurator\<T\>](../../masstransit-abstractions/masstransit/iconsumerregistrationconfigurator-1)<br/>

### **AddConsumers(IRegistrationConfigurator, Assembly[])**

Adds all consumers in the specified assemblies

```csharp
public static void AddConsumers(IRegistrationConfigurator configurator, Assembly[] assemblies)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`assemblies` [Assembly[]](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assembly)<br/>
The assemblies to scan for consumers

### **AddConsumers(IRegistrationConfigurator, Func\<Type, Boolean\>, Assembly[])**

Adds all consumers that match the given filter in the specified assemblies

```csharp
public static void AddConsumers(IRegistrationConfigurator configurator, Func<Type, bool> filter, Assembly[] assemblies)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`filter` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`assemblies` [Assembly[]](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assembly)<br/>
The assemblies to scan for consumers

### **AddConsumersFromNamespaceContaining\<T\>(IRegistrationConfigurator, Func\<Type, Boolean\>)**

Adds all consumers from the assembly containing the specified type that are in the same (or deeper) namespace.

```csharp
public static void AddConsumersFromNamespaceContaining<T>(IRegistrationConfigurator configurator, Func<Type, bool> filter)
```

#### Type Parameters

`T`<br/>
The anchor type

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`filter` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **AddConsumersFromNamespaceContaining(IRegistrationConfigurator, Type, Func\<Type, Boolean\>)**

Adds all consumers in the specified assemblies matching the namespace

```csharp
public static void AddConsumersFromNamespaceContaining(IRegistrationConfigurator configurator, Type type, Func<Type, bool> filter)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The type to use to identify the assembly and namespace to scan

`filter` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **AddConsumers(IRegistrationConfigurator, Type[])**

Adds the specified consumer types

```csharp
public static void AddConsumers(IRegistrationConfigurator configurator, Type[] types)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`types` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The state machine types to add

### **AddConsumers(IRegistrationConfigurator, Func\<Type, Boolean\>, Type[])**

Adds the specified consumer types which match the given filter

```csharp
public static void AddConsumers(IRegistrationConfigurator configurator, Func<Type, bool> filter, Type[] types)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`filter` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`types` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The consumer types to add

### **AddSaga\<T, TDefinition\>(IRegistrationConfigurator, Action\<ISagaConfigurator\<T\>\>)**

Adds the saga, allowing configuration when it is configured on the endpoint. This should not
 be used for state machine (Automatonymous) sagas.

```csharp
public static ISagaRegistrationConfigurator<T> AddSaga<T, TDefinition>(IRegistrationConfigurator configurator, Action<ISagaConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>
The saga type

`TDefinition`<br/>
The saga definition type

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`configure` [Action\<ISagaConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ISagaRegistrationConfigurator\<T\>](../masstransit/isagaregistrationconfigurator-1)<br/>

### **AddSagas(IRegistrationConfigurator, Func\<Type, Boolean\>, Assembly[])**

Adds all sagas in the specified assemblies. If using state machine sagas, they should be added first using AddSagaStateMachines.

```csharp
public static void AddSagas(IRegistrationConfigurator configurator, Func<Type, bool> filter, Assembly[] assemblies)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`filter` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`assemblies` [Assembly[]](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assembly)<br/>
The assemblies to scan for consumers

### **AddSagas(IRegistrationConfigurator, Assembly[])**

Adds all sagas in the specified assemblies. If using state machine sagas, they should be added first using AddSagaStateMachines.

```csharp
public static void AddSagas(IRegistrationConfigurator configurator, Assembly[] assemblies)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`assemblies` [Assembly[]](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assembly)<br/>
The assemblies to scan for consumers

### **AddSagasFromNamespaceContaining\<T\>(IRegistrationConfigurator, Func\<Type, Boolean\>)**

Adds all sagas in the specified assemblies matching the namespace. If you are using both state machine and regular sagas, be
 sure to call AddSagaStateMachinesFromNamespaceContaining prior to calling this one.

```csharp
public static void AddSagasFromNamespaceContaining<T>(IRegistrationConfigurator configurator, Func<Type, bool> filter)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`filter` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **AddSagasFromNamespaceContaining(IRegistrationConfigurator, Type, Func\<Type, Boolean\>)**

Adds all sagas in the specified assemblies matching the namespace. If you are using both state machine and regular sagas, be
 sure to call AddSagaStateMachinesFromNamespaceContaining prior to calling this one.

```csharp
public static void AddSagasFromNamespaceContaining(IRegistrationConfigurator configurator, Type type, Func<Type, bool> filter)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The type to use to identify the assembly and namespace to scan

`filter` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **AddSagas(IRegistrationConfigurator, Type[])**

Adds the specified saga and saga definition types

```csharp
public static void AddSagas(IRegistrationConfigurator configurator, Type[] types)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`types` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The state machine types to add

### **AddSagas(IRegistrationConfigurator, Func\<Type, Boolean\>, Type[])**

Adds the specified saga types

```csharp
public static void AddSagas(IRegistrationConfigurator configurator, Func<Type, bool> filter, Type[] types)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`filter` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`types` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The state machine types to add

### **AddSagaStateMachine\<TStateMachine, T, TDefinition\>(IRegistrationConfigurator, Action\<ISagaConfigurator\<T\>\>)**

Adds a SagaStateMachine to the registry and updates the registrar prior to registering so that the default
 saga registrar isn't notified.

```csharp
public static ISagaRegistrationConfigurator<T> AddSagaStateMachine<TStateMachine, T, TDefinition>(IRegistrationConfigurator configurator, Action<ISagaConfigurator<T>> configure)
```

#### Type Parameters

`TStateMachine`<br/>
The state machine type

`T`<br/>
The state machine instance type

`TDefinition`<br/>
The saga definition type

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`configure` [Action\<ISagaConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ISagaRegistrationConfigurator\<T\>](../masstransit/isagaregistrationconfigurator-1)<br/>

### **AddSagaStateMachines(IRegistrationConfigurator, Assembly[])**

Adds SagaStateMachines to the registry, using the factory method, and updates the registrar prior to registering so that the default
 saga registrar isn't notified.

```csharp
public static void AddSagaStateMachines(IRegistrationConfigurator configurator, Assembly[] assemblies)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`assemblies` [Assembly[]](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assembly)<br/>
The assemblies to scan for state machines

### **AddSagaStateMachinesFromNamespaceContaining\<T\>(IRegistrationConfigurator, Func\<Type, Boolean\>)**

Adds all saga state machines in the specified assemblies matching the namespace. If you are using both state machine and regular sagas, be
 sure to call AddSagasFromNamespaceContaining after calling this one.

```csharp
public static void AddSagaStateMachinesFromNamespaceContaining<T>(IRegistrationConfigurator configurator, Func<Type, bool> filter)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`filter` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **AddSagaStateMachinesFromNamespaceContaining(IRegistrationConfigurator, Type, Func\<Type, Boolean\>)**

Adds all saga state machines in the specified assemblies matching the namespace. If you are using both state machine and regular sagas, be
 sure to call AddSagasFromNamespaceContaining after calling this one.

```csharp
public static void AddSagaStateMachinesFromNamespaceContaining(IRegistrationConfigurator configurator, Type type, Func<Type, bool> filter)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The type to use to identify the assembly and namespace to scan

`filter` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **AddSagaStateMachines(IRegistrationConfigurator, Type[])**

Adds SagaStateMachines to the registry, using the factory method, and updates the registrar prior to registering so that the default
 saga registrar isn't notified.

```csharp
public static void AddSagaStateMachines(IRegistrationConfigurator configurator, Type[] types)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`types` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The state machine types to add

### **AddSagaStateMachines(IRegistrationConfigurator, Func\<Type, Boolean\>, Type[])**

Adds SagaStateMachines to the registry, using the factory method, and updates the registrar prior to registering so that the default
 saga registrar isn't notified.

```csharp
public static void AddSagaStateMachines(IRegistrationConfigurator configurator, Func<Type, bool> filter, Type[] types)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`filter` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`types` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The state machine types to add

### **AddExecuteActivity\<TActivity, TArguments, TDefinition\>(IRegistrationConfigurator, Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>)**

Adds an execute activity (Courier), allowing configuration when it is configured on the endpoint.

```csharp
public static IExecuteActivityRegistrationConfigurator<TActivity, TArguments> AddExecuteActivity<TActivity, TArguments, TDefinition>(IRegistrationConfigurator configurator, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure)
```

#### Type Parameters

`TActivity`<br/>
The activity type

`TArguments`<br/>
The argument type

`TDefinition`<br/>
The activity definition type

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`configure` [Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IExecuteActivityRegistrationConfigurator\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/iexecuteactivityregistrationconfigurator-2)<br/>

### **AddActivity\<TActivity, TArguments, TLog, TDefinition\>(IRegistrationConfigurator, Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>, Action\<ICompensateActivityConfigurator\<TActivity, TLog\>\>)**

Adds an activity (Courier), allowing configuration when it is configured on the endpoint.

```csharp
public static IActivityRegistrationConfigurator<TActivity, TArguments, TLog> AddActivity<TActivity, TArguments, TLog, TDefinition>(IRegistrationConfigurator configurator, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute, Action<ICompensateActivityConfigurator<TActivity, TLog>> configureCompensate)
```

#### Type Parameters

`TActivity`<br/>
The activity type

`TArguments`<br/>
The argument type

`TLog`<br/>
The log type

`TDefinition`<br/>
The activity definition type

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`configureExecute` [Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The execute configuration callback

`configureCompensate` [Action\<ICompensateActivityConfigurator\<TActivity, TLog\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The compensate configuration callback

#### Returns

[IActivityRegistrationConfigurator\<TActivity, TArguments, TLog\>](../../masstransit-abstractions/masstransit/iactivityregistrationconfigurator-3)<br/>

### **AddActivities(IRegistrationConfigurator, Assembly[])**

Adds all activities (including execute-only activities) in the specified assemblies.

```csharp
public static void AddActivities(IRegistrationConfigurator configurator, Assembly[] assemblies)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`assemblies` [Assembly[]](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assembly)<br/>
The assemblies to scan for consumers

### **AddActivitiesFromNamespaceContaining\<T\>(IRegistrationConfigurator, Func\<Type, Boolean\>)**

Adds all activities (including execute-only activities) in the specified assemblies matching the namespace.

```csharp
public static void AddActivitiesFromNamespaceContaining<T>(IRegistrationConfigurator configurator, Func<Type, bool> filter)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`filter` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **AddActivitiesFromNamespaceContaining(IRegistrationConfigurator, Type, Func\<Type, Boolean\>)**

Adds all activities (including execute-only activities) in the specified assemblies matching the namespace.

```csharp
public static void AddActivitiesFromNamespaceContaining(IRegistrationConfigurator configurator, Type type, Func<Type, bool> filter)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The type to use to identify the assembly and namespace to scan

`filter` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **AddActivities(IRegistrationConfigurator, Type[])**

Adds the specified activity types

```csharp
public static void AddActivities(IRegistrationConfigurator configurator, Type[] types)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`types` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The state machine types to add

### **AddActivities(IRegistrationConfigurator, Func\<Type, Boolean\>, Type[])**

```csharp
public static void AddActivities(IRegistrationConfigurator configurator, Func<Type, bool> filter, Type[] types)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`filter` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`types` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **SetDefaultEndpointNameFormatter(IRegistrationConfigurator)**

Configure the default endpoint name formatter in the container

```csharp
public static void SetDefaultEndpointNameFormatter(IRegistrationConfigurator configurator)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

### **SetSnakeCaseEndpointNameFormatter(IRegistrationConfigurator)**

```csharp
public static void SetSnakeCaseEndpointNameFormatter(IRegistrationConfigurator configurator)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

### **SetKebabCaseEndpointNameFormatter(IRegistrationConfigurator)**

Configure the Kebab Case endpoint name formatter

```csharp
public static void SetKebabCaseEndpointNameFormatter(IRegistrationConfigurator configurator)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

### **AddFuture\<T, TDefinition\>(IRegistrationConfigurator)**

Adds the consumer, allowing configuration when it is configured on an endpoint

```csharp
public static IFutureRegistrationConfigurator<T> AddFuture<T, TDefinition>(IRegistrationConfigurator configurator)
```

#### Type Parameters

`T`<br/>
The consumer type

`TDefinition`<br/>
The consumer definition type

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

#### Returns

[IFutureRegistrationConfigurator\<T\>](../masstransit/ifutureregistrationconfigurator-1)<br/>

### **AddFutureRequestConsumer\<TFuture, TConsumer, TRequest, TResponse\>(IRegistrationConfigurator, Action\<IConsumerConfigurator\<TConsumer\>\>)**

Adds a combined consumer/future, where the future handles the requests and the consumer is only known to the future.
 This is a shortcut method,

```csharp
public static IFutureRegistrationConfigurator<TFuture> AddFutureRequestConsumer<TFuture, TConsumer, TRequest, TResponse>(IRegistrationConfigurator configurator, Action<IConsumerConfigurator<TConsumer>> configure)
```

#### Type Parameters

`TFuture`<br/>
The consumer type

`TConsumer`<br/>

`TRequest`<br/>

`TResponse`<br/>

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`configure` [Action\<IConsumerConfigurator\<TConsumer\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IFutureRegistrationConfigurator\<TFuture\>](../masstransit/ifutureregistrationconfigurator-1)<br/>

### **AddFutures(IRegistrationConfigurator, Assembly[])**

Adds all futures in the specified assemblies

```csharp
public static void AddFutures(IRegistrationConfigurator configurator, Assembly[] assemblies)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`assemblies` [Assembly[]](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assembly)<br/>
The assemblies to scan for futures

### **AddFutures(IRegistrationConfigurator, Func\<Type, Boolean\>, Assembly[])**

Adds all futures that match the given filter in the specified assemblies

```csharp
public static void AddFutures(IRegistrationConfigurator configurator, Func<Type, bool> filter, Assembly[] assemblies)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`filter` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`assemblies` [Assembly[]](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assembly)<br/>
The assemblies to scan for futures

### **AddFuturesFromNamespaceContaining\<T\>(IRegistrationConfigurator, Func\<Type, Boolean\>)**

Adds all futures from the assembly containing the specified type that are in the same (or deeper) namespace.

```csharp
public static void AddFuturesFromNamespaceContaining<T>(IRegistrationConfigurator configurator, Func<Type, bool> filter)
```

#### Type Parameters

`T`<br/>
The anchor type

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`filter` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **AddFuturesFromNamespaceContaining(IRegistrationConfigurator, Type, Func\<Type, Boolean\>)**

Adds all futures in the specified assemblies matching the namespace

```csharp
public static void AddFuturesFromNamespaceContaining(IRegistrationConfigurator configurator, Type type, Func<Type, bool> filter)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The type to use to identify the assembly and namespace to scan

`filter` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **AddFutures(IRegistrationConfigurator, Type[])**

Adds the specified consumer types

```csharp
public static void AddFutures(IRegistrationConfigurator configurator, Type[] types)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`types` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The state machine types to add

### **AddFutures(IRegistrationConfigurator, Func\<Type, Boolean\>, Type[])**

Adds the specified consumer types which match the given filter

```csharp
public static void AddFutures(IRegistrationConfigurator configurator, Func<Type, bool> filter, Type[] types)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`filter` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`types` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The consumer types to add
