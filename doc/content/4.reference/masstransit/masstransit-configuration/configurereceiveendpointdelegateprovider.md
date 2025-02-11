---

title: ConfigureReceiveEndpointDelegateProvider

---

# ConfigureReceiveEndpointDelegateProvider

Namespace: MassTransit.Configuration

```csharp
public class ConfigureReceiveEndpointDelegateProvider : IConfigureReceiveEndpoint
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConfigureReceiveEndpointDelegateProvider](../masstransit-configuration/configurereceiveendpointdelegateprovider)<br/>
Implements [IConfigureReceiveEndpoint](../../masstransit-abstractions/masstransit/iconfigurereceiveendpoint)

## Constructors

### **ConfigureReceiveEndpointDelegateProvider(IRegistrationContext, ConfigureEndpointsProviderCallback)**

```csharp
public ConfigureReceiveEndpointDelegateProvider(IRegistrationContext context, ConfigureEndpointsProviderCallback callback)
```

#### Parameters

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

`callback` [ConfigureEndpointsProviderCallback](../masstransit/configureendpointsprovidercallback)<br/>

## Methods

### **Configure(String, IReceiveEndpointConfigurator)**

```csharp
public void Configure(string name, IReceiveEndpointConfigurator configurator)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>
