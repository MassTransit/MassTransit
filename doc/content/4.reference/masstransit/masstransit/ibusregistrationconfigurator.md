---

title: IBusRegistrationConfigurator

---

# IBusRegistrationConfigurator

Namespace: MassTransit

Configures the container registration, and supports creation of a bus or a mediator.

```csharp
public interface IBusRegistrationConfigurator : IRegistrationConfigurator, IServiceCollection, IList<ServiceDescriptor>, ICollection<ServiceDescriptor>, IEnumerable<ServiceDescriptor>, IEnumerable
```

Implements [IRegistrationConfigurator](../masstransit/iregistrationconfigurator), IServiceCollection, [IList\<ServiceDescriptor\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1), [ICollection\<ServiceDescriptor\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.icollection-1), [IEnumerable\<ServiceDescriptor\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Properties

### **Registrar**

```csharp
public abstract IContainerRegistrar Registrar { get; }
```

#### Property Value

[IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

## Methods

### **AddBus(Func\<IBusRegistrationContext, IBusControl\>)**

#### Caution

Use 'Using[TransportName]' instead. Visit https://masstransit.io/obsolete for details.

---

This method is being deprecated. Use the transport-specific UsingRabbitMq, UsingActiveMq, etc. methods instead.

```csharp
void AddBus(Func<IBusRegistrationContext, IBusControl> busFactory)
```

#### Parameters

`busFactory` [Func\<IBusRegistrationContext, IBusControl\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **SetBusFactory\<T\>(T)**

Sets the bus factory. This is used by the transport extension methods (such as UsingRabbitMq, UsingActiveMq, etc.) to
 specify the bus factory. The extension method approach is preferred (since v7) over the AddBus method.

```csharp
void SetBusFactory<T>(T busFactory)
```

#### Type Parameters

`T`<br/>

#### Parameters

`busFactory` T<br/>

### **AddRider(Action\<IRiderRegistrationConfigurator\>)**

Add bus rider

```csharp
void AddRider(Action<IRiderRegistrationConfigurator> configure)
```

#### Parameters

`configure` [Action\<IRiderRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **AddConfigureEndpointsCallback(ConfigureEndpointsCallback)**

Adds a method that is called for each receive endpoint when it is configured, allowing additional
 configuration to be specified. Multiple callbacks may be configured.

```csharp
void AddConfigureEndpointsCallback(ConfigureEndpointsCallback callback)
```

#### Parameters

`callback` [ConfigureEndpointsCallback](../masstransit/configureendpointscallback)<br/>
Callback invoked for each receive endpoint

### **AddConfigureEndpointsCallback(ConfigureEndpointsProviderCallback)**

Adds a method that is called for each receive endpoint when it is configured, allowing additional
 configuration to be specified. Multiple callbacks may be configured.

```csharp
void AddConfigureEndpointsCallback(ConfigureEndpointsProviderCallback callback)
```

#### Parameters

`callback` [ConfigureEndpointsProviderCallback](../masstransit/configureendpointsprovidercallback)<br/>
Callback invoked for each receive endpoint

### **SetRequestClientFactory(Func\<IBus, RequestTimeout, IClientFactory\>)**

Override the default request client factory to enable advanced scenarios. This is typically not called by end-users.

```csharp
void SetRequestClientFactory(Func<IBus, RequestTimeout, IClientFactory> clientFactory)
```

#### Parameters

`clientFactory` [Func\<IBus, RequestTimeout, IClientFactory\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>
