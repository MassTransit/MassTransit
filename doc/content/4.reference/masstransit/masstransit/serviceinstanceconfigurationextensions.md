---

title: ServiceInstanceConfigurationExtensions

---

# ServiceInstanceConfigurationExtensions

Namespace: MassTransit

```csharp
public static class ServiceInstanceConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ServiceInstanceConfigurationExtensions](../masstransit/serviceinstanceconfigurationextensions)

## Methods

### **ServiceInstance\<TEndpointConfigurator\>(IBusFactoryConfigurator\<TEndpointConfigurator\>, Action\<IServiceInstanceConfigurator\<TEndpointConfigurator\>\>)**

Configure a service instance for use with the job service

```csharp
public static void ServiceInstance<TEndpointConfigurator>(IBusFactoryConfigurator<TEndpointConfigurator> configurator, Action<IServiceInstanceConfigurator<TEndpointConfigurator>> configure)
```

#### Type Parameters

`TEndpointConfigurator`<br/>

#### Parameters

`configurator` [IBusFactoryConfigurator\<TEndpointConfigurator\>](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator-1)<br/>

`configure` [Action\<IServiceInstanceConfigurator\<TEndpointConfigurator\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ServiceInstance\<TEndpointConfigurator\>(IBusFactoryConfigurator\<TEndpointConfigurator\>, ServiceInstanceOptions, Action\<IServiceInstanceConfigurator\<TEndpointConfigurator\>\>)**

Configure a service instance for use with the job service

```csharp
public static void ServiceInstance<TEndpointConfigurator>(IBusFactoryConfigurator<TEndpointConfigurator> configurator, ServiceInstanceOptions options, Action<IServiceInstanceConfigurator<TEndpointConfigurator>> configure)
```

#### Type Parameters

`TEndpointConfigurator`<br/>

#### Parameters

`configurator` [IBusFactoryConfigurator\<TEndpointConfigurator\>](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator-1)<br/>

`options` [ServiceInstanceOptions](../masstransit/serviceinstanceoptions)<br/>

`configure` [Action\<IServiceInstanceConfigurator\<TEndpointConfigurator\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
