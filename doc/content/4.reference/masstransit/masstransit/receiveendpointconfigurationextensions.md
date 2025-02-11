---

title: ReceiveEndpointConfigurationExtensions

---

# ReceiveEndpointConfigurationExtensions

Namespace: MassTransit

```csharp
public static class ReceiveEndpointConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReceiveEndpointConfigurationExtensions](../masstransit/receiveendpointconfigurationextensions)

## Methods

### **ReceiveEndpoint(IBusFactoryConfigurator, Action\<IReceiveEndpointConfigurator\>)**

Creates a temporary endpoint, with a dynamically generated name, that should be removed when the bus is stopped.

```csharp
public static void ReceiveEndpoint(IBusFactoryConfigurator configurator, Action<IReceiveEndpointConfigurator> configure)
```

#### Parameters

`configurator` [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator)<br/>

`configure` [Action\<IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ReceiveEndpoint(IBusFactoryConfigurator, IEndpointDefinition, Action\<IReceiveEndpointConfigurator\>)**

Creates a management endpoint which can be used by controllable filters on a bus instance

```csharp
public static void ReceiveEndpoint(IBusFactoryConfigurator configurator, IEndpointDefinition definition, Action<IReceiveEndpointConfigurator> configure)
```

#### Parameters

`configurator` [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator)<br/>

`definition` [IEndpointDefinition](../../masstransit-abstractions/masstransit/iendpointdefinition)<br/>

`configure` [Action\<IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
