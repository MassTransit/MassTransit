---

title: MassTransitHealthCheckOptionsExtensions

---

# MassTransitHealthCheckOptionsExtensions

Namespace: MassTransit

```csharp
public static class MassTransitHealthCheckOptionsExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MassTransitHealthCheckOptionsExtensions](../masstransit/masstransithealthcheckoptionsextensions)

## Methods

### **ConfigureHealthCheckOptions(IBusRegistrationConfigurator, Action\<IHealthCheckOptionsConfigurator\>)**

Configure the health check options for this bus

```csharp
public static IBusRegistrationConfigurator ConfigureHealthCheckOptions(IBusRegistrationConfigurator configurator, Action<IHealthCheckOptionsConfigurator> callback)
```

#### Parameters

`configurator` [IBusRegistrationConfigurator](../masstransit/ibusregistrationconfigurator)<br/>

`callback` [Action\<IHealthCheckOptionsConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IBusRegistrationConfigurator](../masstransit/ibusregistrationconfigurator)<br/>

### **ConfigureHealthCheckOptions\<T\>(IBusRegistrationConfigurator\<T\>, Action\<IHealthCheckOptionsConfigurator\>)**

Configure the health check options for this bus

```csharp
public static IBusRegistrationConfigurator ConfigureHealthCheckOptions<T>(IBusRegistrationConfigurator<T> configurator, Action<IHealthCheckOptionsConfigurator> callback)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IBusRegistrationConfigurator\<T\>](../masstransit/ibusregistrationconfigurator-1)<br/>

`callback` [Action\<IHealthCheckOptionsConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IBusRegistrationConfigurator](../masstransit/ibusregistrationconfigurator)<br/>
