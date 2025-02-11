---

title: IReceiveConfigurator<TEndpointConfigurator>

---

# IReceiveConfigurator\<TEndpointConfigurator\>

Namespace: MassTransit

```csharp
public interface IReceiveConfigurator<TEndpointConfigurator> : IReceiveConfigurator, IEndpointConfigurationObserverConnector
```

#### Type Parameters

`TEndpointConfigurator`<br/>

Implements [IReceiveConfigurator](../masstransit/ireceiveconfigurator), [IEndpointConfigurationObserverConnector](../masstransit/iendpointconfigurationobserverconnector)

## Methods

### **ReceiveEndpoint(IEndpointDefinition, IEndpointNameFormatter, Action\<TEndpointConfigurator\>)**

Adds a receive endpoint

```csharp
void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter, Action<TEndpointConfigurator> configureEndpoint)
```

#### Parameters

`definition` [IEndpointDefinition](../masstransit/iendpointdefinition)<br/>
An endpoint definition, which abstracts specific endpoint behaviors from the transport

`endpointNameFormatter` [IEndpointNameFormatter](../masstransit/iendpointnameformatter)<br/>

`configureEndpoint` [Action\<TEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The configuration callback

### **ReceiveEndpoint(String, Action\<TEndpointConfigurator\>)**

Adds a receive endpoint

```csharp
void ReceiveEndpoint(string queueName, Action<TEndpointConfigurator> configureEndpoint)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The queue name for the receive endpoint

`configureEndpoint` [Action\<TEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The configuration callback
