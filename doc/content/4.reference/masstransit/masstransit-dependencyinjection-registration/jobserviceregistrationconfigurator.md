---

title: JobServiceRegistrationConfigurator

---

# JobServiceRegistrationConfigurator

Namespace: MassTransit.DependencyInjection.Registration

```csharp
public class JobServiceRegistrationConfigurator : IJobServiceRegistrationConfigurator
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobServiceRegistrationConfigurator](../masstransit-dependencyinjection-registration/jobserviceregistrationconfigurator)<br/>
Implements [IJobServiceRegistrationConfigurator](../masstransit/ijobserviceregistrationconfigurator)

## Constructors

### **JobServiceRegistrationConfigurator(IBusRegistrationConfigurator, IJobServiceRegistration)**

```csharp
public JobServiceRegistrationConfigurator(IBusRegistrationConfigurator configurator, IJobServiceRegistration registration)
```

#### Parameters

`configurator` [IBusRegistrationConfigurator](../masstransit/ibusregistrationconfigurator)<br/>

`registration` [IJobServiceRegistration](../masstransit-configuration/ijobserviceregistration)<br/>

## Methods

### **Options(Action\<JobConsumerOptions\>)**

```csharp
public IJobServiceRegistrationConfigurator Options(Action<JobConsumerOptions> configure)
```

#### Parameters

`configure` [Action\<JobConsumerOptions\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IJobServiceRegistrationConfigurator](../masstransit/ijobserviceregistrationconfigurator)<br/>

### **Endpoint(Action\<IEndpointRegistrationConfigurator\>)**

```csharp
public void Endpoint(Action<IEndpointRegistrationConfigurator> configure)
```

#### Parameters

`configure` [Action\<IEndpointRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
