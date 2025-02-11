---

title: HostedServiceConfigurationExtensions

---

# HostedServiceConfigurationExtensions

Namespace: MassTransit

These are the updated extensions compatible with the container registration code. They should be used, for real.

```csharp
public static class HostedServiceConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [HostedServiceConfigurationExtensions](../masstransit/hostedserviceconfigurationextensions)

## Methods

### **AddMassTransitHostedService(IServiceCollection)**

#### Caution

Remove, the hosted service is automatically registered. Visit https://masstransit.io/obsolete for details.

---

Adds the MassTransit , which includes a bus and endpoint health check.

```csharp
public static IServiceCollection AddMassTransitHostedService(IServiceCollection services)
```

#### Parameters

`services` IServiceCollection<br/>

#### Returns

IServiceCollection<br/>

### **AddMassTransitHostedService(IServiceCollection, Boolean)**

#### Caution

Remove, the hosted service is automatically registered. Visit https://masstransit.io/obsolete for details.

---

Adds the MassTransit , which includes a bus and endpoint health check.

```csharp
public static IServiceCollection AddMassTransitHostedService(IServiceCollection services, bool waitUntilStarted)
```

#### Parameters

`services` IServiceCollection<br/>

`waitUntilStarted` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
Await until bus fully started. (It will block application until bus becomes ready)

#### Returns

IServiceCollection<br/>

### **AddMassTransitHostedService(IServiceCollection, Boolean, Nullable\<TimeSpan\>, Nullable\<TimeSpan\>)**

#### Caution

Remove, the hosted service is automatically registered. Visit https://masstransit.io/obsolete for details.

---

Adds the MassTransit , which includes a bus and endpoint health check.
 Use it together with UseHealthCheck to get more detailed diagnostics.

```csharp
public static IServiceCollection AddMassTransitHostedService(IServiceCollection services, bool waitUntilStarted, Nullable<TimeSpan> startTimeout, Nullable<TimeSpan> stopTimeout)
```

#### Parameters

`services` IServiceCollection<br/>

`waitUntilStarted` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
Await until bus fully started. (It will block application until bus becomes ready)

`startTimeout` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
The timeout for starting the bus. The bus start process will not respond to the hosted service's cancellation token.
 In other words, if host shutdown is triggered during bus startup, the startup will still complete (subject to the specified timeout).

`stopTimeout` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
The timeout for stopping the bus. The bus stop process will not respond to the hosted service's cancellation token.
 In other words, bus shutdown will complete gracefully (subject to the specified timeout) even if instructed by ASP.NET Core
 to no longer be graceful.

#### Returns

IServiceCollection<br/>
