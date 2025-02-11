---

title: HostConnectReceiveEndpointExtensions

---

# HostConnectReceiveEndpointExtensions

Namespace: MassTransit

```csharp
public static class HostConnectReceiveEndpointExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [HostConnectReceiveEndpointExtensions](../masstransit/hostconnectreceiveendpointextensions)

## Methods

### **ConnectResponseEndpoint(IReceiveConnector, IEndpointNameFormatter, Action\<IReceiveEndpointConfigurator\>)**

Connect a response endpoint for the host

```csharp
public static HostReceiveEndpointHandle ConnectResponseEndpoint(IReceiveConnector connector, IEndpointNameFormatter endpointNameFormatter, Action<IReceiveEndpointConfigurator> configureEndpoint)
```

#### Parameters

`connector` [IReceiveConnector](../masstransit/ireceiveconnector)<br/>
The host to connect

`endpointNameFormatter` [IEndpointNameFormatter](../masstransit/iendpointnameformatter)<br/>

`configureEndpoint` [Action\<IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The configuration callback

#### Returns

[HostReceiveEndpointHandle](../masstransit/hostreceiveendpointhandle)<br/>

### **ConnectReceiveEndpoint(IReceiveConnector, Action\<IReceiveEndpointConfigurator\>)**

Connect an endpoint for the host

```csharp
public static HostReceiveEndpointHandle ConnectReceiveEndpoint(IReceiveConnector connector, Action<IReceiveEndpointConfigurator> configureEndpoint)
```

#### Parameters

`connector` [IReceiveConnector](../masstransit/ireceiveconnector)<br/>
The host to connect

`configureEndpoint` [Action\<IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The configuration callback

#### Returns

[HostReceiveEndpointHandle](../masstransit/hostreceiveendpointhandle)<br/>
