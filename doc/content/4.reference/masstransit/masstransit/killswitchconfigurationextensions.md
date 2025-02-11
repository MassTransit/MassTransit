---

title: KillSwitchConfigurationExtensions

---

# KillSwitchConfigurationExtensions

Namespace: MassTransit

```csharp
public static class KillSwitchConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [KillSwitchConfigurationExtensions](../masstransit/killswitchconfigurationextensions)

## Methods

### **UseKillSwitch(IBusFactoryConfigurator, Action\<KillSwitchOptions\>)**

A Kill Switch monitors a receive endpoint and automatically stops and restarts the endpoint in the presence of consumer faults. The options
 can be configured to adjust the trip threshold, restart timeout, and exceptions that are observed by the kill switch. When configured on the bus,
 a kill switch is installed on every receive endpoint.

```csharp
public static void UseKillSwitch(IBusFactoryConfigurator configurator, Action<KillSwitchOptions> configure)
```

#### Parameters

`configurator` [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator)<br/>
The bus factory configurator

`configure` [Action\<KillSwitchOptions\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configure the kill switch options

### **UseKillSwitch(IReceiveEndpointConfigurator, Action\<KillSwitchOptions\>)**

A Kill Switch monitors a receive endpoint and automatically stops and restarts the endpoint in the presence of consumer faults. The options
 can be configured to adjust the trip threshold, restart timeout, and exceptions that are observed by the kill switch. When configured on a
 receive endpoint, a kill switch is installed on that receive endpoint only.

```csharp
public static void UseKillSwitch(IReceiveEndpointConfigurator configurator, Action<KillSwitchOptions> configure)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>
The bus factory configurator

`configure` [Action\<KillSwitchOptions\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configure the kill switch options
