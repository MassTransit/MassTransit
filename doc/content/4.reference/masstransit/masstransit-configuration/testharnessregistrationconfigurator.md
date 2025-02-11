---

title: TestHarnessRegistrationConfigurator

---

# TestHarnessRegistrationConfigurator

Namespace: MassTransit.Configuration

```csharp
public class TestHarnessRegistrationConfigurator : IBusRegistrationConfigurator, IRegistrationConfigurator, IServiceCollection, IList<ServiceDescriptor>, ICollection<ServiceDescriptor>, IEnumerable<ServiceDescriptor>, IEnumerable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TestHarnessRegistrationConfigurator](../masstransit-configuration/testharnessregistrationconfigurator)<br/>
Implements [IBusRegistrationConfigurator](../masstransit/ibusregistrationconfigurator), [IRegistrationConfigurator](../masstransit/iregistrationconfigurator), IServiceCollection, [IList\<ServiceDescriptor\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1), [ICollection\<ServiceDescriptor\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.icollection-1), [IEnumerable\<ServiceDescriptor\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Properties

### **UseDefaultBusFactory**

```csharp
public bool UseDefaultBusFactory { get; private set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Count**

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **IsReadOnly**

```csharp
public bool IsReadOnly { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Item**

```csharp
public ServiceDescriptor Item { get; set; }
```

#### Property Value

ServiceDescriptor<br/>

### **Registrar**

```csharp
public IContainerRegistrar Registrar { get; }
```

#### Property Value

[IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

## Constructors

### **TestHarnessRegistrationConfigurator(IBusRegistrationConfigurator)**

```csharp
public TestHarnessRegistrationConfigurator(IBusRegistrationConfigurator configurator)
```

#### Parameters

`configurator` [IBusRegistrationConfigurator](../masstransit/ibusregistrationconfigurator)<br/>

## Methods

### **GetEnumerator()**

```csharp
public IEnumerator<ServiceDescriptor> GetEnumerator()
```

#### Returns

[IEnumerator\<ServiceDescriptor\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerator-1)<br/>

### **Add(ServiceDescriptor)**

```csharp
public void Add(ServiceDescriptor item)
```

#### Parameters

`item` ServiceDescriptor<br/>

### **Clear()**

```csharp
public void Clear()
```

### **Contains(ServiceDescriptor)**

```csharp
public bool Contains(ServiceDescriptor item)
```

#### Parameters

`item` ServiceDescriptor<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **CopyTo(ServiceDescriptor[], Int32)**

```csharp
public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
```

#### Parameters

`array` ServiceDescriptor[]<br/>

`arrayIndex` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Remove(ServiceDescriptor)**

```csharp
public bool Remove(ServiceDescriptor item)
```

#### Parameters

`item` ServiceDescriptor<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IndexOf(ServiceDescriptor)**

```csharp
public int IndexOf(ServiceDescriptor item)
```

#### Parameters

`item` ServiceDescriptor<br/>

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Insert(Int32, ServiceDescriptor)**

```csharp
public void Insert(int index, ServiceDescriptor item)
```

#### Parameters

`index` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`item` ServiceDescriptor<br/>

### **RemoveAt(Int32)**

```csharp
public void RemoveAt(int index)
```

#### Parameters

`index` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **AddConsumer\<T\>(Action\<IRegistrationContext, IConsumerConfigurator\<T\>\>)**

```csharp
public IConsumerRegistrationConfigurator<T> AddConsumer<T>(Action<IRegistrationContext, IConsumerConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configure` [Action\<IRegistrationContext, IConsumerConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

#### Returns

[IConsumerRegistrationConfigurator\<T\>](../../masstransit-abstractions/masstransit/iconsumerregistrationconfigurator-1)<br/>

### **AddConsumer\<T\>(Type, Action\<IRegistrationContext, IConsumerConfigurator\<T\>\>)**

```csharp
public IConsumerRegistrationConfigurator<T> AddConsumer<T>(Type consumerDefinitionType, Action<IRegistrationContext, IConsumerConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`consumerDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`configure` [Action\<IRegistrationContext, IConsumerConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

#### Returns

[IConsumerRegistrationConfigurator\<T\>](../../masstransit-abstractions/masstransit/iconsumerregistrationconfigurator-1)<br/>

### **AddSaga\<T\>(Action\<IRegistrationContext, ISagaConfigurator\<T\>\>)**

```csharp
public ISagaRegistrationConfigurator<T> AddSaga<T>(Action<IRegistrationContext, ISagaConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configure` [Action\<IRegistrationContext, ISagaConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

#### Returns

[ISagaRegistrationConfigurator\<T\>](../masstransit/isagaregistrationconfigurator-1)<br/>

### **AddSaga\<T\>(Type, Action\<IRegistrationContext, ISagaConfigurator\<T\>\>)**

```csharp
public ISagaRegistrationConfigurator<T> AddSaga<T>(Type sagaDefinitionType, Action<IRegistrationContext, ISagaConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`sagaDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`configure` [Action\<IRegistrationContext, ISagaConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

#### Returns

[ISagaRegistrationConfigurator\<T\>](../masstransit/isagaregistrationconfigurator-1)<br/>

### **AddSagaStateMachine\<TStateMachine, T\>(Action\<IRegistrationContext, ISagaConfigurator\<T\>\>)**

```csharp
public ISagaRegistrationConfigurator<T> AddSagaStateMachine<TStateMachine, T>(Action<IRegistrationContext, ISagaConfigurator<T>> configure)
```

#### Type Parameters

`TStateMachine`<br/>

`T`<br/>

#### Parameters

`configure` [Action\<IRegistrationContext, ISagaConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

#### Returns

[ISagaRegistrationConfigurator\<T\>](../masstransit/isagaregistrationconfigurator-1)<br/>

### **AddSagaStateMachine\<TStateMachine, T\>(Type, Action\<IRegistrationContext, ISagaConfigurator\<T\>\>)**

```csharp
public ISagaRegistrationConfigurator<T> AddSagaStateMachine<TStateMachine, T>(Type sagaDefinitionType, Action<IRegistrationContext, ISagaConfigurator<T>> configure)
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

```csharp
public IExecuteActivityRegistrationConfigurator<TActivity, TArguments> AddExecuteActivity<TActivity, TArguments>(Action<IRegistrationContext, IExecuteActivityConfigurator<TActivity, TArguments>> configure)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`configure` [Action\<IRegistrationContext, IExecuteActivityConfigurator\<TActivity, TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

#### Returns

[IExecuteActivityRegistrationConfigurator\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/iexecuteactivityregistrationconfigurator-2)<br/>

### **AddExecuteActivity\<TActivity, TArguments\>(Type, Action\<IRegistrationContext, IExecuteActivityConfigurator\<TActivity, TArguments\>\>)**

```csharp
public IExecuteActivityRegistrationConfigurator<TActivity, TArguments> AddExecuteActivity<TActivity, TArguments>(Type executeActivityDefinitionType, Action<IRegistrationContext, IExecuteActivityConfigurator<TActivity, TArguments>> configure)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`executeActivityDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`configure` [Action\<IRegistrationContext, IExecuteActivityConfigurator\<TActivity, TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

#### Returns

[IExecuteActivityRegistrationConfigurator\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/iexecuteactivityregistrationconfigurator-2)<br/>

### **AddActivity\<TActivity, TArguments, TLog\>(Action\<IRegistrationContext, IExecuteActivityConfigurator\<TActivity, TArguments\>\>, Action\<IRegistrationContext, ICompensateActivityConfigurator\<TActivity, TLog\>\>)**

```csharp
public IActivityRegistrationConfigurator<TActivity, TArguments, TLog> AddActivity<TActivity, TArguments, TLog>(Action<IRegistrationContext, IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute, Action<IRegistrationContext, ICompensateActivityConfigurator<TActivity, TLog>> configureCompensate)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

`TLog`<br/>

#### Parameters

`configureExecute` [Action\<IRegistrationContext, IExecuteActivityConfigurator\<TActivity, TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

`configureCompensate` [Action\<IRegistrationContext, ICompensateActivityConfigurator\<TActivity, TLog\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

#### Returns

[IActivityRegistrationConfigurator\<TActivity, TArguments, TLog\>](../../masstransit-abstractions/masstransit/iactivityregistrationconfigurator-3)<br/>

### **AddActivity\<TActivity, TArguments, TLog\>(Type, Action\<IRegistrationContext, IExecuteActivityConfigurator\<TActivity, TArguments\>\>, Action\<IRegistrationContext, ICompensateActivityConfigurator\<TActivity, TLog\>\>)**

```csharp
public IActivityRegistrationConfigurator<TActivity, TArguments, TLog> AddActivity<TActivity, TArguments, TLog>(Type activityDefinitionType, Action<IRegistrationContext, IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute, Action<IRegistrationContext, ICompensateActivityConfigurator<TActivity, TLog>> configureCompensate)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

`TLog`<br/>

#### Parameters

`activityDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`configureExecute` [Action\<IRegistrationContext, IExecuteActivityConfigurator\<TActivity, TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

`configureCompensate` [Action\<IRegistrationContext, ICompensateActivityConfigurator\<TActivity, TLog\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

#### Returns

[IActivityRegistrationConfigurator\<TActivity, TArguments, TLog\>](../../masstransit-abstractions/masstransit/iactivityregistrationconfigurator-3)<br/>

### **AddEndpoint(Type)**

```csharp
public void AddEndpoint(Type endpointDefinition)
```

#### Parameters

`endpointDefinition` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **AddEndpoint\<TDefinition, T\>(IRegistration, IEndpointSettings\<IEndpointDefinition\<T\>\>)**

```csharp
public void AddEndpoint<TDefinition, T>(IRegistration registration, IEndpointSettings<IEndpointDefinition<T>> settings)
```

#### Type Parameters

`TDefinition`<br/>

`T`<br/>

#### Parameters

`registration` [IRegistration](../masstransit-configuration/iregistration)<br/>

`settings` [IEndpointSettings\<IEndpointDefinition\<T\>\>](../../masstransit-abstractions/masstransit/iendpointsettings-1)<br/>

### **AddRequestClient\<T\>(RequestTimeout)**

```csharp
public void AddRequestClient<T>(RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>

#### Parameters

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

### **AddRequestClient\<T\>(Uri, RequestTimeout)**

```csharp
public void AddRequestClient<T>(Uri destinationAddress, RequestTimeout timeout)
```

#### Type Parameters

`T`<br/>

#### Parameters

`destinationAddress` Uri<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

### **AddRequestClient(Type, RequestTimeout)**

```csharp
public void AddRequestClient(Type requestType, RequestTimeout timeout)
```

#### Parameters

`requestType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

### **AddRequestClient(Type, Uri, RequestTimeout)**

```csharp
public void AddRequestClient(Type requestType, Uri destinationAddress, RequestTimeout timeout)
```

#### Parameters

`requestType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`destinationAddress` Uri<br/>

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

### **SetDefaultRequestTimeout(RequestTimeout)**

```csharp
public void SetDefaultRequestTimeout(RequestTimeout timeout)
```

#### Parameters

`timeout` [RequestTimeout](../../masstransit-abstractions/masstransit/requesttimeout)<br/>

### **SetDefaultRequestTimeout(Nullable\<Int32\>, Nullable\<Int32\>, Nullable\<Int32\>, Nullable\<Int32\>, Nullable\<Int32\>)**

```csharp
public void SetDefaultRequestTimeout(Nullable<int> d, Nullable<int> h, Nullable<int> m, Nullable<int> s, Nullable<int> ms)
```

#### Parameters

`d` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`h` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`m` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`s` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`ms` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **SetEndpointNameFormatter(IEndpointNameFormatter)**

```csharp
public void SetEndpointNameFormatter(IEndpointNameFormatter endpointNameFormatter)
```

#### Parameters

`endpointNameFormatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

### **AddSagaRepository\<T\>()**

```csharp
public ISagaRegistrationConfigurator<T> AddSagaRepository<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[ISagaRegistrationConfigurator\<T\>](../masstransit/isagaregistrationconfigurator-1)<br/>

### **SetSagaRepositoryProvider(ISagaRepositoryRegistrationProvider)**

```csharp
public void SetSagaRepositoryProvider(ISagaRepositoryRegistrationProvider provider)
```

#### Parameters

`provider` [ISagaRepositoryRegistrationProvider](../masstransit-configuration/isagarepositoryregistrationprovider)<br/>

### **AddFuture\<TFuture\>(Type)**

```csharp
public IFutureRegistrationConfigurator<TFuture> AddFuture<TFuture>(Type futureDefinitionType)
```

#### Type Parameters

`TFuture`<br/>

#### Parameters

`futureDefinitionType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IFutureRegistrationConfigurator\<TFuture\>](../masstransit/ifutureregistrationconfigurator-1)<br/>

### **AddConfigureEndpointsCallback(ConfigureEndpointsCallback)**

```csharp
public void AddConfigureEndpointsCallback(ConfigureEndpointsCallback callback)
```

#### Parameters

`callback` [ConfigureEndpointsCallback](../masstransit/configureendpointscallback)<br/>

### **AddConfigureEndpointsCallback(ConfigureEndpointsProviderCallback)**

```csharp
public void AddConfigureEndpointsCallback(ConfigureEndpointsProviderCallback callback)
```

#### Parameters

`callback` [ConfigureEndpointsProviderCallback](../masstransit/configureendpointsprovidercallback)<br/>

### **SetRequestClientFactory(Func\<IBus, RequestTimeout, IClientFactory\>)**

```csharp
public void SetRequestClientFactory(Func<IBus, RequestTimeout, IClientFactory> clientFactory)
```

#### Parameters

`clientFactory` [Func\<IBus, RequestTimeout, IClientFactory\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

### **AddBus(Func\<IBusRegistrationContext, IBusControl\>)**

#### Caution

Use 'Using[TransportName]' instead. Visit https://masstransit.io/obsolete for details.

---

```csharp
public void AddBus(Func<IBusRegistrationContext, IBusControl> busFactory)
```

#### Parameters

`busFactory` [Func\<IBusRegistrationContext, IBusControl\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **SetBusFactory\<T\>(T)**

```csharp
public void SetBusFactory<T>(T busFactory)
```

#### Type Parameters

`T`<br/>

#### Parameters

`busFactory` T<br/>

### **AddRider(Action\<IRiderRegistrationConfigurator\>)**

```csharp
public void AddRider(Action<IRiderRegistrationConfigurator> configure)
```

#### Parameters

`configure` [Action\<IRiderRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
