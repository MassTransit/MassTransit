---

title: IRegistrationConfigurator

---

# IRegistrationConfigurator

Namespace: MassTransit

```csharp
public interface IRegistrationConfigurator : IServiceCollection, IList<ServiceDescriptor>, ICollection<ServiceDescriptor>, IEnumerable<ServiceDescriptor>, IEnumerable
```

Implements IServiceCollection, [IList\<ServiceDescriptor\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1), [ICollection\<ServiceDescriptor\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.icollection-1), [IEnumerable\<ServiceDescriptor\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Methods

### **AddConsumer\<T\>(Action\<IRegistrationContext, IConsumerConfigurator\<T\>\>)**

Adds the consumer, allowing configuration when it is configured on an endpoint

```csharp
IConsumerRegistrationConfigurator<T> AddConsumer<T>(Action<IRegistrationContext, IConsumerConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>
The consumer type

#### Parameters

`configure` [Action\<IRegistrationContext, IConsumerConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

#### Returns

[IConsumerRegistrationConfigurator\<T\>](../../masstransit-abstractions/masstransit/iconsumerregistrationconfigurator-1)<br/>

### **AddConsumer\<T\>(Type, Action\<IRegistrationContext, IConsumerConfigurator\<T\>\>)**

Adds the consumer, allowing configuration when it is configured on an endpoint

```csharp
IConsumerRegistrationConfigurator<T> AddConsumer<T>(Type consumerDefinitionType, Action<IRegistrationContext, IConsumerConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>
The consumer type

#### Parameters

`consumerDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The consumer definition type

`configure` [Action\<IRegistrationContext, IConsumerConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

#### Returns

[IConsumerRegistrationConfigurator\<T\>](../../masstransit-abstractions/masstransit/iconsumerregistrationconfigurator-1)<br/>

### **AddSaga\<T\>(Action\<IRegistrationContext, ISagaConfigurator\<T\>\>)**

Adds the saga, allowing configuration when it is configured on the endpoint. This should not
 be used for state machine (Automatonymous) sagas.

```csharp
ISagaRegistrationConfigurator<T> AddSaga<T>(Action<IRegistrationContext, ISagaConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>
The saga type

#### Parameters

`configure` [Action\<IRegistrationContext, ISagaConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

#### Returns

[ISagaRegistrationConfigurator\<T\>](../masstransit/isagaregistrationconfigurator-1)<br/>

### **AddSaga\<T\>(Type, Action\<IRegistrationContext, ISagaConfigurator\<T\>\>)**

Adds the saga, allowing configuration when it is configured on the endpoint. This should not
 be used for state machine (Automatonymous) sagas.

```csharp
ISagaRegistrationConfigurator<T> AddSaga<T>(Type sagaDefinitionType, Action<IRegistrationContext, ISagaConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>
The saga type

#### Parameters

`sagaDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The saga definition type

`configure` [Action\<IRegistrationContext, ISagaConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

#### Returns

[ISagaRegistrationConfigurator\<T\>](../masstransit/isagaregistrationconfigurator-1)<br/>

### **AddSagaStateMachine\<TStateMachine, T\>(Action\<IRegistrationContext, ISagaConfigurator\<T\>\>)**

Adds a SagaStateMachine to the registry, using the factory method, and updates the registrar prior to registering so that the default
 saga registrar isn't notified.

```csharp
ISagaRegistrationConfigurator<T> AddSagaStateMachine<TStateMachine, T>(Action<IRegistrationContext, ISagaConfigurator<T>> configure)
```

#### Type Parameters

`TStateMachine`<br/>

`T`<br/>

#### Parameters

`configure` [Action\<IRegistrationContext, ISagaConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

#### Returns

[ISagaRegistrationConfigurator\<T\>](../masstransit/isagaregistrationconfigurator-1)<br/>

### **AddSagaStateMachine\<TStateMachine, T\>(Type, Action\<IRegistrationContext, ISagaConfigurator\<T\>\>)**

Adds a SagaStateMachine to the registry, using the factory method, and updates the registrar prior to registering so that the default
 saga registrar isn't notified.

```csharp
ISagaRegistrationConfigurator<T> AddSagaStateMachine<TStateMachine, T>(Type sagaDefinitionType, Action<IRegistrationContext, ISagaConfigurator<T>> configure)
```

#### Type Parameters

`TStateMachine`<br/>

`T`<br/>

#### Parameters

`sagaDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`configure` [Action\<IRegistrationContext, ISagaConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

#### Returns

[ISagaRegistrationConfigurator\<T\>](../masstransit/isagaregistrationconfigurator-1)<br/>

### **AddExecuteActivity\<TActivity, TArguments\>(Action\<IRegistrationContext, IExecuteActivityConfigurator\<TActivity, TArguments\>\>)**

Adds an execute activity (Courier), allowing configuration when it is configured on the endpoint.

```csharp
IExecuteActivityRegistrationConfigurator<TActivity, TArguments> AddExecuteActivity<TActivity, TArguments>(Action<IRegistrationContext, IExecuteActivityConfigurator<TActivity, TArguments>> configure)
```

#### Type Parameters

`TActivity`<br/>
The activity type

`TArguments`<br/>
The argument type

#### Parameters

`configure` [Action\<IRegistrationContext, IExecuteActivityConfigurator\<TActivity, TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

#### Returns

[IExecuteActivityRegistrationConfigurator\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/iexecuteactivityregistrationconfigurator-2)<br/>

### **AddExecuteActivity\<TActivity, TArguments\>(Type, Action\<IRegistrationContext, IExecuteActivityConfigurator\<TActivity, TArguments\>\>)**

Adds an execute activity (Courier), allowing configuration when it is configured on the endpoint.

```csharp
IExecuteActivityRegistrationConfigurator<TActivity, TArguments> AddExecuteActivity<TActivity, TArguments>(Type executeActivityDefinitionType, Action<IRegistrationContext, IExecuteActivityConfigurator<TActivity, TArguments>> configure)
```

#### Type Parameters

`TActivity`<br/>
The activity type

`TArguments`<br/>
The argument type

#### Parameters

`executeActivityDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`configure` [Action\<IRegistrationContext, IExecuteActivityConfigurator\<TActivity, TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

#### Returns

[IExecuteActivityRegistrationConfigurator\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/iexecuteactivityregistrationconfigurator-2)<br/>

### **AddActivity\<TActivity, TArguments, TLog\>(Action\<IRegistrationContext, IExecuteActivityConfigurator\<TActivity, TArguments\>\>, Action\<IRegistrationContext, ICompensateActivityConfigurator\<TActivity, TLog\>\>)**

Adds an activity (Courier), allowing configuration when it is configured on the endpoint.

```csharp
IActivityRegistrationConfigurator<TActivity, TArguments, TLog> AddActivity<TActivity, TArguments, TLog>(Action<IRegistrationContext, IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute, Action<IRegistrationContext, ICompensateActivityConfigurator<TActivity, TLog>> configureCompensate)
```

#### Type Parameters

`TActivity`<br/>
The activity type

`TArguments`<br/>
The argument type

`TLog`<br/>
The log type

#### Parameters

`configureExecute` [Action\<IRegistrationContext, IExecuteActivityConfigurator\<TActivity, TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>
The execute configuration callback

`configureCompensate` [Action\<IRegistrationContext, ICompensateActivityConfigurator\<TActivity, TLog\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>
The compensate configuration callback

#### Returns

[IActivityRegistrationConfigurator\<TActivity, TArguments, TLog\>](../../masstransit-abstractions/masstransit/iactivityregistrationconfigurator-3)<br/>

### **AddActivity\<TActivity, TArguments, TLog\>(Type, Action\<IRegistrationContext, IExecuteActivityConfigurator\<TActivity, TArguments\>\>, Action\<IRegistrationContext, ICompensateActivityConfigurator\<TActivity, TLog\>\>)**

Adds an activity (Courier), allowing configuration when it is configured on the endpoint.

```csharp
IActivityRegistrationConfigurator<TActivity, TArguments, TLog> AddActivity<TActivity, TArguments, TLog>(Type activityDefinitionType, Action<IRegistrationContext, IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute, Action<IRegistrationContext, ICompensateActivityConfigurator<TActivity, TLog>> configureCompensate)
```

#### Type Parameters

`TActivity`<br/>
The activity type

`TArguments`<br/>
The argument type

`TLog`<br/>
The log type

#### Parameters

`activityDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`configureExecute` [Action\<IRegistrationContext, IExecuteActivityConfigurator\<TActivity, TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>
The execute configuration callback

`configureCompensate` [Action\<IRegistrationContext, ICompensateActivityConfigurator\<TActivity, TLog\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>
The compensate configuration callback

#### Returns

[IActivityRegistrationConfigurator\<TActivity, TArguments, TLog\>](../../masstransit-abstractions/masstransit/iactivityregistrationconfigurator-3)<br/>

### **AddEndpoint(Type)**

Adds an endpoint definition, which will to used for consumers, sagas, etc. that are on that same endpoint. If a consumer, etc.
 specifies an endpoint without a definition, the default endpoint definition is used if one cannot be resolved from the configuration
 service provider (via generic registration).

```csharp
void AddEndpoint(Type endpointDefinition)
```

#### Parameters

`endpointDefinition` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The endpoint definition to add

### **AddEndpoint\<TDefinition, T\>(IRegistration, IEndpointSettings\<IEndpointDefinition\<T\>\>)**

```csharp
void AddEndpoint<TDefinition, T>(IRegistration registration, IEndpointSettings<IEndpointDefinition<T>> settings)
```

#### Type Parameters

`TDefinition`<br/>

`T`<br/>

#### Parameters

`registration` [IRegistration](../masstransit-configuration/iregistration)<br/>

`settings` [IEndpointSettings\<IEndpointDefinition\<T\>\>](../../masstransit-abstractions/masstransit/iendpointsettings-1)<br/>

### **AddRequestClient\<T\>(RequestTimeout)**

Add a request client, for the request type, which uses the  if present, otherwise
 uses the . The request is published, unless an endpoint convention is specified for the
 request type.

```csharp
void AddRequestClient<T>(RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>
The request message type

#### Parameters

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>
The request timeout

### **AddRequestClient\<T\>(Uri, RequestTimeout)**

Add a request client, for the request type, which uses the  if present, otherwise
 uses the .

```csharp
void AddRequestClient<T>(Uri destinationAddress, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>
The request message type

#### Parameters

`destinationAddress` Uri<br/>
The destination address for the request

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>
The request timeout

### **AddRequestClient(Type, RequestTimeout)**

Add a request client, for the request type, which uses the  if present, otherwise
 uses the . The request is published, unless an endpoint convention is specified for the
 request type.

```csharp
void AddRequestClient(Type requestType, RequestTimeout timeout)
```

#### Parameters

`requestType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The request message type

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>
The request timeout

### **AddRequestClient(Type, Uri, RequestTimeout)**

Add a request client, for the request type, which uses the  if present, otherwise
 uses the .

```csharp
void AddRequestClient(Type requestType, Uri destinationAddress, RequestTimeout timeout)
```

#### Parameters

`requestType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The request message type

`destinationAddress` Uri<br/>
The destination address for the request

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>
The request timeout

### **SetDefaultRequestTimeout(RequestTimeout)**

Sets the default request timeout for this bus instance, used by the client factory to create request clients

```csharp
void SetDefaultRequestTimeout(RequestTimeout timeout)
```

#### Parameters

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

### **SetDefaultRequestTimeout(Nullable\<Int32\>, Nullable\<Int32\>, Nullable\<Int32\>, Nullable\<Int32\>, Nullable\<Int32\>)**

Sets the default request timeout for this bus instance, used by the client factory to create request clients

```csharp
void SetDefaultRequestTimeout(Nullable<int> d, Nullable<int> h, Nullable<int> m, Nullable<int> s, Nullable<int> ms)
```

#### Parameters

`d` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
days

`h` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
hours

`m` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
minutes

`s` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
seconds

`ms` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
milliseconds

### **SetEndpointNameFormatter(IEndpointNameFormatter)**

Set the default endpoint name formatter used for endpoint names

```csharp
void SetEndpointNameFormatter(IEndpointNameFormatter endpointNameFormatter)
```

#### Parameters

`endpointNameFormatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

### **AddSagaRepository\<T\>()**

Add a saga repository for the specified saga type, by specifying the repository type via method chaining. Using this
 method alone does nothing, it should be followed with the appropriate repository configuration method.

```csharp
ISagaRegistrationConfigurator<T> AddSagaRepository<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[ISagaRegistrationConfigurator\<T\>](../masstransit/isagaregistrationconfigurator-1)<br/>

### **SetSagaRepositoryProvider(ISagaRepositoryRegistrationProvider)**

Specify a saga repository provider, that will be called when a saga is configured by type
 (without a specific generic call to AddSaga/AddSagaStateMachine)

```csharp
void SetSagaRepositoryProvider(ISagaRepositoryRegistrationProvider provider)
```

#### Parameters

`provider` [ISagaRepositoryRegistrationProvider](../masstransit-configuration/isagarepositoryregistrationprovider)<br/>

### **AddFuture\<TFuture\>(Type)**

Adds a future registration, along with an optional definition

```csharp
IFutureRegistrationConfigurator<TFuture> AddFuture<TFuture>(Type futureDefinitionType)
```

#### Type Parameters

`TFuture`<br/>

#### Parameters

`futureDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The future definition type

#### Returns

[IFutureRegistrationConfigurator\<TFuture\>](../masstransit/ifutureregistrationconfigurator-1)<br/>
