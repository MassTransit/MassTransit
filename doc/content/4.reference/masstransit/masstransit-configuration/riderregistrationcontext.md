---

title: RiderRegistrationContext

---

# RiderRegistrationContext

Namespace: MassTransit.Configuration

```csharp
public class RiderRegistrationContext : ISetScopedConsumeContext, IRiderRegistrationContext, IRegistrationContext, IServiceProvider
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RiderRegistrationContext](../masstransit-configuration/riderregistrationcontext)<br/>
Implements [ISetScopedConsumeContext](../masstransit/isetscopedconsumecontext), [IRiderRegistrationContext](../masstransit/iriderregistrationcontext), [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext), IServiceProvider

## Constructors

### **RiderRegistrationContext(RegistrationContext, IContainerSelector)**

```csharp
public RiderRegistrationContext(RegistrationContext registration, IContainerSelector selector)
```

#### Parameters

`registration` [RegistrationContext](../masstransit-configuration/registrationcontext)<br/>

`selector` [IContainerSelector](../masstransit-configuration/icontainerselector)<br/>

## Methods

### **GetRegistrations\<T\>()**

```csharp
public IEnumerable<T> GetRegistrations<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **GetService(Type)**

```csharp
public object GetService(Type serviceType)
```

#### Parameters

`serviceType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

### **ConfigureConsumer(Type, IReceiveEndpointConfigurator)**

```csharp
public void ConfigureConsumer(Type consumerType, IReceiveEndpointConfigurator configurator)
```

#### Parameters

`consumerType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

### **ConfigureConsumer\<T\>(IReceiveEndpointConfigurator, Action\<IConsumerConfigurator\<T\>\>)**

```csharp
public void ConfigureConsumer<T>(IReceiveEndpointConfigurator configurator, Action<IConsumerConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`configure` [Action\<IConsumerConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ConfigureConsumers(IReceiveEndpointConfigurator)**

```csharp
public void ConfigureConsumers(IReceiveEndpointConfigurator configurator)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

### **ConfigureSaga(Type, IReceiveEndpointConfigurator)**

```csharp
public void ConfigureSaga(Type sagaType, IReceiveEndpointConfigurator configurator)
```

#### Parameters

`sagaType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

### **ConfigureSaga\<T\>(IReceiveEndpointConfigurator, Action\<ISagaConfigurator\<T\>\>)**

```csharp
public void ConfigureSaga<T>(IReceiveEndpointConfigurator configurator, Action<ISagaConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`configure` [Action\<ISagaConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ConfigureSagas(IReceiveEndpointConfigurator)**

```csharp
public void ConfigureSagas(IReceiveEndpointConfigurator configurator)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

### **ConfigureExecuteActivity(Type, IReceiveEndpointConfigurator)**

```csharp
public void ConfigureExecuteActivity(Type activityType, IReceiveEndpointConfigurator configurator)
```

#### Parameters

`activityType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

### **ConfigureActivity(Type, IReceiveEndpointConfigurator, IReceiveEndpointConfigurator)**

```csharp
public void ConfigureActivity(Type activityType, IReceiveEndpointConfigurator executeEndpointConfigurator, IReceiveEndpointConfigurator compensateEndpointConfigurator)
```

#### Parameters

`activityType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`executeEndpointConfigurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`compensateEndpointConfigurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

### **ConfigureActivityExecute(Type, IReceiveEndpointConfigurator, Uri)**

```csharp
public void ConfigureActivityExecute(Type activityType, IReceiveEndpointConfigurator executeEndpointConfigurator, Uri compensateAddress)
```

#### Parameters

`activityType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`executeEndpointConfigurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`compensateAddress` Uri<br/>

### **ConfigureActivityCompensate(Type, IReceiveEndpointConfigurator)**

```csharp
public void ConfigureActivityCompensate(Type activityType, IReceiveEndpointConfigurator compensateEndpointConfigurator)
```

#### Parameters

`activityType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`compensateEndpointConfigurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

### **ConfigureFuture(Type, IReceiveEndpointConfigurator)**

```csharp
public void ConfigureFuture(Type futureType, IReceiveEndpointConfigurator configurator)
```

#### Parameters

`futureType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

### **ConfigureFuture\<T\>(IReceiveEndpointConfigurator)**

```csharp
public void ConfigureFuture<T>(IReceiveEndpointConfigurator configurator)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

### **PushContext(IServiceScope, ConsumeContext)**

```csharp
public IDisposable PushContext(IServiceScope scope, ConsumeContext context)
```

#### Parameters

`scope` IServiceScope<br/>

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

#### Returns

[IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br/>
