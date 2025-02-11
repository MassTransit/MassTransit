---

title: ConfigureBusHealthCheckServiceOptions

---

# ConfigureBusHealthCheckServiceOptions

Namespace: MassTransit.Monitoring

```csharp
public class ConfigureBusHealthCheckServiceOptions : IConfigureOptions<HealthCheckServiceOptions>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConfigureBusHealthCheckServiceOptions](../masstransit-monitoring/configurebushealthcheckserviceoptions)<br/>
Implements IConfigureOptions\<HealthCheckServiceOptions\>

## Constructors

### **ConfigureBusHealthCheckServiceOptions(IEnumerable\<IBusInstance\>, IServiceProvider)**

```csharp
public ConfigureBusHealthCheckServiceOptions(IEnumerable<IBusInstance> busInstances, IServiceProvider provider)
```

#### Parameters

`busInstances` [IEnumerable\<IBusInstance\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`provider` IServiceProvider<br/>

## Methods

### **Configure(HealthCheckServiceOptions)**

```csharp
public void Configure(HealthCheckServiceOptions options)
```

#### Parameters

`options` HealthCheckServiceOptions<br/>
