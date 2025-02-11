---

title: InMemoryReceiveEndpointConfiguration

---

# InMemoryReceiveEndpointConfiguration

Namespace: MassTransit.InMemoryTransport.Configuration

```csharp
public class InMemoryReceiveEndpointConfiguration : ReceiveEndpointConfiguration, IEndpointConfiguration, IConsumePipeConfigurator, IPipeConfigurator<ConsumeContext>, IConsumerConfigurationObserverConnector, ISagaConfigurationObserverConnector, IHandlerConfigurationObserverConnector, IActivityConfigurationObserverConnector, IConsumerConfigurationObserver, ISagaConfigurationObserver, IHandlerConfigurationObserver, IActivityConfigurationObserver, ISendPipelineConfigurator, IPublishPipelineConfigurator, IReceivePipelineConfigurator, ISpecification, IReceiveEndpointConfiguration, IReceiveEndpointObserverConnector, IReceiveEndpointDependentConnector, IInMemoryReceiveEndpointConfiguration, IInMemoryEndpointConfiguration, IInMemoryReceiveEndpointConfigurator, IReceiveEndpointConfigurator, IEndpointConfigurator, IReceiveEndpointDependencyConnector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [EndpointConfiguration](../masstransit-configuration/endpointconfiguration) → [ReceiveEndpointConfiguration](../masstransit-configuration/receiveendpointconfiguration) → [InMemoryReceiveEndpointConfiguration](../masstransit-inmemorytransport-configuration/inmemoryreceiveendpointconfiguration)<br/>
Implements [IEndpointConfiguration](../masstransit-configuration/iendpointconfiguration), [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator), [IPipeConfigurator\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserverconnector), [ISagaConfigurationObserverConnector](../../masstransit-abstractions/masstransit/isagaconfigurationobserverconnector), [IHandlerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserverconnector), [IActivityConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iactivityconfigurationobserverconnector), [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver), [ISagaConfigurationObserver](../../masstransit-abstractions/masstransit/isagaconfigurationobserver), [IHandlerConfigurationObserver](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserver), [IActivityConfigurationObserver](../../masstransit-abstractions/masstransit/iactivityconfigurationobserver), [ISendPipelineConfigurator](../../masstransit-abstractions/masstransit/isendpipelineconfigurator), [IPublishPipelineConfigurator](../../masstransit-abstractions/masstransit/ipublishpipelineconfigurator), [IReceivePipelineConfigurator](../../masstransit-abstractions/masstransit/ireceivepipelineconfigurator), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IReceiveEndpointConfiguration](../masstransit-configuration/ireceiveendpointconfiguration), [IReceiveEndpointObserverConnector](../../masstransit-abstractions/masstransit/ireceiveendpointobserverconnector), [IReceiveEndpointDependentConnector](../../masstransit-abstractions/masstransit/ireceiveendpointdependentconnector), [IInMemoryReceiveEndpointConfiguration](../masstransit-inmemorytransport-configuration/iinmemoryreceiveendpointconfiguration), [IInMemoryEndpointConfiguration](../masstransit-inmemorytransport-configuration/iinmemoryendpointconfiguration), [IInMemoryReceiveEndpointConfigurator](../masstransit/iinmemoryreceiveendpointconfigurator), [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator), [IEndpointConfigurator](../../masstransit-abstractions/masstransit/iendpointconfigurator), [IReceiveEndpointDependencyConnector](../../masstransit-abstractions/masstransit/ireceiveendpointdependencyconnector)

## Properties

### **HostAddress**

```csharp
public Uri HostAddress { get; }
```

#### Property Value

Uri<br/>

### **InputAddress**

```csharp
public Uri InputAddress { get; }
```

#### Property Value

Uri<br/>

### **EndpointObservers**

```csharp
public ReceiveEndpointObservable EndpointObservers { get; }
```

#### Property Value

[ReceiveEndpointObservable](../../masstransit-abstractions/masstransit-observables/receiveendpointobservable)<br/>

### **ReceiveObservers**

```csharp
public ReceiveObservable ReceiveObservers { get; }
```

#### Property Value

[ReceiveObservable](../../masstransit-abstractions/masstransit-observables/receiveobservable)<br/>

### **TransportObservers**

```csharp
public ReceiveTransportObservable TransportObservers { get; }
```

#### Property Value

[ReceiveTransportObservable](../../masstransit-abstractions/masstransit-observables/receivetransportobservable)<br/>

### **ConfigureConsumeTopology**

```csharp
public bool ConfigureConsumeTopology { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **PublishFaults**

```csharp
public bool PublishFaults { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **ConsumePipe**

```csharp
public IConsumePipe ConsumePipe { get; }
```

#### Property Value

[IConsumePipe](../../masstransit-abstractions/masstransit-transports/iconsumepipe)<br/>

### **ReceiveEndpoint**

```csharp
public IReceiveEndpoint ReceiveEndpoint { get; protected set; }
```

#### Property Value

[IReceiveEndpoint](../../masstransit-abstractions/masstransit/ireceiveendpoint)<br/>

### **DependenciesReady**

```csharp
public Task DependenciesReady { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **DependentsCompleted**

```csharp
public Task DependentsCompleted { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

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

### **InMemoryReceiveEndpointConfiguration(IInMemoryHostConfiguration, String, IInMemoryEndpointConfiguration)**

```csharp
public InMemoryReceiveEndpointConfiguration(IInMemoryHostConfiguration hostConfiguration, string queueName, IInMemoryEndpointConfiguration endpointConfiguration)
```

#### Parameters

`hostConfiguration` [IInMemoryHostConfiguration](../masstransit-inmemorytransport-configuration/iinmemoryhostconfiguration)<br/>

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`endpointConfiguration` [IInMemoryEndpointConfiguration](../masstransit-inmemorytransport-configuration/iinmemoryendpointconfiguration)<br/>

## Methods

### **CreateReceiveEndpointContext()**

```csharp
public ReceiveEndpointContext CreateReceiveEndpointContext()
```

#### Returns

[ReceiveEndpointContext](../masstransit-transports/receiveendpointcontext)<br/>

### **Build(IHost)**

```csharp
public void Build(IHost host)
```

#### Parameters

`host` [IHost](../masstransit-transports/ihost)<br/>

### **Bind(String, ExchangeType, String)**

```csharp
public void Bind(string exchangeName, ExchangeType exchangeType, string routingKey)
```

#### Parameters

`exchangeName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`exchangeType` [ExchangeType](../masstransit-transports-fabric/exchangetype)<br/>

`routingKey` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Bind\<T\>(ExchangeType, String)**

```csharp
public void Bind<T>(ExchangeType exchangeType, string routingKey)
```

#### Type Parameters

`T`<br/>

#### Parameters

`exchangeType` [ExchangeType](../masstransit-transports-fabric/exchangetype)<br/>

`routingKey` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
