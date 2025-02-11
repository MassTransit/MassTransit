---

title: BusRegistrationContext

---

# BusRegistrationContext

Namespace: MassTransit.Configuration

```csharp
public class BusRegistrationContext : RegistrationContext, IRegistrationContext, IServiceProvider, ISetScopedConsumeContext, IBusRegistrationContext
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RegistrationContext](../masstransit-configuration/registrationcontext) → [BusRegistrationContext](../masstransit-configuration/busregistrationcontext)<br/>
Implements [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext), IServiceProvider, [ISetScopedConsumeContext](../masstransit/isetscopedconsumecontext), [IBusRegistrationContext](../masstransit/ibusregistrationcontext)

## Properties

### **EndpointNameFormatter**

```csharp
public IEndpointNameFormatter EndpointNameFormatter { get; }
```

#### Property Value

[IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

## Constructors

### **BusRegistrationContext(IServiceProvider, IContainerSelector, ISetScopedConsumeContext)**

```csharp
public BusRegistrationContext(IServiceProvider provider, IContainerSelector selector, ISetScopedConsumeContext setScopedConsumeContext)
```

#### Parameters

`provider` IServiceProvider<br/>

`selector` [IContainerSelector](../masstransit-configuration/icontainerselector)<br/>

`setScopedConsumeContext` [ISetScopedConsumeContext](../masstransit/isetscopedconsumecontext)<br/>

## Methods

### **ConfigureEndpoints\<T\>(IReceiveConfigurator\<T\>, IEndpointNameFormatter)**

```csharp
public void ConfigureEndpoints<T>(IReceiveConfigurator<T> configurator, IEndpointNameFormatter endpointNameFormatter)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IReceiveConfigurator\<T\>](../../masstransit-abstractions/masstransit/ireceiveconfigurator-1)<br/>

`endpointNameFormatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

### **ConfigureEndpoints\<T\>(IReceiveConfigurator\<T\>, IEndpointNameFormatter, Action\<IRegistrationFilterConfigurator\>)**

```csharp
public void ConfigureEndpoints<T>(IReceiveConfigurator<T> configurator, IEndpointNameFormatter endpointNameFormatter, Action<IRegistrationFilterConfigurator> configureFilter)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IReceiveConfigurator\<T\>](../../masstransit-abstractions/masstransit/ireceiveconfigurator-1)<br/>

`endpointNameFormatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

`configureFilter` [Action\<IRegistrationFilterConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **GetConfigureReceiveEndpoints()**

```csharp
public IConfigureReceiveEndpoint GetConfigureReceiveEndpoints()
```

#### Returns

[IConfigureReceiveEndpoint](../../masstransit-abstractions/masstransit/iconfigurereceiveendpoint)<br/>
