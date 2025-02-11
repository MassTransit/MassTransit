---

title: RegistrationContextExtensions

---

# RegistrationContextExtensions

Namespace: MassTransit

```csharp
public static class RegistrationContextExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RegistrationContextExtensions](../masstransit/registrationcontextextensions)

## Methods

### **ConfigureEndpoints\<T\>(IBusFactoryConfigurator\<T\>, IBusRegistrationContext, IEndpointNameFormatter)**

Configure the endpoints for all defined consumer, saga, and activity types using an optional
 endpoint name formatter. If no endpoint name formatter is specified and an 
 is registered in the container, it is resolved from the container. Otherwise, the [DefaultEndpointNameFormatter](../masstransit/defaultendpointnameformatter)
 is used.

```csharp
public static void ConfigureEndpoints<T>(IBusFactoryConfigurator<T> configurator, IBusRegistrationContext registration, IEndpointNameFormatter endpointNameFormatter)
```

#### Type Parameters

`T`<br/>
The bus factory type (depends upon the transport)

#### Parameters

`configurator` [IBusFactoryConfigurator\<T\>](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator-1)<br/>
The  for the bus being configured

`registration` [IBusRegistrationContext](../masstransit/ibusregistrationcontext)<br/>
The registration for this bus instance

`endpointNameFormatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>
Optional, the endpoint name formatter

### **ConfigureEndpoints\<T\>(IBusFactoryConfigurator\<T\>, IBusRegistrationContext, Action\<IRegistrationFilterConfigurator\>, IEndpointNameFormatter)**

Configure the endpoints for all defined consumer, saga, and activity types using an optional
 endpoint name formatter. If no endpoint name formatter is specified and an 
 is registered in the container, it is resolved from the container. Otherwise, the [DefaultEndpointNameFormatter](../masstransit/defaultendpointnameformatter)
 is used.

```csharp
public static void ConfigureEndpoints<T>(IBusFactoryConfigurator<T> configurator, IBusRegistrationContext registration, Action<IRegistrationFilterConfigurator> configureFilter, IEndpointNameFormatter endpointNameFormatter)
```

#### Type Parameters

`T`<br/>
The bus factory type (depends upon the transport)

#### Parameters

`configurator` [IBusFactoryConfigurator\<T\>](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator-1)<br/>
The  for the bus being configured

`registration` [IBusRegistrationContext](../masstransit/ibusregistrationcontext)<br/>
The registration for this bus instance

