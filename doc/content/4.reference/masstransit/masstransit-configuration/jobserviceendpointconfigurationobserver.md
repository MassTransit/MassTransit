---

title: JobServiceEndpointConfigurationObserver

---

# JobServiceEndpointConfigurationObserver

Namespace: MassTransit.Configuration

```csharp
public class JobServiceEndpointConfigurationObserver : IEndpointConfigurationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobServiceEndpointConfigurationObserver](../masstransit-configuration/jobserviceendpointconfigurationobserver)<br/>
Implements [IEndpointConfigurationObserver](../../masstransit-abstractions/masstransit/iendpointconfigurationobserver)

## Constructors

### **JobServiceEndpointConfigurationObserver(JobServiceSettings, Action\<IReceiveEndpointConfigurator\>)**

```csharp
public JobServiceEndpointConfigurationObserver(JobServiceSettings settings, Action<IReceiveEndpointConfigurator> configureEndpoint)
```

#### Parameters

`settings` [JobServiceSettings](../masstransit-jobservice/jobservicesettings)<br/>

`configureEndpoint` [Action\<IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

## Methods

### **EndpointConfigured\<T\>(T)**

```csharp
public void EndpointConfigured<T>(T configurator)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` T<br/>
