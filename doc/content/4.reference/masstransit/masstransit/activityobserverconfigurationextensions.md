---

title: ActivityObserverConfigurationExtensions

---

# ActivityObserverConfigurationExtensions

Namespace: MassTransit

```csharp
public static class ActivityObserverConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ActivityObserverConfigurationExtensions](../masstransit/activityobserverconfigurationextensions)

## Methods

### **ConnectActivityObserver(IBusFactoryConfigurator, IActivityObserver)**

Connect an activity observer that will be connected to all activity execute/compensate endpoints

```csharp
public static ConnectHandle ConnectActivityObserver(IBusFactoryConfigurator configurator, IActivityObserver observer)
```

#### Parameters

`configurator` [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator)<br/>

`observer` [IActivityObserver](../../masstransit-abstractions/masstransit/iactivityobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectActivityObserver(IReceiveEndpointConfigurator, IActivityObserver)**

Connect an activity observer that will be connected to all activity execute/compensate endpoints

```csharp
public static ConnectHandle ConnectActivityObserver(IReceiveEndpointConfigurator configurator, IActivityObserver observer)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`observer` [IActivityObserver](../../masstransit-abstractions/masstransit/iactivityobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
