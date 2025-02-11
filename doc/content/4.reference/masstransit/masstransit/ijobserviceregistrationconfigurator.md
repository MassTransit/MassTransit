---

title: IJobServiceRegistrationConfigurator

---

# IJobServiceRegistrationConfigurator

Namespace: MassTransit

```csharp
public interface IJobServiceRegistrationConfigurator
```

## Methods

### **Options(Action\<JobConsumerOptions\>)**

Configure the job service options

```csharp
IJobServiceRegistrationConfigurator Options(Action<JobConsumerOptions> configure)
```

#### Parameters

`configure` [Action\<JobConsumerOptions\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IJobServiceRegistrationConfigurator](../masstransit/ijobserviceregistrationconfigurator)<br/>

### **Endpoint(Action\<IEndpointRegistrationConfigurator\>)**

Configure the instance endpoint settings

```csharp
void Endpoint(Action<IEndpointRegistrationConfigurator> configure)
```

#### Parameters

`configure` [Action\<IEndpointRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
