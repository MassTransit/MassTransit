---

title: ISqlHostConfiguration

---

# ISqlHostConfiguration

Namespace: MassTransit.SqlTransport.Configuration

```csharp
public interface ISqlHostConfiguration : IHostConfiguration, IEndpointConfigurationObserverConnector, IReceiveObserverConnector, IConsumeObserverConnector, IPublishObserverConnector, ISendObserverConnector, ISpecification, IReceiveConfigurator<ISqlReceiveEndpointConfigurator>, IReceiveConfigurator
```

Implements [IHostConfiguration](../masstransit-configuration/ihostconfiguration), [IEndpointConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iendpointconfigurationobserverconnector), [IReceiveObserverConnector](../../masstransit-abstractions/masstransit/ireceiveobserverconnector), [IConsumeObserverConnector](../../masstransit-abstractions/masstransit/iconsumeobserverconnector), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IReceiveConfigurator\<ISqlReceiveEndpointConfigurator\>](../../masstransit-abstractions/masstransit/ireceiveconfigurator-1), [IReceiveConfigurator](../../masstransit-abstractions/masstransit/ireceiveconfigurator)

## Properties

### **ConnectionContextSupervisor**

```csharp
public abstract IConnectionContextSupervisor ConnectionContextSupervisor { get; }
```

#### Property Value

[IConnectionContextSupervisor](../masstransit-sqltransport/iconnectioncontextsupervisor)<br/>

### **Settings**

```csharp
public abstract SqlHostSettings Settings { get; set; }
```

#### Property Value

[SqlHostSettings](../masstransit/sqlhostsettings)<br/>

### **Topology**

```csharp
public abstract ISqlBusTopology Topology { get; }
```

#### Property Value

[ISqlBusTopology](../masstransit/isqlbustopology)<br/>

## Methods

### **ApplyEndpointDefinition(ISqlReceiveEndpointConfigurator, IEndpointDefinition)**

Apply the endpoint definition to the receive endpoint configurator

```csharp
void ApplyEndpointDefinition(ISqlReceiveEndpointConfigurator configurator, IEndpointDefinition definition)
```

#### Parameters

`configurator` [ISqlReceiveEndpointConfigurator](../masstransit/isqlreceiveendpointconfigurator)<br/>

`definition` [IEndpointDefinition](../../masstransit-abstractions/masstransit/iendpointdefinition)<br/>

### **CreateReceiveEndpointConfiguration(String, Action\<ISqlReceiveEndpointConfigurator\>)**

```csharp
ISqlReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName, Action<ISqlReceiveEndpointConfigurator> configure)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`configure` [Action\<ISqlReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ISqlReceiveEndpointConfiguration](../masstransit-sqltransport-configuration/isqlreceiveendpointconfiguration)<br/>

### **CreateReceiveEndpointConfiguration(SqlReceiveSettings, ISqlEndpointConfiguration, Action\<ISqlReceiveEndpointConfigurator\>)**

```csharp
ISqlReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(SqlReceiveSettings settings, ISqlEndpointConfiguration endpointConfiguration, Action<ISqlReceiveEndpointConfigurator> configure)
```

#### Parameters

`settings` [SqlReceiveSettings](../masstransit-sqltransport-configuration/sqlreceivesettings)<br/>

`endpointConfiguration` [ISqlEndpointConfiguration](../masstransit-sqltransport-configuration/isqlendpointconfiguration)<br/>

`configure` [Action\<ISqlReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ISqlReceiveEndpointConfiguration](../masstransit-sqltransport-configuration/isqlreceiveendpointconfiguration)<br/>