`configureFilter` [Action\<IRegistrationFilterConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Filter the configured consumers, sagas, and activities

`endpointNameFormatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>
Optional, the endpoint name formatter

### **ConfigureEndpoints\<T\>(IServiceInstanceConfigurator\<T\>, IBusRegistrationContext, IEndpointNameFormatter)**

#### Caution

Job Consumers no longer require a service instance. Visit https://masstransit.io/obsolete for details.

---

Configure the endpoints for all defined consumer, saga, and activity types using an optional
 endpoint name formatter. If no endpoint name formatter is specified and an 
 is registered in the container, it is resolved from the container. Otherwise, the [DefaultEndpointNameFormatter](../masstransit/defaultendpointnameformatter)
 is used.

```csharp
public static void ConfigureEndpoints<T>(IServiceInstanceConfigurator<T> configurator, IBusRegistrationContext registration, IEndpointNameFormatter endpointNameFormatter)
```

#### Type Parameters

`T`<br/>
The bus factory type (depends upon the transport)

#### Parameters

`configurator` [IServiceInstanceConfigurator\<T\>](../../masstransit-abstractions/masstransit/iserviceinstanceconfigurator-1)<br/>
The  for the bus being configured

`registration` [IBusRegistrationContext](../masstransit/ibusregistrationcontext)<br/>
The registration for this bus instance

`endpointNameFormatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>
Optional, the endpoint name formatter

### **ConfigureEndpoints\<T\>(IServiceInstanceConfigurator\<T\>, IBusRegistrationContext, Action\<IRegistrationFilterConfigurator\>, IEndpointNameFormatter)**

#### Caution

Job Consumers no longer require a service instance. Visit https://masstransit.io/obsolete for details.

---

Configure the endpoints for all defined consumer, saga, and activity types using an optional
 endpoint name formatter. If no endpoint name formatter is specified and an 
 is registered in the container, it is resolved from the container. Otherwise, the [DefaultEndpointNameFormatter](../masstransit/defaultendpointnameformatter)
 is used.

```csharp
public static void ConfigureEndpoints<T>(IServiceInstanceConfigurator<T> configurator, IBusRegistrationContext registration, Action<IRegistrationFilterConfigurator> configureFilter, IEndpointNameFormatter endpointNameFormatter)
```

#### Type Parameters

`T`<br/>
The bus factory type (depends upon the transport)

#### Parameters

`configurator` [IServiceInstanceConfigurator\<T\>](../../masstransit-abstractions/masstransit/iserviceinstanceconfigurator-1)<br/>
The  for the bus being configured

`registration` [IBusRegistrationContext](../masstransit/ibusregistrationcontext)<br/>
The registration for this bus instance

`configureFilter` [Action\<IRegistrationFilterConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Filter the configured consumers, sagas, and activities

`endpointNameFormatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>
Optional, the endpoint name formatter

### **ConfigureServiceEndpoints\<T\>(IBusFactoryConfigurator\<T\>, IBusRegistrationContext, Action\<IRegistrationFilterConfigurator\>, ServiceInstanceOptions)**

#### Caution

Job Consumers no longer require a service instance. Visit https://masstransit.io/obsolete for details.

---

Configure a service instance for use with the job service

```csharp
public static void ConfigureServiceEndpoints<T>(IBusFactoryConfigurator<T> configurator, IBusRegistrationContext registration, Action<IRegistrationFilterConfigurator> configureFilter, ServiceInstanceOptions options)
```

#### Type Parameters

`T`<br/>
The bus factory type (depends upon the transport)

#### Parameters

`configurator` [IBusFactoryConfigurator\<T\>](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator-1)<br/>
The  for the bus being configured

`registration` [IBusRegistrationContext](../masstransit/ibusregistrationcontext)<br/>
The registration for this bus instance

`configureFilter` [Action\<IRegistrationFilterConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Filter the configured consumers, sagas, and activities

`options` [ServiceInstanceOptions](../masstransit/serviceinstanceoptions)<br/>
Optional service instance options to start

### **ConfigureServiceEndpoints\<T\>(IBusFactoryConfigurator\<T\>, IBusRegistrationContext, ServiceInstanceOptions)**

#### Caution

Job Consumers no longer require a service instance. Visit https://masstransit.io/obsolete for details.

---

Configure a service instance for use with the job service

```csharp
public static void ConfigureServiceEndpoints<T>(IBusFactoryConfigurator<T> configurator, IBusRegistrationContext registration, ServiceInstanceOptions options)
```

#### Type Parameters

`T`<br/>
The bus factory type (depends upon the transport)

#### Parameters

`configurator` [IBusFactoryConfigurator\<T\>](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator-1)<br/>
The  for the bus being configured

`registration` [IBusRegistrationContext](../masstransit/ibusregistrationcontext)<br/>
The registration for this bus instance

`options` [ServiceInstanceOptions](../masstransit/serviceinstanceoptions)<br/>
Optional service instance options to start

### **ConfigureConsumer(IReceiveEndpointConfigurator, IRegistrationContext, Type)**

Configure a consumer on the receive endpoint

```csharp
public static void ConfigureConsumer(IReceiveEndpointConfigurator configurator, IRegistrationContext registration, Type consumerType)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`registration` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
The registration for this bus instance

`consumerType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The consumer type

### **ConfigureConsumer\<T\>(IReceiveEndpointConfigurator, IRegistrationContext, Action\<IConsumerConfigurator\<T\>\>)**

Configure a consumer on the receive endpoint, with an optional configuration action

```csharp
public static void ConfigureConsumer<T>(IReceiveEndpointConfigurator configurator, IRegistrationContext registration, Action<IConsumerConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>
The consumer type

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`registration` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
The registration for this bus instance

`configure` [Action\<IConsumerConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ConfigureConsumers(IReceiveEndpointConfigurator, IRegistrationContext)**

Configure all registered consumers on the receive endpoint

```csharp
public static void ConfigureConsumers(IReceiveEndpointConfigurator configurator, IRegistrationContext registration)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`registration` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
The registration for this bus instance

### **ConfigureSaga(IReceiveEndpointConfigurator, IRegistrationContext, Type)**

Configure a saga on the receive endpoint

```csharp
public static void ConfigureSaga(IReceiveEndpointConfigurator configurator, IRegistrationContext registration, Type sagaType)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`registration` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
The registration for this bus instance

`sagaType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The saga type

### **ConfigureSaga\<T\>(IReceiveEndpointConfigurator, IRegistrationContext, Action\<ISagaConfigurator\<T\>\>)**

Configure a saga on the receive endpoint, with an optional configuration action

```csharp
public static void ConfigureSaga<T>(IReceiveEndpointConfigurator configurator, IRegistrationContext registration, Action<ISagaConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>
The saga type

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`registration` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
The registration for this bus instance

`configure` [Action\<ISagaConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ConfigureSagas(IReceiveEndpointConfigurator, IRegistrationContext)**

Configure all registered sagas on the receive endpoint

```csharp
public static void ConfigureSagas(IReceiveEndpointConfigurator configurator, IRegistrationContext registration)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`registration` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
The registration for this bus instance

### **ConfigureActivity(IReceiveEndpointConfigurator, IReceiveEndpointConfigurator, IRegistrationContext, Type)**

Configure the specified activity type

```csharp
public static void ConfigureActivity(IReceiveEndpointConfigurator configurator, IReceiveEndpointConfigurator compensateEndpointConfigurator, IRegistrationContext registration, Type activityType)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`compensateEndpointConfigurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>
The configurator for the compensate activity endpoint

`registration` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
The registration for this bus instance

`activityType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **ConfigureExecuteActivity(IReceiveEndpointConfigurator, IRegistrationContext, Type)**

Configure the specified execute activity type

```csharp
public static void ConfigureExecuteActivity(IReceiveEndpointConfigurator configurator, IRegistrationContext registration, Type activityType)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`registration` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
The registration for this bus instance

`activityType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **ConfigureActivityExecute(IReceiveEndpointConfigurator, IRegistrationContext, Type, Uri)**

Configure the specified activity type

```csharp
public static void ConfigureActivityExecute(IReceiveEndpointConfigurator configurator, IRegistrationContext registration, Type activityType, Uri compensateAddress)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>
The configurator for the execute activity endpoint

`registration` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
The registration for this bus instance

`activityType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`compensateAddress` Uri<br/>

### **ConfigureActivityCompensate(IReceiveEndpointConfigurator, IRegistrationContext, Type)**

Configure the specified activity type

```csharp
public static void ConfigureActivityCompensate(IReceiveEndpointConfigurator configurator, IRegistrationContext registration, Type activityType)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>
The configurator for the compensate activity endpoint

`registration` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
The registration for this bus instance

`activityType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
