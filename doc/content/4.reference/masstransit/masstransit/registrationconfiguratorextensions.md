---

title: RegistrationConfiguratorExtensions

---

# RegistrationConfiguratorExtensions

Namespace: MassTransit

```csharp
public static class RegistrationConfiguratorExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RegistrationConfiguratorExtensions](../masstransit/registrationconfiguratorextensions)

## Methods

### **AddConsumer\<T\>(IRegistrationConfigurator, Action\<IConsumerConfigurator\<T\>\>)**

Adds the consumer, allowing configuration when it is configured on an endpoint

```csharp
public static IConsumerRegistrationConfigurator<T> AddConsumer<T>(IRegistrationConfigurator configurator, Action<IConsumerConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>
The consumer type

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`configure` [Action\<IConsumerConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IConsumerRegistrationConfigurator\<T\>](../../masstransit-abstractions/masstransit/iconsumerregistrationconfigurator-1)<br/>

### **AddConsumer\<T\>(IRegistrationConfigurator, Type, Action\<IConsumerConfigurator\<T\>\>)**

Adds the consumer, allowing configuration when it is configured on an endpoint

```csharp
public static IConsumerRegistrationConfigurator<T> AddConsumer<T>(IRegistrationConfigurator configurator, Type consumerDefinitionType, Action<IConsumerConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>
The consumer type

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`consumerDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The consumer definition type

`configure` [Action\<IConsumerConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IConsumerRegistrationConfigurator\<T\>](../../masstransit-abstractions/masstransit/iconsumerregistrationconfigurator-1)<br/>

### **AddSaga\<T\>(IRegistrationConfigurator, Action\<ISagaConfigurator\<T\>\>)**

Adds the saga, allowing configuration when it is configured on the endpoint. This should not
 be used for state machine (Automatonymous) sagas.

```csharp
public static ISagaRegistrationConfigurator<T> AddSaga<T>(IRegistrationConfigurator configurator, Action<ISagaConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>
The saga type

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`configure` [Action\<ISagaConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ISagaRegistrationConfigurator\<T\>](../masstransit/isagaregistrationconfigurator-1)<br/>

### **AddSaga\<T\>(IRegistrationConfigurator, Type, Action\<ISagaConfigurator\<T\>\>)**

Adds the saga, allowing configuration when it is configured on the endpoint. This should not
 be used for state machine (Automatonymous) sagas.

```csharp
public static ISagaRegistrationConfigurator<T> AddSaga<T>(IRegistrationConfigurator configurator, Type sagaDefinitionType, Action<ISagaConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>
The saga type

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`sagaDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The saga definition type

`configure` [Action\<ISagaConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ISagaRegistrationConfigurator\<T\>](../masstransit/isagaregistrationconfigurator-1)<br/>

### **AddSagaStateMachine\<TStateMachine, T\>(IRegistrationConfigurator, Action\<ISagaConfigurator\<T\>\>)**

Adds a SagaStateMachine to the registry, using the factory method, and updates the registrar prior to registering so that the default
 saga registrar isn't notified.

```csharp
public static ISagaRegistrationConfigurator<T> AddSagaStateMachine<TStateMachine, T>(IRegistrationConfigurator configurator, Action<ISagaConfigurator<T>> configure)
```

#### Type Parameters

`TStateMachine`<br/>

`T`<br/>

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`configure` [Action\<ISagaConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ISagaRegistrationConfigurator\<T\>](../masstransit/isagaregistrationconfigurator-1)<br/>

### **AddSagaStateMachine\<TStateMachine, T\>(IRegistrationConfigurator, Type, Action\<ISagaConfigurator\<T\>\>)**

Adds a SagaStateMachine to the registry, using the factory method, and updates the registrar prior to registering so that the default
 saga registrar isn't notified.

```csharp
public static ISagaRegistrationConfigurator<T> AddSagaStateMachine<TStateMachine, T>(IRegistrationConfigurator configurator, Type sagaDefinitionType, Action<ISagaConfigurator<T>> configure)
```

#### Type Parameters

`TStateMachine`<br/>

`T`<br/>

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`sagaDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`configure` [Action\<ISagaConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ISagaRegistrationConfigurator\<T\>](../masstransit/isagaregistrationconfigurator-1)<br/>

### **AddExecuteActivity\<TActivity, TArguments\>(IRegistrationConfigurator, Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>)**

Adds an execute activity (Courier), allowing configuration when it is configured on the endpoint.

```csharp
public static IExecuteActivityRegistrationConfigurator<TActivity, TArguments> AddExecuteActivity<TActivity, TArguments>(IRegistrationConfigurator configurator, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure)
```

#### Type Parameters

`TActivity`<br/>
The activity type

`TArguments`<br/>
The argument type

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`configure` [Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IExecuteActivityRegistrationConfigurator\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/iexecuteactivityregistrationconfigurator-2)<br/>

### **AddExecuteActivity\<TActivity, TArguments\>(IRegistrationConfigurator, Type, Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>)**

Adds an execute activity (Courier), allowing configuration when it is configured on the endpoint.

```csharp
public static IExecuteActivityRegistrationConfigurator<TActivity, TArguments> AddExecuteActivity<TActivity, TArguments>(IRegistrationConfigurator configurator, Type executeActivityDefinitionType, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure)
```

#### Type Parameters

`TActivity`<br/>
The activity type

`TArguments`<br/>
The argument type

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`executeActivityDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`configure` [Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IExecuteActivityRegistrationConfigurator\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/iexecuteactivityregistrationconfigurator-2)<br/>

### **AddActivity\<TActivity, TArguments, TLog\>(IRegistrationConfigurator, Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>, Action\<ICompensateActivityConfigurator\<TActivity, TLog\>\>)**

Adds an activity (Courier), allowing configuration when it is configured on the endpoint.

```csharp
public static IActivityRegistrationConfigurator<TActivity, TArguments, TLog> AddActivity<TActivity, TArguments, TLog>(IRegistrationConfigurator configurator, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute, Action<ICompensateActivityConfigurator<TActivity, TLog>> configureCompensate)
```

#### Type Parameters

`TActivity`<br/>
The activity type

`TArguments`<br/>
The argument type

`TLog`<br/>
The log type

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`configureExecute` [Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The execute configuration callback

`configureCompensate` [Action\<ICompensateActivityConfigurator\<TActivity, TLog\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The compensate configuration callback

#### Returns

[IActivityRegistrationConfigurator\<TActivity, TArguments, TLog\>](../../masstransit-abstractions/masstransit/iactivityregistrationconfigurator-3)<br/>

### **AddActivity\<TActivity, TArguments, TLog\>(IRegistrationConfigurator, Type, Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>, Action\<ICompensateActivityConfigurator\<TActivity, TLog\>\>)**

Adds an activity (Courier), allowing configuration when it is configured on the endpoint.

```csharp
public static IActivityRegistrationConfigurator<TActivity, TArguments, TLog> AddActivity<TActivity, TArguments, TLog>(IRegistrationConfigurator configurator, Type activityDefinitionType, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute, Action<ICompensateActivityConfigurator<TActivity, TLog>> configureCompensate)
```

#### Type Parameters

`TActivity`<br/>
The activity type

`TArguments`<br/>
The argument type

`TLog`<br/>
The log type

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`activityDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`configureExecute` [Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The execute configuration callback

`configureCompensate` [Action\<ICompensateActivityConfigurator\<TActivity, TLog\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The compensate configuration callback

#### Returns

[IActivityRegistrationConfigurator\<TActivity, TArguments, TLog\>](../../masstransit-abstractions/masstransit/iactivityregistrationconfigurator-3)<br/>

### **AddConsumer(IRegistrationConfigurator, Type, Type)**

Adds the consumer, along with an optional consumer definition

```csharp
public static IConsumerRegistrationConfigurator AddConsumer(IRegistrationConfigurator configurator, Type consumerType, Type consumerDefinitionType)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`consumerType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The consumer type

`consumerDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The consumer definition type

#### Returns

[IConsumerRegistrationConfigurator](../../masstransit-abstractions/masstransit/iconsumerregistrationconfigurator)<br/>

### **AddSaga(IRegistrationConfigurator, Type, Type)**

Adds the saga, along with an optional saga definition

```csharp
public static ISagaRegistrationConfigurator AddSaga(IRegistrationConfigurator configurator, Type sagaType, Type sagaDefinitionType)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`sagaType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The saga type

`sagaDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The saga definition type

#### Returns

[ISagaRegistrationConfigurator](../masstransit/isagaregistrationconfigurator)<br/>

### **AddSagaStateMachine(IRegistrationConfigurator, Type, Type)**

Adds the state machine saga, along with an optional saga definition

```csharp
public static ISagaRegistrationConfigurator AddSagaStateMachine(IRegistrationConfigurator configurator, Type sagaType, Type sagaDefinitionType)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`sagaType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The saga type

`sagaDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The saga definition type

#### Returns

[ISagaRegistrationConfigurator](../masstransit/isagaregistrationconfigurator)<br/>

### **AddActivity(IRegistrationConfigurator, Type, Type)**

Adds an activity (Courier), along with an optional activity definition

```csharp
public static IActivityRegistrationConfigurator AddActivity(IRegistrationConfigurator configurator, Type activityType, Type activityDefinitionType)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`activityType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`activityDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IActivityRegistrationConfigurator](../../masstransit-abstractions/masstransit/iactivityregistrationconfigurator)<br/>

### **AddExecuteActivity(IRegistrationConfigurator, Type, Type)**

Adds an execute activity (Courier), along with an optional activity definition

```csharp
public static IExecuteActivityRegistrationConfigurator AddExecuteActivity(IRegistrationConfigurator configurator, Type activityType, Type activityDefinitionType)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`activityType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`activityDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IExecuteActivityRegistrationConfigurator](../../masstransit-abstractions/masstransit/iexecuteactivityregistrationconfigurator)<br/>

### **AddFuture(IRegistrationConfigurator, Type, Type)**

Adds a future registration, along with an optional definition

```csharp
public static IFutureRegistrationConfigurator AddFuture(IRegistrationConfigurator configurator, Type futureType, Type futureDefinitionType)
```

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`futureType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`futureDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The future definition type

#### Returns

[IFutureRegistrationConfigurator](../masstransit/ifutureregistrationconfigurator)<br/>
