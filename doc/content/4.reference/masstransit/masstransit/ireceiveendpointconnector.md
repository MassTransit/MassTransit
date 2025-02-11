---

title: IReceiveEndpointConnector

---

# IReceiveEndpointConnector

Namespace: MassTransit

```csharp
public interface IReceiveEndpointConnector
```

## Methods

### **ConnectReceiveEndpoint(IEndpointDefinition, IEndpointNameFormatter, Action\<IBusRegistrationContext, IReceiveEndpointConfigurator\>)**

Connects a receive endpoint to the bus

```csharp
HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter, Action<IBusRegistrationContext, IReceiveEndpointConfigurator> configure)
```

#### Parameters

`definition` [IEndpointDefinition](../../masstransit-abstractions/masstransit/iendpointdefinition)<br/>
An endpoint definition, which abstracts specific endpoint behaviors from the transport

`endpointNameFormatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

`configure` [Action\<IBusRegistrationContext, IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>
The configuration callback

#### Returns

[HostReceiveEndpointHandle](../../masstransit-abstractions/masstransit/hostreceiveendpointhandle)<br/>

### **ConnectReceiveEndpoint(String, Action\<IBusRegistrationContext, IReceiveEndpointConfigurator\>)**

Connects a receive endpoint to the bus

```csharp
HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IBusRegistrationContext, IReceiveEndpointConfigurator> configure)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The queue name for the receive endpoint

`configure` [Action\<IBusRegistrationContext, IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>
The configuration callback

#### Returns

[HostReceiveEndpointHandle](../../masstransit-abstractions/masstransit/hostreceiveendpointhandle)<br/>
