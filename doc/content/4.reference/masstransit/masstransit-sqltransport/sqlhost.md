---

title: SqlHost

---

# SqlHost

Namespace: MassTransit.SqlTransport

```csharp
public class SqlHost : BaseHost, IHost, IReceiveConnector, IEndpointConfigurationObserverConnector, IConsumeMessageObserverConnector, IConsumeObserverConnector, IReceiveObserverConnector, IPublishObserverConnector, ISendObserverConnector, IReceiveEndpointObserverConnector, IProbeSite, ISqlHost, IHost<ISqlReceiveEndpointConfigurator>, IReceiveConnector<ISqlReceiveEndpointConfigurator>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BaseHost](../masstransit-transports/basehost) → [SqlHost](../masstransit-sqltransport/sqlhost)<br/>
Implements [IHost](../masstransit-transports/ihost), [IReceiveConnector](../../masstransit-abstractions/masstransit/ireceiveconnector), [IEndpointConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iendpointconfigurationobserverconnector), [IConsumeMessageObserverConnector](../../masstransit-abstractions/masstransit/iconsumemessageobserverconnector), [IConsumeObserverConnector](../../masstransit-abstractions/masstransit/iconsumeobserverconnector), [IReceiveObserverConnector](../../masstransit-abstractions/masstransit/ireceiveobserverconnector), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [IReceiveEndpointObserverConnector](../../masstransit-abstractions/masstransit/ireceiveendpointobserverconnector), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [ISqlHost](../masstransit-sqltransport/isqlhost), [IHost\<ISqlReceiveEndpointConfigurator\>](../masstransit-transports/ihost-1), [IReceiveConnector\<ISqlReceiveEndpointConfigurator\>](../../masstransit-abstractions/masstransit/ireceiveconnector-1)

## Properties

### **Topology**

```csharp
public ISqlBusTopology Topology { get; }
```

#### Property Value

[ISqlBusTopology](../masstransit/isqlbustopology)<br/>

### **Address**

```csharp
public Uri Address { get; }
```

#### Property Value

Uri<br/>

### **Topology**

```csharp
public IBusTopology Topology { get; }
```

#### Property Value

[IBusTopology](../../masstransit-abstractions/masstransit/ibustopology)<br/>

## Constructors

### **SqlHost(ISqlHostConfiguration, ISqlBusTopology)**

```csharp
public SqlHost(ISqlHostConfiguration hostConfiguration, ISqlBusTopology busTopology)
```

#### Parameters

`hostConfiguration` [ISqlHostConfiguration](../masstransit-sqltransport-configuration/isqlhostconfiguration)<br/>

`busTopology` [ISqlBusTopology](../masstransit/isqlbustopology)<br/>

## Methods

### **ConnectReceiveEndpoint(IEndpointDefinition, IEndpointNameFormatter, Action\<IReceiveEndpointConfigurator\>)**

```csharp
public HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter, Action<IReceiveEndpointConfigurator> configureEndpoint)
```

#### Parameters

`definition` [IEndpointDefinition](../../masstransit-abstractions/masstransit/iendpointdefinition)<br/>

`endpointNameFormatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

`configureEndpoint` [Action\<IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[HostReceiveEndpointHandle](../../masstransit-abstractions/masstransit/hostreceiveendpointhandle)<br/>

### **ConnectReceiveEndpoint(String, Action\<IReceiveEndpointConfigurator\>)**

```csharp
public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`configureEndpoint` [Action\<IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[HostReceiveEndpointHandle](../../masstransit-abstractions/masstransit/hostreceiveendpointhandle)<br/>

### **ConnectReceiveEndpoint(IEndpointDefinition, IEndpointNameFormatter, Action\<ISqlReceiveEndpointConfigurator\>)**

```csharp
public HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter, Action<ISqlReceiveEndpointConfigurator> configureEndpoint)
```

#### Parameters

`definition` [IEndpointDefinition](../../masstransit-abstractions/masstransit/iendpointdefinition)<br/>

`endpointNameFormatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

`configureEndpoint` [Action\<ISqlReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[HostReceiveEndpointHandle](../../masstransit-abstractions/masstransit/hostreceiveendpointhandle)<br/>

### **ConnectReceiveEndpoint(String, Action\<ISqlReceiveEndpointConfigurator\>)**

```csharp
public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<ISqlReceiveEndpointConfigurator> configure)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`configure` [Action\<ISqlReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[HostReceiveEndpointHandle](../../masstransit-abstractions/masstransit/hostreceiveendpointhandle)<br/>

### **Probe(ProbeContext)**

```csharp
protected void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **GetAgentHandles()**

```csharp
protected IAgent[] GetAgentHandles()
```

#### Returns

[IAgent[]](../../masstransit-abstractions/masstransit/iagent)<br/>
