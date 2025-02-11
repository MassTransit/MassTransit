---

title: IReceiveEndpointConnector<TEndpointConfigurator>

---

# IReceiveEndpointConnector\<TEndpointConfigurator\>

Namespace: MassTransit

```csharp
public interface IReceiveEndpointConnector<TEndpointConfigurator> : IReceiveEndpointConnector
```

#### Type Parameters

`TEndpointConfigurator`<br/>

Implements [IReceiveEndpointConnector](../masstransit/ireceiveendpointconnector)

## Methods

### **ConnectReceiveEndpoint(IEndpointDefinition, IEndpointNameFormatter, Action\<IBusRegistrationContext, TEndpointConfigurator\>)**

Connects a receive endpoint to the bus

```csharp
HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter, Action<IBusRegistrationContext, TEndpointConfigurator> configure)
```

#### Parameters

`definition` [IEndpointDefinition](../../masstransit-abstractions/masstransit/iendpointdefinition)<br/>
An endpoint definition, which abstracts specific endpoint behaviors from the transport

`endpointNameFormatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

`configure` [Action\<IBusRegistrationContext, TEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>
The configuration callback

#### Returns

[HostReceiveEndpointHandle](../../masstransit-abstractions/masstransit/hostreceiveendpointhandle)<br/>

### **ConnectReceiveEndpoint(String, Action\<IBusRegistrationContext, TEndpointConfigurator\>)**

Connects a receive endpoint to the bus

```csharp
HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IBusRegistrationContext, TEndpointConfigurator> configure)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The queue name for the receive endpoint

`configure` [Action\<IBusRegistrationContext, TEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>
The configuration callback

#### Returns

[HostReceiveEndpointHandle](../../masstransit-abstractions/masstransit/hostreceiveendpointhandle)<br/>
