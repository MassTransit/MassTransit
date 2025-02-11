---

title: IJobSagaRegistrationConfigurator

---

# IJobSagaRegistrationConfigurator

Namespace: MassTransit

```csharp
public interface IJobSagaRegistrationConfigurator
```

## Methods

### **Endpoints(Action\<IEndpointRegistrationConfigurator\>)**

Configure all three saga endpoints (using the same configuration)

```csharp
IJobSagaRegistrationConfigurator Endpoints(Action<IEndpointRegistrationConfigurator> configure)
```

#### Parameters

`configure` [Action\<IEndpointRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IJobSagaRegistrationConfigurator](../masstransit/ijobsagaregistrationconfigurator)<br/>

### **JobAttemptEndpoint(Action\<IEndpointRegistrationConfigurator\>)**

Configure the JobAttemptSaga endpoint

```csharp
IJobSagaRegistrationConfigurator JobAttemptEndpoint(Action<IEndpointRegistrationConfigurator> configure)
```

#### Parameters

`configure` [Action\<IEndpointRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IJobSagaRegistrationConfigurator](../masstransit/ijobsagaregistrationconfigurator)<br/>

### **JobEndpoint(Action\<IEndpointRegistrationConfigurator\>)**

Configure the JobSaga endpoint

```csharp
IJobSagaRegistrationConfigurator JobEndpoint(Action<IEndpointRegistrationConfigurator> configure)
```

#### Parameters

`configure` [Action\<IEndpointRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IJobSagaRegistrationConfigurator](../masstransit/ijobsagaregistrationconfigurator)<br/>

### **JobTypeEndpoint(Action\<IEndpointRegistrationConfigurator\>)**

Configure the JobTypeSaga endpoint

```csharp
IJobSagaRegistrationConfigurator JobTypeEndpoint(Action<IEndpointRegistrationConfigurator> configure)
```

#### Parameters

`configure` [Action\<IEndpointRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IJobSagaRegistrationConfigurator](../masstransit/ijobsagaregistrationconfigurator)<br/>

### **UseRepositoryRegistrationProvider(ISagaRepositoryRegistrationProvider)**

Internally used by the saga repositories to register as the saga repository for the job sagas

```csharp
IJobSagaRegistrationConfigurator UseRepositoryRegistrationProvider(ISagaRepositoryRegistrationProvider registrationProvider)
```

#### Parameters

`registrationProvider` [ISagaRepositoryRegistrationProvider](../masstransit-configuration/isagarepositoryregistrationprovider)<br/>

#### Returns

[IJobSagaRegistrationConfigurator](../masstransit/ijobsagaregistrationconfigurator)<br/>
