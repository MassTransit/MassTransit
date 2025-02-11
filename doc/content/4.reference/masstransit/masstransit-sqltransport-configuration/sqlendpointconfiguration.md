---

title: SqlEndpointConfiguration

---

# SqlEndpointConfiguration

Namespace: MassTransit.SqlTransport.Configuration

```csharp
public class SqlEndpointConfiguration : EndpointConfiguration, IEndpointConfiguration, IConsumePipeConfigurator, IPipeConfigurator<ConsumeContext>, IConsumerConfigurationObserverConnector, ISagaConfigurationObserverConnector, IHandlerConfigurationObserverConnector, IActivityConfigurationObserverConnector, IConsumerConfigurationObserver, ISagaConfigurationObserver, IHandlerConfigurationObserver, IActivityConfigurationObserver, ISendPipelineConfigurator, IPublishPipelineConfigurator, IReceivePipelineConfigurator, ISpecification, ISqlEndpointConfiguration
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [EndpointConfiguration](../masstransit-configuration/endpointconfiguration) → [SqlEndpointConfiguration](../masstransit-sqltransport-configuration/sqlendpointconfiguration)<br/>
Implements [IEndpointConfiguration](../masstransit-configuration/iendpointconfiguration), [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator), [IPipeConfigurator\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserverconnector), [ISagaConfigurationObserverConnector](../../masstransit-abstractions/masstransit/isagaconfigurationobserverconnector), [IHandlerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserverconnector), [IActivityConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iactivityconfigurationobserverconnector), [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver), [ISagaConfigurationObserver](../../masstransit-abstractions/masstransit/isagaconfigurationobserver), [IHandlerConfigurationObserver](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserver), [IActivityConfigurationObserver](../../masstransit-abstractions/masstransit/iactivityconfigurationobserver), [ISendPipelineConfigurator](../../masstransit-abstractions/masstransit/isendpipelineconfigurator), [IPublishPipelineConfigurator](../../masstransit-abstractions/masstransit/ipublishpipelineconfigurator), [IReceivePipelineConfigurator](../../masstransit-abstractions/masstransit/ireceivepipelineconfigurator), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [ISqlEndpointConfiguration](../masstransit-sqltransport-configuration/isqlendpointconfiguration)

## Properties

### **Topology**

```csharp
public ISqlTopologyConfiguration Topology { get; }
```

#### Property Value

[ISqlTopologyConfiguration](../masstransit-sqltransport-configuration/isqltopologyconfiguration)<br/>

### **ConcurrentMessageLimit**

```csharp
public Nullable<int> ConcurrentMessageLimit { get; set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **PrefetchCount**

```csharp
public int PrefetchCount { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **DefaultContentType**

```csharp
public ContentType DefaultContentType { set; }
```

#### Property Value

ContentType<br/>

### **SerializerContentType**

```csharp
public ContentType SerializerContentType { set; }
```

#### Property Value

ContentType<br/>

### **IsBusEndpoint**

```csharp
public bool IsBusEndpoint { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **AutoStart**

```csharp
public bool AutoStart { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Consume**

```csharp
public IConsumePipeConfiguration Consume { get; }
```

#### Property Value

[IConsumePipeConfiguration](../masstransit-configuration/iconsumepipeconfiguration)<br/>

### **Send**

```csharp
public ISendPipeConfiguration Send { get; }
```

#### Property Value

[ISendPipeConfiguration](../masstransit-configuration/isendpipeconfiguration)<br/>

### **Publish**

```csharp
public IPublishPipeConfiguration Publish { get; }
```

#### Property Value

[IPublishPipeConfiguration](../masstransit-configuration/ipublishpipeconfiguration)<br/>

### **Receive**

```csharp
public IReceivePipeConfiguration Receive { get; }
```

#### Property Value

[IReceivePipeConfiguration](../masstransit-configuration/ireceivepipeconfiguration)<br/>

### **Topology**

```csharp
public ITopologyConfiguration Topology { get; }
```

#### Property Value

[ITopologyConfiguration](../masstransit-configuration/itopologyconfiguration)<br/>

### **Serialization**

```csharp
public ISerializationConfiguration Serialization { get; }
```

#### Property Value

[ISerializationConfiguration](../masstransit-configuration/iserializationconfiguration)<br/>

### **Transport**

```csharp
public ITransportConfiguration Transport { get; }
```

#### Property Value

[ITransportConfiguration](../masstransit-configuration/itransportconfiguration)<br/>

## Constructors

### **SqlEndpointConfiguration(ISqlTopologyConfiguration)**

```csharp
public SqlEndpointConfiguration(ISqlTopologyConfiguration topologyConfiguration)
```

#### Parameters

`topologyConfiguration` [ISqlTopologyConfiguration](../masstransit-sqltransport-configuration/isqltopologyconfiguration)<br/>

## Methods

### **CreateEndpointConfiguration(Boolean)**

```csharp
public ISqlEndpointConfiguration CreateEndpointConfiguration(bool isBusEndpoint)
```

#### Parameters

`isBusEndpoint` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

#### Returns

[ISqlEndpointConfiguration](../masstransit-sqltransport-configuration/isqlendpointconfiguration)<br/>
