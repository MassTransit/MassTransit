---

title: SqlHostConfiguration

---

# SqlHostConfiguration

Namespace: MassTransit.SqlTransport.Configuration

```csharp
public class SqlHostConfiguration : BaseHostConfiguration<ISqlReceiveEndpointConfiguration, ISqlReceiveEndpointConfigurator>, IHostConfiguration, IEndpointConfigurationObserverConnector, IReceiveObserverConnector, IConsumeObserverConnector, IPublishObserverConnector, ISendObserverConnector, ISpecification, IReceiveConfigurator<ISqlReceiveEndpointConfigurator>, IReceiveConfigurator, ISqlHostConfiguration
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BaseHostConfiguration\<ISqlReceiveEndpointConfiguration, ISqlReceiveEndpointConfigurator\>](../masstransit-configuration/basehostconfiguration-2) → [SqlHostConfiguration](../masstransit-sqltransport-configuration/sqlhostconfiguration)<br/>
Implements [IHostConfiguration](../masstransit-configuration/ihostconfiguration), [IEndpointConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iendpointconfigurationobserverconnector), [IReceiveObserverConnector](../../masstransit-abstractions/masstransit/ireceiveobserverconnector), [IConsumeObserverConnector](../../masstransit-abstractions/masstransit/iconsumeobserverconnector), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IReceiveConfigurator\<ISqlReceiveEndpointConfigurator\>](../../masstransit-abstractions/masstransit/ireceiveconfigurator-1), [IReceiveConfigurator](../../masstransit-abstractions/masstransit/ireceiveconfigurator), [ISqlHostConfiguration](../masstransit-sqltransport-configuration/isqlhostconfiguration)

## Properties

### **ConnectionContextSupervisor**

```csharp
public IConnectionContextSupervisor ConnectionContextSupervisor { get; }
```

#### Property Value

[IConnectionContextSupervisor](../masstransit-sqltransport/iconnectioncontextsupervisor)<br/>

### **HostAddress**

```csharp
public Uri HostAddress { get; }
```

#### Property Value

Uri<br/>

### **ReceiveTransportRetryPolicy**

```csharp
public IRetryPolicy ReceiveTransportRetryPolicy { get; }
```

#### Property Value

[IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

### **Topology**

```csharp
public IBusTopology Topology { get; }
```

#### Property Value

[IBusTopology](../../masstransit-abstractions/masstransit/ibustopology)<br/>

### **Settings**

```csharp
public SqlHostSettings Settings { get; set; }
```

#### Property Value

[SqlHostSettings](../masstransit/sqlhostsettings)<br/>

### **BusConfiguration**

```csharp
public IBusConfiguration BusConfiguration { get; }
```

#### Property Value

[IBusConfiguration](../masstransit-configuration/ibusconfiguration)<br/>

### **DeployTopologyOnly**

```csharp
public bool DeployTopologyOnly { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **DeployPublishTopology**

```csharp
public bool DeployPublishTopology { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **SendObservers**

```csharp
public ISendObserver SendObservers { get; }
```

#### Property Value

[ISendObserver](../../masstransit-abstractions/masstransit/isendobserver)<br/>

### **LogContext**

```csharp
public ILogContext LogContext { get; set; }
```

#### Property Value

[ILogContext](../masstransit-logging/ilogcontext)<br/>

### **ReceiveLogContext**

```csharp
public ILogContext ReceiveLogContext { get; }
```

#### Property Value

[ILogContext](../masstransit-logging/ilogcontext)<br/>

### **SendLogContext**

```csharp
public ILogContext SendLogContext { get; }
```

#### Property Value

[ILogContext](../masstransit-logging/ilogcontext)<br/>

### **SendTransportRetryPolicy**

```csharp
public IRetryPolicy SendTransportRetryPolicy { get; }
```

#### Property Value

[IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

### **ConsumerStopTimeout**

```csharp
public Nullable<TimeSpan> ConsumerStopTimeout { get; set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **StopTimeout**

```csharp
public Nullable<TimeSpan> StopTimeout { get; set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **SqlHostConfiguration(ISqlBusConfiguration, ISqlTopologyConfiguration)**

```csharp
public SqlHostConfiguration(ISqlBusConfiguration busConfiguration, ISqlTopologyConfiguration topologyConfiguration)
```

#### Parameters

`busConfiguration` [ISqlBusConfiguration](../masstransit-sqltransport-configuration/isqlbusconfiguration)<br/>

`topologyConfiguration` [ISqlTopologyConfiguration](../masstransit-sqltransport-configuration/isqltopologyconfiguration)<br/>

## Methods

### **ApplyEndpointDefinition(ISqlReceiveEndpointConfigurator, IEndpointDefinition)**

```csharp
public void ApplyEndpointDefinition(ISqlReceiveEndpointConfigurator configurator, IEndpointDefinition definition)
```

#### Parameters

`configurator` [ISqlReceiveEndpointConfigurator](../masstransit/isqlreceiveendpointconfigurator)<br/>

`definition` [IEndpointDefinition](../../masstransit-abstractions/masstransit/iendpointdefinition)<br/>

### **CreateReceiveEndpointConfiguration(String, Action\<ISqlReceiveEndpointConfigurator\>)**

```csharp
public ISqlReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName, Action<ISqlReceiveEndpointConfigurator> configure)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`configure` [Action\<ISqlReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ISqlReceiveEndpointConfiguration](../masstransit-sqltransport-configuration/isqlreceiveendpointconfiguration)<br/>

### **CreateReceiveEndpointConfiguration(SqlReceiveSettings, ISqlEndpointConfiguration, Action\<ISqlReceiveEndpointConfigurator\>)**

```csharp
public ISqlReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(SqlReceiveSettings settings, ISqlEndpointConfiguration endpointConfiguration, Action<ISqlReceiveEndpointConfigurator> configure)
```

#### Parameters

`settings` [SqlReceiveSettings](../masstransit-sqltransport-configuration/sqlreceivesettings)<br/>

`endpointConfiguration` [ISqlEndpointConfiguration](../masstransit-sqltransport-configuration/isqlendpointconfiguration)<br/>

`configure` [Action\<ISqlReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ISqlReceiveEndpointConfiguration](../masstransit-sqltransport-configuration/isqlreceiveendpointconfiguration)<br/>

### **ReceiveEndpoint(IEndpointDefinition, IEndpointNameFormatter, Action\<ISqlReceiveEndpointConfigurator\>)**

```csharp
public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter, Action<ISqlReceiveEndpointConfigurator> configureEndpoint)
```

#### Parameters

`definition` [IEndpointDefinition](../../masstransit-abstractions/masstransit/iendpointdefinition)<br/>

`endpointNameFormatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

`configureEndpoint` [Action\<ISqlReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ReceiveEndpoint(String, Action\<ISqlReceiveEndpointConfigurator\>)**

```csharp
public void ReceiveEndpoint(string queueName, Action<ISqlReceiveEndpointConfigurator> configureEndpoint)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`configureEndpoint` [Action\<ISqlReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **CreateReceiveEndpointConfiguration(String, Action\<IReceiveEndpointConfigurator\>)**

```csharp
public IReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName, Action<IReceiveEndpointConfigurator> configure)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`configure` [Action\<IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IReceiveEndpointConfiguration](../masstransit-configuration/ireceiveendpointconfiguration)<br/>

### **Build()**

```csharp
public IHost Build()
```

#### Returns

[IHost](../masstransit-transports/ihost)<br/>
