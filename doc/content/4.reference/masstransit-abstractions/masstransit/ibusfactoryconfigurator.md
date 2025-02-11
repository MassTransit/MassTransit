---

title: IBusFactoryConfigurator

---

# IBusFactoryConfigurator

Namespace: MassTransit

```csharp
public interface IBusFactoryConfigurator : IReceiveConfigurator, IEndpointConfigurationObserverConnector, IConsumePipeConfigurator, IPipeConfigurator<ConsumeContext>, IConsumerConfigurationObserverConnector, ISagaConfigurationObserverConnector, IHandlerConfigurationObserverConnector, IActivityConfigurationObserverConnector, IConsumerConfigurationObserver, ISagaConfigurationObserver, IHandlerConfigurationObserver, IActivityConfigurationObserver, ISendPipelineConfigurator, IPublishPipelineConfigurator, IBusObserverConnector, IReceiveObserverConnector, IConsumeObserverConnector, ISendObserverConnector, IPublishObserverConnector
```

Implements [IReceiveConfigurator](../masstransit/ireceiveconfigurator), [IEndpointConfigurationObserverConnector](../masstransit/iendpointconfigurationobserverconnector), [IConsumePipeConfigurator](../masstransit/iconsumepipeconfigurator), [IPipeConfigurator\<ConsumeContext\>](../masstransit/ipipeconfigurator-1), [IConsumerConfigurationObserverConnector](../masstransit/iconsumerconfigurationobserverconnector), [ISagaConfigurationObserverConnector](../masstransit/isagaconfigurationobserverconnector), [IHandlerConfigurationObserverConnector](../masstransit/ihandlerconfigurationobserverconnector), [IActivityConfigurationObserverConnector](../masstransit/iactivityconfigurationobserverconnector), [IConsumerConfigurationObserver](../masstransit/iconsumerconfigurationobserver), [ISagaConfigurationObserver](../masstransit/isagaconfigurationobserver), [IHandlerConfigurationObserver](../masstransit/ihandlerconfigurationobserver), [IActivityConfigurationObserver](../masstransit/iactivityconfigurationobserver), [ISendPipelineConfigurator](../masstransit/isendpipelineconfigurator), [IPublishPipelineConfigurator](../masstransit/ipublishpipelineconfigurator), [IBusObserverConnector](../masstransit/ibusobserverconnector), [IReceiveObserverConnector](../masstransit/ireceiveobserverconnector), [IConsumeObserverConnector](../masstransit/iconsumeobserverconnector), [ISendObserverConnector](../masstransit/isendobserverconnector), [IPublishObserverConnector](../masstransit/ipublishobserverconnector)

## Properties

### **MessageTopology**

```csharp
public abstract IMessageTopologyConfigurator MessageTopology { get; }
```

#### Property Value

[IMessageTopologyConfigurator](../masstransit-configuration/imessagetopologyconfigurator)<br/>

### **ConsumeTopology**

```csharp
public abstract IConsumeTopologyConfigurator ConsumeTopology { get; }
```

#### Property Value

[IConsumeTopologyConfigurator](../masstransit/iconsumetopologyconfigurator)<br/>

### **SendTopology**

```csharp
public abstract ISendTopologyConfigurator SendTopology { get; }
```

#### Property Value

[ISendTopologyConfigurator](../masstransit/isendtopologyconfigurator)<br/>

### **PublishTopology**

```csharp
public abstract IPublishTopologyConfigurator PublishTopology { get; }
```

#### Property Value

[IPublishTopologyConfigurator](../masstransit/ipublishtopologyconfigurator)<br/>

### **DeployTopologyOnly**

Set to true if the topology should be deployed only

```csharp
public abstract bool DeployTopologyOnly { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **DeployPublishTopology**

Deploys defined Publish message types to the broker at startup

```csharp
public abstract bool DeployPublishTopology { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **PrefetchCount**

Specify the number of messages to prefetch from the message broker

```csharp
public abstract int PrefetchCount { set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The limit

### **ConcurrentMessageLimit**

Specify the number of concurrent messages that can be consumed (separate from prefetch count)

```csharp
public abstract Nullable<int> ConcurrentMessageLimit { set; }
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

### **Message\<T\>(Action\<IMessageTopologyConfigurator\<T\>\>)**

Configure the message topology for the message type (global across all bus instances of the same transport type)

```csharp
void Message<T>(Action<IMessageTopologyConfigurator<T>> configureTopology)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configureTopology` [Action\<IMessageTopologyConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Send\<T\>(Action\<IMessageSendTopologyConfigurator\<T\>\>)**

Configure the send topology of the message type

```csharp
void Send<T>(Action<IMessageSendTopologyConfigurator<T>> configureTopology)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configureTopology` [Action\<IMessageSendTopologyConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Publish\<T\>(Action\<IMessagePublishTopologyConfigurator\<T\>\>)**

Configure the send topology of the message type

```csharp
void Publish<T>(Action<IMessagePublishTopologyConfigurator<T>> configureTopology)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configureTopology` [Action\<IMessagePublishTopologyConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

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
