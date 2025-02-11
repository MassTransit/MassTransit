---

title: InMemoryBusConfiguration

---

# InMemoryBusConfiguration

Namespace: MassTransit.InMemoryTransport.Configuration

```csharp
public class InMemoryBusConfiguration : InMemoryEndpointConfiguration, IEndpointConfiguration, IConsumePipeConfigurator, IPipeConfigurator<ConsumeContext>, IConsumerConfigurationObserverConnector, ISagaConfigurationObserverConnector, IHandlerConfigurationObserverConnector, IActivityConfigurationObserverConnector, IConsumerConfigurationObserver, ISagaConfigurationObserver, IHandlerConfigurationObserver, IActivityConfigurationObserver, ISendPipelineConfigurator, IPublishPipelineConfigurator, IReceivePipelineConfigurator, ISpecification, IInMemoryEndpointConfiguration, IInMemoryBusConfiguration, IBusConfiguration, IBusObserverConnector, IEndpointConfigurationObserverConnector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [EndpointConfiguration](../masstransit-configuration/endpointconfiguration) → [InMemoryEndpointConfiguration](../masstransit-inmemorytransport-configuration/inmemoryendpointconfiguration) → [InMemoryBusConfiguration](../masstransit-inmemorytransport-configuration/inmemorybusconfiguration)<br/>
Implements [IEndpointConfiguration](../masstransit-configuration/iendpointconfiguration), [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator), [IPipeConfigurator\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserverconnector), [ISagaConfigurationObserverConnector](../../masstransit-abstractions/masstransit/isagaconfigurationobserverconnector), [IHandlerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserverconnector), [IActivityConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iactivityconfigurationobserverconnector), [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver), [ISagaConfigurationObserver](../../masstransit-abstractions/masstransit/isagaconfigurationobserver), [IHandlerConfigurationObserver](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserver), [IActivityConfigurationObserver](../../masstransit-abstractions/masstransit/iactivityconfigurationobserver), [ISendPipelineConfigurator](../../masstransit-abstractions/masstransit/isendpipelineconfigurator), [IPublishPipelineConfigurator](../../masstransit-abstractions/masstransit/ipublishpipelineconfigurator), [IReceivePipelineConfigurator](../../masstransit-abstractions/masstransit/ireceivepipelineconfigurator), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IInMemoryEndpointConfiguration](../masstransit-inmemorytransport-configuration/iinmemoryendpointconfiguration), [IInMemoryBusConfiguration](../masstransit-inmemorytransport-configuration/iinmemorybusconfiguration), [IBusConfiguration](../masstransit-configuration/ibusconfiguration), [IBusObserverConnector](../../masstransit-abstractions/masstransit/ibusobserverconnector), [IEndpointConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iendpointconfigurationobserverconnector)

## Properties

### **BusEndpointConfiguration**

```csharp
public IInMemoryEndpointConfiguration BusEndpointConfiguration { get; }
```

#### Property Value

[IInMemoryEndpointConfiguration](../masstransit-inmemorytransport-configuration/iinmemoryendpointconfiguration)<br/>

### **HostConfiguration**

```csharp
public IInMemoryHostConfiguration HostConfiguration { get; }
```

#### Property Value

[IInMemoryHostConfiguration](../masstransit-inmemorytransport-configuration/iinmemoryhostconfiguration)<br/>

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

### **InMemoryBusConfiguration(IInMemoryTopologyConfiguration, Uri)**

```csharp
public InMemoryBusConfiguration(IInMemoryTopologyConfiguration topologyConfiguration, Uri baseAddress)
```

#### Parameters

`topologyConfiguration` [IInMemoryTopologyConfiguration](../masstransit-inmemorytransport-configuration/iinmemorytopologyconfiguration)<br/>

`baseAddress` Uri<br/>

## Methods

### **ConnectBusObserver(IBusObserver)**

```csharp
public ConnectHandle ConnectBusObserver(IBusObserver observer)
```

#### Parameters

`observer` [IBusObserver](../../masstransit-abstractions/masstransit/ibusobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver)**

```csharp
public ConnectHandle ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver observer)
```

#### Parameters

`observer` [IEndpointConfigurationObserver](../../masstransit-abstractions/masstransit/iendpointconfigurationobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
