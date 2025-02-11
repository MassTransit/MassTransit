---

title: IRegistrationContext

---

# IRegistrationContext

Namespace: MassTransit

Registration contains the consumers and sagas that have been registered, allowing them to be configured on one or more
 receive endpoints.

```csharp
public interface IRegistrationContext : IServiceProvider
```

Implements IServiceProvider

## Methods

### **ConfigureConsumer(Type, IReceiveEndpointConfigurator)**

Configure a consumer on the receive endpoint

```csharp
void ConfigureConsumer(Type consumerType, IReceiveEndpointConfigurator configurator)
```

#### Parameters

`consumerType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The consumer type

`configurator` [IReceiveEndpointConfigurator](../masstransit/ireceiveendpointconfigurator)<br/>

### **ConfigureConsumer\<T\>(IReceiveEndpointConfigurator, Action\<IConsumerConfigurator\<T\>\>)**

Configure a consumer on the receive endpoint, with an optional configuration action

```csharp
void ConfigureConsumer<T>(IReceiveEndpointConfigurator configurator, Action<IConsumerConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>
The consumer type

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../masstransit/ireceiveendpointconfigurator)<br/>

`configure` [Action\<IConsumerConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ConfigureConsumers(IReceiveEndpointConfigurator)**

Configure all registered consumers on the receive endpoint

```csharp
void ConfigureConsumers(IReceiveEndpointConfigurator configurator)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../masstransit/ireceiveendpointconfigurator)<br/>

### **ConfigureSaga(Type, IReceiveEndpointConfigurator)**

Configure a saga on the receive endpoint

```csharp
void ConfigureSaga(Type sagaType, IReceiveEndpointConfigurator configurator)
```

#### Parameters

`sagaType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The saga type

`configurator` [IReceiveEndpointConfigurator](../masstransit/ireceiveendpointconfigurator)<br/>

### **ConfigureSaga\<T\>(IReceiveEndpointConfigurator, Action\<ISagaConfigurator\<T\>\>)**

Configure a saga on the receive endpoint, with an optional configuration action

```csharp
void ConfigureSaga<T>(IReceiveEndpointConfigurator configurator, Action<ISagaConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>
The saga type

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../masstransit/ireceiveendpointconfigurator)<br/>

`configure` [Action\<ISagaConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ConfigureSagas(IReceiveEndpointConfigurator)**

Configure all registered sagas on the receive endpoint

```csharp
void ConfigureSagas(IReceiveEndpointConfigurator configurator)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../masstransit/ireceiveendpointconfigurator)<br/>

### **ConfigureExecuteActivity(Type, IReceiveEndpointConfigurator)**

Configure the specified execute activity type

```csharp
void ConfigureExecuteActivity(Type activityType, IReceiveEndpointConfigurator configurator)
```

#### Parameters

`activityType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`configurator` [IReceiveEndpointConfigurator](../masstransit/ireceiveendpointconfigurator)<br/>

### **ConfigureActivity(Type, IReceiveEndpointConfigurator, IReceiveEndpointConfigurator)**

Configure the specified activity type

```csharp
void ConfigureActivity(Type activityType, IReceiveEndpointConfigurator executeEndpointConfigurator, IReceiveEndpointConfigurator compensateEndpointConfigurator)
```

#### Parameters

`activityType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`executeEndpointConfigurator` [IReceiveEndpointConfigurator](../masstransit/ireceiveendpointconfigurator)<br/>
The configurator for the execute activity endpoint

`compensateEndpointConfigurator` [IReceiveEndpointConfigurator](../masstransit/ireceiveendpointconfigurator)<br/>
The configurator for the compensate activity endpoint

### **ConfigureActivityExecute(Type, IReceiveEndpointConfigurator, Uri)**

Configure the specified activity type

```csharp
void ConfigureActivityExecute(Type activityType, IReceiveEndpointConfigurator executeEndpointConfigurator, Uri compensateAddress)
```

#### Parameters

`activityType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`executeEndpointConfigurator` [IReceiveEndpointConfigurator](../masstransit/ireceiveendpointconfigurator)<br/>
The configurator for the execute activity endpoint

`compensateAddress` Uri<br/>

### **ConfigureActivityCompensate(Type, IReceiveEndpointConfigurator)**

Configure the specified activity type

```csharp
void ConfigureActivityCompensate(Type activityType, IReceiveEndpointConfigurator compensateEndpointConfigurator)
```

#### Parameters

`activityType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`compensateEndpointConfigurator` [IReceiveEndpointConfigurator](../masstransit/ireceiveendpointconfigurator)<br/>
The configurator for the compensate activity endpoint

### **ConfigureFuture(Type, IReceiveEndpointConfigurator)**

Configure a future on the receive endpoint

```csharp
void ConfigureFuture(Type futureType, IReceiveEndpointConfigurator configurator)
```

#### Parameters

`futureType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The saga type

`configurator` [IReceiveEndpointConfigurator](../masstransit/ireceiveendpointconfigurator)<br/>

### **ConfigureFuture\<T\>(IReceiveEndpointConfigurator)**

Configure a future on the receive endpoint, with an optional configuration action

```csharp
void ConfigureFuture<T>(IReceiveEndpointConfigurator configurator)
```

#### Type Parameters

`T`<br/>
The saga type

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../masstransit/ireceiveendpointconfigurator)<br/>
