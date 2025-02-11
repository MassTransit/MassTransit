---

title: JobSagaRegistrationConfigurator

---

# JobSagaRegistrationConfigurator

Namespace: MassTransit.DependencyInjection.Registration

```csharp
public class JobSagaRegistrationConfigurator : IJobSagaRegistrationConfigurator
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobSagaRegistrationConfigurator](../masstransit-dependencyinjection-registration/jobsagaregistrationconfigurator)<br/>
Implements [IJobSagaRegistrationConfigurator](../masstransit/ijobsagaregistrationconfigurator)

## Constructors

### **JobSagaRegistrationConfigurator(IBusRegistrationConfigurator, Action\<JobSagaOptions\>)**

```csharp
public JobSagaRegistrationConfigurator(IBusRegistrationConfigurator configurator, Action<JobSagaOptions> configure)
```

#### Parameters

`configurator` [IBusRegistrationConfigurator](../masstransit/ibusregistrationconfigurator)<br/>

`configure` [Action\<JobSagaOptions\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

## Methods

### **Endpoints(Action\<IEndpointRegistrationConfigurator\>)**

```csharp
public IJobSagaRegistrationConfigurator Endpoints(Action<IEndpointRegistrationConfigurator> configure)
```

#### Parameters

`configure` [Action\<IEndpointRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IJobSagaRegistrationConfigurator](../masstransit/ijobsagaregistrationconfigurator)<br/>

### **JobAttemptEndpoint(Action\<IEndpointRegistrationConfigurator\>)**

```csharp
public IJobSagaRegistrationConfigurator JobAttemptEndpoint(Action<IEndpointRegistrationConfigurator> configure)
```

#### Parameters

`configure` [Action\<IEndpointRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IJobSagaRegistrationConfigurator](../masstransit/ijobsagaregistrationconfigurator)<br/>

### **JobEndpoint(Action\<IEndpointRegistrationConfigurator\>)**

```csharp
public IJobSagaRegistrationConfigurator JobEndpoint(Action<IEndpointRegistrationConfigurator> configure)
```

#### Parameters

`configure` [Action\<IEndpointRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IJobSagaRegistrationConfigurator](../masstransit/ijobsagaregistrationconfigurator)<br/>

### **JobTypeEndpoint(Action\<IEndpointRegistrationConfigurator\>)**

```csharp
public IJobSagaRegistrationConfigurator JobTypeEndpoint(Action<IEndpointRegistrationConfigurator> configure)
```

#### Parameters

`configure` [Action\<IEndpointRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IJobSagaRegistrationConfigurator](../masstransit/ijobsagaregistrationconfigurator)<br/>

### **UseRepositoryRegistrationProvider(ISagaRepositoryRegistrationProvider)**

```csharp
public IJobSagaRegistrationConfigurator UseRepositoryRegistrationProvider(ISagaRepositoryRegistrationProvider registrationProvider)
```

#### Parameters

`registrationProvider` [ISagaRepositoryRegistrationProvider](../masstransit-configuration/isagarepositoryregistrationprovider)<br/>

#### Returns

[IJobSagaRegistrationConfigurator](../masstransit/ijobsagaregistrationconfigurator)<br/>
