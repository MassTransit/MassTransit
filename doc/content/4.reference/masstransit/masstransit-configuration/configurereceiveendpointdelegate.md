---

title: ConfigureReceiveEndpointDelegate

---

# ConfigureReceiveEndpointDelegate

Namespace: MassTransit.Configuration

```csharp
public class ConfigureReceiveEndpointDelegate : IConfigureReceiveEndpoint
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConfigureReceiveEndpointDelegate](../masstransit-configuration/configurereceiveendpointdelegate)<br/>
Implements [IConfigureReceiveEndpoint](../../masstransit-abstractions/masstransit/iconfigurereceiveendpoint)

## Constructors

### **ConfigureReceiveEndpointDelegate(ConfigureEndpointsCallback)**

```csharp
public ConfigureReceiveEndpointDelegate(ConfigureEndpointsCallback callback)
```

#### Parameters

`callback` [ConfigureEndpointsCallback](../masstransit/configureendpointscallback)<br/>

## Methods

### **Configure(String, IReceiveEndpointConfigurator)**

```csharp
public void Configure(string name, IReceiveEndpointConfigurator configurator)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>
