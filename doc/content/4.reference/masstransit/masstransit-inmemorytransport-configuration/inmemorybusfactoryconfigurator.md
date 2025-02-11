---

title: InMemoryBusFactoryConfigurator

---

# InMemoryBusFactoryConfigurator

Namespace: MassTransit.InMemoryTransport.Configuration

```csharp
public class InMemoryBusFactoryConfigurator : BusFactoryConfigurator, IConsumePipeConfigurator, IPipeConfigurator<ConsumeContext>, IConsumerConfigurationObserverConnector, ISagaConfigurationObserverConnector, IHandlerConfigurationObserverConnector, IActivityConfigurationObserverConnector, IConsumerConfigurationObserver, ISagaConfigurationObserver, IHandlerConfigurationObserver, IActivityConfigurationObserver, ISendPipelineConfigurator, IPublishPipelineConfigurator, IEndpointConfigurationObserverConnector, IBusObserverConnector, IReceiveObserverConnector, IConsumeObserverConnector, ISendObserverConnector, IPublishObserverConnector, IInMemoryBusFactoryConfigurator, IBusFactoryConfigurator<IInMemoryReceiveEndpointConfigurator>, IBusFactoryConfigurator, IReceiveConfigurator, IReceiveConfigurator<IInMemoryReceiveEndpointConfigurator>, IBusFactory, ISpecification
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BusFactoryConfigurator](../masstransit-configuration/busfactoryconfigurator) → [InMemoryBusFactoryConfigurator](../masstransit-inmemorytransport-configuration/inmemorybusfactoryconfigurator)<br/>
Implements [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator), [IPipeConfigurator\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserverconnector), [ISagaConfigurationObserverConnector](../../masstransit-abstractions/masstransit/isagaconfigurationobserverconnector), [IHandlerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserverconnector), [IActivityConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iactivityconfigurationobserverconnector), [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver), [ISagaConfigurationObserver](../../masstransit-abstractions/masstransit/isagaconfigurationobserver), [IHandlerConfigurationObserver](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserver), [IActivityConfigurationObserver](../../masstransit-abstractions/masstransit/iactivityconfigurationobserver), [ISendPipelineConfigurator](../../masstransit-abstractions/masstransit/isendpipelineconfigurator), [IPublishPipelineConfigurator](../../masstransit-abstractions/masstransit/ipublishpipelineconfigurator), [IEndpointConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iendpointconfigurationobserverconnector), [IBusObserverConnector](../../masstransit-abstractions/masstransit/ibusobserverconnector), [IReceiveObserverConnector](../../masstransit-abstractions/masstransit/ireceiveobserverconnector), [IConsumeObserverConnector](../../masstransit-abstractions/masstransit/iconsumeobserverconnector), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [IInMemoryBusFactoryConfigurator](../masstransit/iinmemorybusfactoryconfigurator), [IBusFactoryConfigurator\<IInMemoryReceiveEndpointConfigurator\>](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator-1), [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator), [IReceiveConfigurator](../../masstransit-abstractions/masstransit/ireceiveconfigurator), [IReceiveConfigurator\<IInMemoryReceiveEndpointConfigurator\>](../../masstransit-abstractions/masstransit/ireceiveconfigurator-1), [IBusFactory](../masstransit/ibusfactory), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **TransportConcurrencyLimit**

```csharp
public int TransportConcurrencyLimit { set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **AutoStart**

```csharp
public bool AutoStart { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **PublishTopology**

```csharp
public IInMemoryPublishTopologyConfigurator PublishTopology { get; }
```

#### Property Value

[IInMemoryPublishTopologyConfigurator](../masstransit/iinmemorypublishtopologyconfigurator)<br/>

### **MessageTopology**

```csharp
public IMessageTopologyConfigurator MessageTopology { get; }
```

#### Property Value

[IMessageTopologyConfigurator](../../masstransit-abstractions/masstransit-configuration/imessagetopologyconfigurator)<br/>

### **ConsumeTopology**

```csharp
public IConsumeTopologyConfigurator ConsumeTopology { get; }
```

#### Property Value

[IConsumeTopologyConfigurator](../../masstransit-abstractions/masstransit/iconsumetopologyconfigurator)<br/>

### **SendTopology**

```csharp
public ISendTopologyConfigurator SendTopology { get; }
```

#### Property Value

[ISendTopologyConfigurator](../../masstransit-abstractions/masstransit/isendtopologyconfigurator)<br/>

### **PublishTopology**

```csharp
public IPublishTopologyConfigurator PublishTopology { get; }
```

#### Property Value

[IPublishTopologyConfigurator](../../masstransit-abstractions/masstransit/ipublishtopologyconfigurator)<br/>

### **DeployTopologyOnly**

```csharp
public bool DeployTopologyOnly { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **DeployPublishTopology**

```csharp
public bool DeployPublishTopology { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **ConcurrentMessageLimit**

```csharp
public Nullable<int> ConcurrentMessageLimit { set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **PrefetchCount**

```csharp
public int PrefetchCount { set; }
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

## Constructors

### **InMemoryBusFactoryConfigurator(IInMemoryBusConfiguration)**

```csharp
public InMemoryBusFactoryConfigurator(IInMemoryBusConfiguration busConfiguration)
```

#### Parameters

`busConfiguration` [IInMemoryBusConfiguration](../masstransit-inmemorytransport-configuration/iinmemorybusconfiguration)<br/>

## Methods

### **CreateBusEndpointConfiguration(Action\<IReceiveEndpointConfigurator\>)**

```csharp
public IReceiveEndpointConfiguration CreateBusEndpointConfiguration(Action<IReceiveEndpointConfigurator> configure)
```

#### Parameters

`configure` [Action\<IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IReceiveEndpointConfiguration](../masstransit-configuration/ireceiveendpointconfiguration)<br/>

### **Publish\<T\>(Action\<IInMemoryMessagePublishTopologyConfigurator\<T\>\>)**

```csharp
public void Publish<T>(Action<IInMemoryMessagePublishTopologyConfigurator<T>> configureTopology)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configureTopology` [Action\<IInMemoryMessagePublishTopologyConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Publish(Type, Action\<IInMemoryMessagePublishTopologyConfigurator\>)**

```csharp
public void Publish(Type messageType, Action<IInMemoryMessagePublishTopologyConfigurator> configure)
```

#### Parameters

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`configure` [Action\<IInMemoryMessagePublishTopologyConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Host(Action\<IInMemoryHostConfigurator\>)**

```csharp
public void Host(Action<IInMemoryHostConfigurator> configure)
```

#### Parameters

`configure` [Action\<IInMemoryHostConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Host(Uri, Action\<IInMemoryHostConfigurator\>)**

```csharp
public void Host(Uri baseAddress, Action<IInMemoryHostConfigurator> configure)
```

#### Parameters

`baseAddress` Uri<br/>

`configure` [Action\<IInMemoryHostConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Host(String, Action\<IInMemoryHostConfigurator\>)**

```csharp
public void Host(string virtualHost, Action<IInMemoryHostConfigurator> configure)
```

#### Parameters

`virtualHost` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`configure` [Action\<IInMemoryHostConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ReceiveEndpoint(IEndpointDefinition, IEndpointNameFormatter, Action\<IInMemoryReceiveEndpointConfigurator\>)**

```csharp
public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter, Action<IInMemoryReceiveEndpointConfigurator> configureEndpoint)
```

#### Parameters

`definition` [IEndpointDefinition](../../masstransit-abstractions/masstransit/iendpointdefinition)<br/>

`endpointNameFormatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

`configureEndpoint` [Action\<IInMemoryReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ReceiveEndpoint(IEndpointDefinition, IEndpointNameFormatter, Action\<IReceiveEndpointConfigurator\>)**

```csharp
public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter, Action<IReceiveEndpointConfigurator> configureEndpoint)
```

#### Parameters

`definition` [IEndpointDefinition](../../masstransit-abstractions/masstransit/iendpointdefinition)<br/>

`endpointNameFormatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

`configureEndpoint` [Action\<IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ReceiveEndpoint(String, Action\<IInMemoryReceiveEndpointConfigurator\>)**

```csharp
public void ReceiveEndpoint(string queueName, Action<IInMemoryReceiveEndpointConfigurator> configureEndpoint)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`configureEndpoint` [Action\<IInMemoryReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ReceiveEndpoint(String, Action\<IReceiveEndpointConfigurator\>)**

```csharp
public void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`configureEndpoint` [Action\<IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
