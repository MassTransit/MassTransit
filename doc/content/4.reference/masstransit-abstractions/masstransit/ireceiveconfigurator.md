---

title: IReceiveConfigurator

---

# IReceiveConfigurator

Namespace: MassTransit

```csharp
public interface IReceiveConfigurator : IEndpointConfigurationObserverConnector
```

Implements [IEndpointConfigurationObserverConnector](../masstransit/iendpointconfigurationobserverconnector)

## Methods

### **ReceiveEndpoint(IEndpointDefinition, IEndpointNameFormatter, Action\<IReceiveEndpointConfigurator\>)**

Adds a receive endpoint

```csharp
void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter, Action<IReceiveEndpointConfigurator> configureEndpoint)
```

#### Parameters

`definition` [IEndpointDefinition](../masstransit/iendpointdefinition)<br/>
An endpoint definition, which abstracts specific endpoint behaviors from the transport

`endpointNameFormatter` [IEndpointNameFormatter](../masstransit/iendpointnameformatter)<br/>

`configureEndpoint` [Action\<IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The configuration callback

### **ReceiveEndpoint(String, Action\<IReceiveEndpointConfigurator\>)**

Adds a receive endpoint

```csharp
void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The queue name for the receive endpoint

`configureEndpoint` [Action\<IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The configuration callback
