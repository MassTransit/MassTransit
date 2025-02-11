---

title: IReceiveConnector<TEndpointConfigurator>

---

# IReceiveConnector\<TEndpointConfigurator\>

Namespace: MassTransit

```csharp
public interface IReceiveConnector<TEndpointConfigurator> : IReceiveConnector, IEndpointConfigurationObserverConnector
```

#### Type Parameters

`TEndpointConfigurator`<br/>

Implements [IReceiveConnector](../masstransit/ireceiveconnector), [IEndpointConfigurationObserverConnector](../masstransit/iendpointconfigurationobserverconnector)

## Methods

### **ConnectReceiveEndpoint(IEndpointDefinition, IEndpointNameFormatter, Action\<TEndpointConfigurator\>)**

Adds a receive endpoint

```csharp
HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter, Action<TEndpointConfigurator> configureEndpoint)
```

#### Parameters

`definition` [IEndpointDefinition](../masstransit/iendpointdefinition)<br/>
An endpoint definition, which abstracts specific endpoint behaviors from the transport

`endpointNameFormatter` [IEndpointNameFormatter](../masstransit/iendpointnameformatter)<br/>

`configureEndpoint` [Action\<TEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The configuration callback

#### Returns

[HostReceiveEndpointHandle](../masstransit/hostreceiveendpointhandle)<br/>

### **ConnectReceiveEndpoint(String, Action\<TEndpointConfigurator\>)**

Adds a receive endpoint

```csharp
HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<TEndpointConfigurator> configureEndpoint)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The queue name for the receive endpoint

`configureEndpoint` [Action\<TEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The configuration callback

#### Returns

[HostReceiveEndpointHandle](../masstransit/hostreceiveendpointhandle)<br/>
