---

title: IBusRegistrationContext

---

# IBusRegistrationContext

Namespace: MassTransit

```csharp
public interface IBusRegistrationContext : IRegistrationContext, IServiceProvider
```

Implements [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext), IServiceProvider

## Properties

### **EndpointNameFormatter**

```csharp
public abstract IEndpointNameFormatter EndpointNameFormatter { get; }
```

#### Property Value

[IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

## Methods

### **ConfigureEndpoints\<T\>(IReceiveConfigurator\<T\>, IEndpointNameFormatter)**

Configure the endpoints for all defined consumer, saga, and activity types using an optional
 endpoint name formatter. If no endpoint name formatter is specified and an 
 is registered in the container, it is resolved from the container. Otherwise, the [DefaultEndpointNameFormatter](../masstransit/defaultendpointnameformatter)
 is used.

```csharp
void ConfigureEndpoints<T>(IReceiveConfigurator<T> configurator, IEndpointNameFormatter endpointNameFormatter)
```

#### Type Parameters

`T`<br/>
The bus factory type (depends upon the transport)

#### Parameters

`configurator` [IReceiveConfigurator\<T\>](../../masstransit-abstractions/masstransit/ireceiveconfigurator-1)<br/>
The  for the bus being configured

`endpointNameFormatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>
Optional, the endpoint name formatter

### **ConfigureEndpoints\<T\>(IReceiveConfigurator\<T\>, IEndpointNameFormatter, Action\<IRegistrationFilterConfigurator\>)**

Configure the endpoints for all defined consumer, saga, and activity types using an optional
 endpoint name formatter. If no endpoint name formatter is specified and an 
 is registered in the container, it is resolved from the container. Otherwise, the [DefaultEndpointNameFormatter](../masstransit/defaultendpointnameformatter)
 is used.

```csharp
void ConfigureEndpoints<T>(IReceiveConfigurator<T> configurator, IEndpointNameFormatter endpointNameFormatter, Action<IRegistrationFilterConfigurator> configureFilter)
```

#### Type Parameters

`T`<br/>
The bus factory type (depends upon the transport)

#### Parameters

`configurator` [IReceiveConfigurator\<T\>](../../masstransit-abstractions/masstransit/ireceiveconfigurator-1)<br/>
The  for the bus being configured

`endpointNameFormatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>
Optional, the endpoint name formatter

`configureFilter` [Action\<IRegistrationFilterConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
A filter for the endpoints to be configured

### **GetConfigureReceiveEndpoints()**

Returns the registered  instances from the container. Used internally
 to apply configuration to every receive endpoint. This method should normally not be called.

```csharp
IConfigureReceiveEndpoint GetConfigureReceiveEndpoints()
```

#### Returns

[IConfigureReceiveEndpoint](../../masstransit-abstractions/masstransit/iconfigurereceiveendpoint)<br/>
