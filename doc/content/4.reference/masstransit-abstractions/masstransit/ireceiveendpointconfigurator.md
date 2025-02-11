---

title: IReceiveEndpointConfigurator

---

# IReceiveEndpointConfigurator

Namespace: MassTransit

Configure a receiving endpoint

```csharp
public interface IReceiveEndpointConfigurator : IEndpointConfigurator, IConsumePipeConfigurator, IPipeConfigurator<ConsumeContext>, IConsumerConfigurationObserverConnector, ISagaConfigurationObserverConnector, IHandlerConfigurationObserverConnector, IActivityConfigurationObserverConnector, IConsumerConfigurationObserver, ISagaConfigurationObserver, IHandlerConfigurationObserver, IActivityConfigurationObserver, ISendPipelineConfigurator, IPublishPipelineConfigurator, IReceivePipelineConfigurator, IReceiveEndpointObserverConnector, IReceiveEndpointDependencyConnector, IReceiveEndpointDependentConnector
```

Implements [IEndpointConfigurator](../masstransit/iendpointconfigurator), [IConsumePipeConfigurator](../masstransit/iconsumepipeconfigurator), [IPipeConfigurator\<ConsumeContext\>](../masstransit/ipipeconfigurator-1), [IConsumerConfigurationObserverConnector](../masstransit/iconsumerconfigurationobserverconnector), [ISagaConfigurationObserverConnector](../masstransit/isagaconfigurationobserverconnector), [IHandlerConfigurationObserverConnector](../masstransit/ihandlerconfigurationobserverconnector), [IActivityConfigurationObserverConnector](../masstransit/iactivityconfigurationobserverconnector), [IConsumerConfigurationObserver](../masstransit/iconsumerconfigurationobserver), [ISagaConfigurationObserver](../masstransit/isagaconfigurationobserver), [IHandlerConfigurationObserver](../masstransit/ihandlerconfigurationobserver), [IActivityConfigurationObserver](../masstransit/iactivityconfigurationobserver), [ISendPipelineConfigurator](../masstransit/isendpipelineconfigurator), [IPublishPipelineConfigurator](../masstransit/ipublishpipelineconfigurator), [IReceivePipelineConfigurator](../masstransit/ireceivepipelineconfigurator), [IReceiveEndpointObserverConnector](../masstransit/ireceiveendpointobserverconnector), [IReceiveEndpointDependencyConnector](../masstransit/ireceiveendpointdependencyconnector), [IReceiveEndpointDependentConnector](../masstransit/ireceiveendpointdependentconnector)

## Properties

### **InputAddress**

Returns the input address of the receive endpoint

```csharp
public abstract Uri InputAddress { get; }
```

#### Property Value

Uri<br/>

### **ConfigureConsumeTopology**

If true (the default), the broker topology is configured using the message types consumed by
 handlers, consumers, sagas, and activities. The implementation is broker-specific, but generally
 supported enough to be implemented across the board. This method obsoletes the previous methods,
 such as BindMessageTopics, BindMessageExchanges, SubscribeMessageTopics, etc.

```csharp
public abstract bool ConfigureConsumeTopology { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **PublishFaults**

If true (the default), faults should be published when no ResponseAddress or FaultAddress are present.

```csharp
public abstract bool PublishFaults { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **PrefetchCount**

Specify the number of messages to prefetch from the message broker

```csharp
public abstract int PrefetchCount { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The limit

### **ConcurrentMessageLimit**

Specify the number of concurrent messages that can be consumed (separate from prefetch count)

```csharp
public abstract Nullable<int> ConcurrentMessageLimit { get; set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **DefaultContentType**

When deserializing a message, if no ContentType is present on the receive context, use this as the default

```csharp
public abstract ContentType DefaultContentType { set; }
```

#### Property Value

ContentType<br/>

### **SerializerContentType**

When serializing a message, use the content type specified for serialization

```csharp
public abstract ContentType SerializerContentType { set; }
```

#### Property Value

ContentType<br/>

## Methods

### **ConfigureMessageTopology\<T\>(Boolean)**

Configures whether the broker topology is configured for the specified message type. Related to
 [IReceiveEndpointConfigurator.ConfigureConsumeTopology](ireceiveendpointconfigurator#configureconsumetopology), but for an individual message type.

```csharp
void ConfigureMessageTopology<T>(bool enabled)
```

#### Type Parameters

`T`<br/>

#### Parameters

`enabled` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **ConfigureMessageTopology(Type, Boolean)**

Configures whether the broker topology is configured for the specified message type. Related to
 [IReceiveEndpointConfigurator.ConfigureConsumeTopology](ireceiveendpointconfigurator#configureconsumetopology), but for an individual message type.

```csharp
void ConfigureMessageTopology(Type messageType, bool enabled)
```

#### Parameters

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`enabled` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **AddEndpointSpecification(IReceiveEndpointSpecification)**

```csharp
void AddEndpointSpecification(IReceiveEndpointSpecification configurator)
```

#### Parameters

`configurator` [IReceiveEndpointSpecification](../masstransit/ireceiveendpointspecification)<br/>

### **AddSerializer(ISerializerFactory, Boolean)**

Add a message serializer using the specified factory (can be shared by serializer/deserializer)

```csharp
void AddSerializer(ISerializerFactory factory, bool isSerializer)
```

#### Parameters

`factory` [ISerializerFactory](../masstransit/iserializerfactory)<br/>

`isSerializer` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
If true, set the current serializer to the specified factory

### **AddDeserializer(ISerializerFactory, Boolean)**

Add a message deserializer using the specified factory (can be shared by serializer/deserializer)

```csharp
void AddDeserializer(ISerializerFactory factory, bool isDefault)
```

#### Parameters

`factory` [ISerializerFactory](../masstransit/iserializerfactory)<br/>

`isDefault` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
If true, set the default content type to the content type of the deserializer

### **ClearSerialization()**

Clears all message serialization configuration

```csharp
void ClearSerialization()
```
