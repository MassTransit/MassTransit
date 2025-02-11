---

title: MediatorConfiguration

---

# MediatorConfiguration

Namespace: MassTransit.Configuration

```csharp
public class MediatorConfiguration : ReceivePipeDispatcherConfiguration, IEndpointConfiguration, IConsumePipeConfigurator, IPipeConfigurator<ConsumeContext>, IConsumerConfigurationObserverConnector, ISagaConfigurationObserverConnector, IHandlerConfigurationObserverConnector, IActivityConfigurationObserverConnector, IConsumerConfigurationObserver, ISagaConfigurationObserver, IHandlerConfigurationObserver, IActivityConfigurationObserver, ISendPipelineConfigurator, IPublishPipelineConfigurator, IReceivePipelineConfigurator, ISpecification, IReceiveEndpointConfigurator, IEndpointConfigurator, IReceiveEndpointObserverConnector, IReceiveEndpointDependencyConnector, IReceiveEndpointDependentConnector, IMediatorConfigurator, IConsumeObserverConnector, ISendObserverConnector, IPublishObserverConnector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [EndpointConfiguration](../masstransit-configuration/endpointconfiguration) → [ReceiverConfiguration](../masstransit-configuration/receiverconfiguration) → [ReceivePipeDispatcherConfiguration](../masstransit-configuration/receivepipedispatcherconfiguration) → [MediatorConfiguration](../masstransit-configuration/mediatorconfiguration)<br/>
Implements [IEndpointConfiguration](../masstransit-configuration/iendpointconfiguration), [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator), [IPipeConfigurator\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserverconnector), [ISagaConfigurationObserverConnector](../../masstransit-abstractions/masstransit/isagaconfigurationobserverconnector), [IHandlerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserverconnector), [IActivityConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iactivityconfigurationobserverconnector), [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver), [ISagaConfigurationObserver](../../masstransit-abstractions/masstransit/isagaconfigurationobserver), [IHandlerConfigurationObserver](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserver), [IActivityConfigurationObserver](../../masstransit-abstractions/masstransit/iactivityconfigurationobserver), [ISendPipelineConfigurator](../../masstransit-abstractions/masstransit/isendpipelineconfigurator), [IPublishPipelineConfigurator](../../masstransit-abstractions/masstransit/ipublishpipelineconfigurator), [IReceivePipelineConfigurator](../../masstransit-abstractions/masstransit/ireceivepipelineconfigurator), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator), [IEndpointConfigurator](../../masstransit-abstractions/masstransit/iendpointconfigurator), [IReceiveEndpointObserverConnector](../../masstransit-abstractions/masstransit/ireceiveendpointobserverconnector), [IReceiveEndpointDependencyConnector](../../masstransit-abstractions/masstransit/ireceiveendpointdependencyconnector), [IReceiveEndpointDependentConnector](../../masstransit-abstractions/masstransit/ireceiveendpointdependentconnector), [IMediatorConfigurator](../masstransit/imediatorconfigurator), [IConsumeObserverConnector](../../masstransit-abstractions/masstransit/iconsumeobserverconnector), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector)

## Properties

### **InputAddress**

```csharp
public Uri InputAddress { get; }
```

#### Property Value

Uri<br/>

### **ConfigureConsumeTopology**

```csharp
public bool ConfigureConsumeTopology { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **PublishFaults**

```csharp
public bool PublishFaults { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

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

### **MediatorConfiguration(IHostConfiguration, IReceiveEndpointConfiguration)**

```csharp
public MediatorConfiguration(IHostConfiguration hostConfiguration, IReceiveEndpointConfiguration endpointConfiguration)
```

#### Parameters

`hostConfiguration` [IHostConfiguration](../masstransit-configuration/ihostconfiguration)<br/>

`endpointConfiguration` [IReceiveEndpointConfiguration](../masstransit-configuration/ireceiveendpointconfiguration)<br/>

## Methods

### **ConnectConsumeObserver(IConsumeObserver)**

```csharp
public ConnectHandle ConnectConsumeObserver(IConsumeObserver observer)
```

#### Parameters

`observer` [IConsumeObserver](../../masstransit-abstractions/masstransit/iconsumeobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectSendObserver(ISendObserver)**

```csharp
public ConnectHandle ConnectSendObserver(ISendObserver observer)
```

#### Parameters

`observer` [ISendObserver](../../masstransit-abstractions/masstransit/isendobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectPublishObserver(IPublishObserver)**

```csharp
public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
```

#### Parameters

`observer` [IPublishObserver](../../masstransit-abstractions/masstransit/ipublishobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
