---

title: JobServiceConfigurationExtensions

---

# JobServiceConfigurationExtensions

Namespace: MassTransit

```csharp
public static class JobServiceConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobServiceConfigurationExtensions](../masstransit/jobserviceconfigurationextensions)

## Methods

### **ConfigureJobServiceEndpoints\<T\>(IServiceInstanceConfigurator\<T\>, IRegistrationContext, Action\<IJobServiceConfigurator\>)**

#### Caution

Use AddJobSagaStateMachines instead. Visit https://masstransit.io/obsolete for details.

---

Configures support for job consumers on the service instance, which supports executing long-running jobs without blocking the consumer pipeline.
 Job consumers use multiple state machines to track jobs, each of which runs on its own dedicated receive endpoint. Multiple service
 instances will use the competing consumer pattern, so a shared saga repository should be configured.

```csharp
public static IServiceInstanceConfigurator<T> ConfigureJobServiceEndpoints<T>(IServiceInstanceConfigurator<T> configurator, IRegistrationContext context, Action<IJobServiceConfigurator> configure)
```

#### Type Parameters

`T`<br/>
The transport receive endpoint configurator type

#### Parameters

`configurator` [IServiceInstanceConfigurator\<T\>](../../masstransit-abstractions/masstransit/iserviceinstanceconfigurator-1)<br/>
The service instance

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

`configure` [Action\<IJobServiceConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IServiceInstanceConfigurator\<T\>](../../masstransit-abstractions/masstransit/iserviceinstanceconfigurator-1)<br/>

### **ConfigureJobServiceEndpoints\<T\>(IServiceInstanceConfigurator\<T\>, Action\<IJobServiceConfigurator\>)**

Configures support for job consumers on the service instance, which supports executing long-running jobs without blocking the consumer pipeline.
 Job consumers use multiple state machines to track jobs, each of which runs on its own dedicated receive endpoint. Multiple service
 instances will use the competing consumer pattern, so a shared saga repository should be configured.

```csharp
public static IServiceInstanceConfigurator<T> ConfigureJobServiceEndpoints<T>(IServiceInstanceConfigurator<T> configurator, Action<IJobServiceConfigurator> configure)
```

#### Type Parameters

`T`<br/>
The transport receive endpoint configurator type

#### Parameters

`configurator` [IServiceInstanceConfigurator\<T\>](../../masstransit-abstractions/masstransit/iserviceinstanceconfigurator-1)<br/>
The service instance

`configure` [Action\<IJobServiceConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IServiceInstanceConfigurator\<T\>](../../masstransit-abstractions/masstransit/iserviceinstanceconfigurator-1)<br/>

### **ConfigureJobServiceEndpoints\<T\>(IServiceInstanceConfigurator\<T\>, JobServiceOptions, IRegistrationContext, Action\<IJobServiceConfigurator\>)**

#### Caution

Use AddJobSagaStateMachines instead. Visit https://masstransit.io/obsolete for details.

---

Configures support for job consumers on the service instance, which supports executing long-running jobs without blocking the consumer pipeline.
 Job consumers use multiple state machines to track jobs, each of which runs on its own dedicated receive endpoint. Multiple service
 instances will use the competing consumer pattern, so a shared saga repository should be configured.

```csharp
public static IServiceInstanceConfigurator<T> ConfigureJobServiceEndpoints<T>(IServiceInstanceConfigurator<T> configurator, JobServiceOptions options, IRegistrationContext context, Action<IJobServiceConfigurator> configure)
```

#### Type Parameters

`T`<br/>
The transport receive endpoint configurator type

#### Parameters

`configurator` [IServiceInstanceConfigurator\<T\>](../../masstransit-abstractions/masstransit/iserviceinstanceconfigurator-1)<br/>
The service instance

`options` [JobServiceOptions](../masstransit/jobserviceoptions)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>

`configure` [Action\<IJobServiceConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IServiceInstanceConfigurator\<T\>](../../masstransit-abstractions/masstransit/iserviceinstanceconfigurator-1)<br/>

### **ConfigureJobServiceEndpoints\<T\>(IServiceInstanceConfigurator\<T\>, JobServiceOptions, Action\<IJobServiceConfigurator\>)**

#### Caution

Use AddJobSagaStateMachines instead. Visit https://masstransit.io/obsolete for details.

---

Configures support for job consumers on the service instance, which supports executing long-running jobs without blocking the consumer pipeline.
 Job consumers use multiple state machines to track jobs, each of which runs on its own dedicated receive endpoint. Multiple service
 instances will use the competing consumer pattern, so a shared saga repository should be configured.

```csharp
public static IServiceInstanceConfigurator<T> ConfigureJobServiceEndpoints<T>(IServiceInstanceConfigurator<T> configurator, JobServiceOptions options, Action<IJobServiceConfigurator> configure)
```

#### Type Parameters

`T`<br/>
The transport receive endpoint configurator type

#### Parameters

`configurator` [IServiceInstanceConfigurator\<T\>](../../masstransit-abstractions/masstransit/iserviceinstanceconfigurator-1)<br/>
The service instance

`options` [JobServiceOptions](../masstransit/jobserviceoptions)<br/>

`configure` [Action\<IJobServiceConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IServiceInstanceConfigurator\<T\>](../../masstransit-abstractions/masstransit/iserviceinstanceconfigurator-1)<br/>

### **ConfigureJobService\<T\>(IServiceInstanceConfigurator\<T\>, Action\<IJobServiceConfigurator\>)**

#### Caution

Job Consumers no longer require a service instance. Visit https://masstransit.io/obsolete for details.

---

Configures support for job consumers on the service instance, which supports executing long-running jobs without blocking the consumer pipeline.
 Job consumers use multiple state machines to track jobs, each of which runs on its own dedicated receive endpoint. Multiple service
 instances will use the competing consumer pattern, so a shared saga repository should be configured.
 This method does not configure the state machine endpoints required to use the job service, and should only be used for services where another
 service has the job service endpoints configured.

```csharp
public static IServiceInstanceConfigurator<T> ConfigureJobService<T>(IServiceInstanceConfigurator<T> configurator, Action<IJobServiceConfigurator> configure)
```

#### Type Parameters

`T`<br/>
The transport receive endpoint configurator type

#### Parameters

`configurator` [IServiceInstanceConfigurator\<T\>](../../masstransit-abstractions/masstransit/iserviceinstanceconfigurator-1)<br/>
The service instance

`configure` [Action\<IJobServiceConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IServiceInstanceConfigurator\<T\>](../../masstransit-abstractions/masstransit/iserviceinstanceconfigurator-1)<br/>

### **ConfigureJobService\<T\>(IServiceInstanceConfigurator\<T\>, JobServiceOptions, Action\<IJobServiceConfigurator\>)**

#### Caution

Job Consumers no longer require a service instance. Visit https://masstransit.io/obsolete for details.

---

Configures support for job consumers on the service instance, which supports executing long-running jobs without blocking the consumer pipeline.
 Job consumers use multiple state machines to track jobs, each of which runs on its own dedicated receive endpoint. Multiple service
 instances will use the competing consumer pattern, so a shared saga repository should be configured.
 This method does not configure the state machine endpoints required to use the job service, and should only be used for services where another
 service has the job service endpoints configured.

```csharp
public static IServiceInstanceConfigurator<T> ConfigureJobService<T>(IServiceInstanceConfigurator<T> configurator, JobServiceOptions options, Action<IJobServiceConfigurator> configure)
```

#### Type Parameters

`T`<br/>
The transport receive endpoint configurator type

#### Parameters

`configurator` [IServiceInstanceConfigurator\<T\>](../../masstransit-abstractions/masstransit/iserviceinstanceconfigurator-1)<br/>
The service instance

`options` [JobServiceOptions](../masstransit/jobserviceoptions)<br/>

`configure` [Action\<IJobServiceConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IServiceInstanceConfigurator\<T\>](../../masstransit-abstractions/masstransit/iserviceinstanceconfigurator-1)<br/>
