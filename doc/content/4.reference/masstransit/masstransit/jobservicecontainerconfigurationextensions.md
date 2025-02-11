---

title: JobServiceContainerConfigurationExtensions

---

# JobServiceContainerConfigurationExtensions

Namespace: MassTransit

```csharp
public static class JobServiceContainerConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobServiceContainerConfigurationExtensions](../masstransit/jobservicecontainerconfigurationextensions)

## Methods

### **ConfigureSagaRepositories(IJobServiceConfigurator, IRegistrationContext)**

Configure the job server saga repositories to resolve from the container.

```csharp
public static IJobServiceConfigurator ConfigureSagaRepositories(IJobServiceConfigurator configurator, IRegistrationContext context)
```

#### Parameters

`configurator` [IJobServiceConfigurator](../masstransit/ijobserviceconfigurator)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
The bus registration context provided during configuration

#### Returns

[IJobServiceConfigurator](../masstransit/ijobserviceconfigurator)<br/>

### **ConfigureSagaRepositories(IJobServiceConfigurator, IServiceProvider)**

#### Caution

Use the IRegistrationContext overload instead. Visit https://masstransit.io/obsolete for details.

---

Configure the job server saga repositories to resolve from the container.

```csharp
public static IJobServiceConfigurator ConfigureSagaRepositories(IJobServiceConfigurator configurator, IServiceProvider provider)
```

#### Parameters

`configurator` [IJobServiceConfigurator](../masstransit/ijobserviceconfigurator)<br/>

`provider` IServiceProvider<br/>
The bus registration context provided during configuration

#### Returns

[IJobServiceConfigurator](../masstransit/ijobserviceconfigurator)<br/>
