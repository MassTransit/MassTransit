---

title: JobServiceRegistrationExtensions

---

# JobServiceRegistrationExtensions

Namespace: MassTransit

```csharp
public static class JobServiceRegistrationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobServiceRegistrationExtensions](../masstransit/jobserviceregistrationextensions)

## Methods

### **SetJobConsumerOptions(IBusRegistrationConfigurator, Action\<JobConsumerOptions\>)**

Set the job consumer options (optional, not required to use job consumers)

```csharp
public static IJobServiceRegistrationConfigurator SetJobConsumerOptions(IBusRegistrationConfigurator configurator, Action<JobConsumerOptions> configure)
```

#### Parameters

`configurator` [IBusRegistrationConfigurator](../masstransit/ibusregistrationconfigurator)<br/>

`configure` [Action\<JobConsumerOptions\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configure the job consumer options using this callback

#### Returns

[IJobServiceRegistrationConfigurator](../masstransit/ijobserviceregistrationconfigurator)<br/>

### **AddJobSagaStateMachines(IBusRegistrationConfigurator, Action\<JobSagaOptions\>)**

Add registrations for the job service saga state machines

```csharp
public static IJobSagaRegistrationConfigurator AddJobSagaStateMachines(IBusRegistrationConfigurator configurator, Action<JobSagaOptions> configure)
```

#### Parameters

`configurator` [IBusRegistrationConfigurator](../masstransit/ibusregistrationconfigurator)<br/>

`configure` [Action\<JobSagaOptions\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configure the job saga options

#### Returns

[IJobSagaRegistrationConfigurator](../masstransit/ijobsagaregistrationconfigurator)<br/>

### **TryAddJobDistributionStrategy\<T\>(IServiceCollection)**

Register a custom job distribution strategy for the job saga state machines

```csharp
public static IServiceCollection TryAddJobDistributionStrategy<T>(IServiceCollection services)
```

#### Type Parameters

`T`<br/>

#### Parameters

`services` IServiceCollection<br/>

#### Returns

IServiceCollection<br/>
