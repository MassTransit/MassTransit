---

title: SqlBusFactoryConfigurator

---

# SqlBusFactoryConfigurator

Namespace: MassTransit.SqlTransport.Configuration

```csharp
public class SqlBusFactoryConfigurator : BusFactoryConfigurator, IConsumePipeConfigurator, IPipeConfigurator<ConsumeContext>, IConsumerConfigurationObserverConnector, ISagaConfigurationObserverConnector, IHandlerConfigurationObserverConnector, IActivityConfigurationObserverConnector, IConsumerConfigurationObserver, ISagaConfigurationObserver, IHandlerConfigurationObserver, IActivityConfigurationObserver, ISendPipelineConfigurator, IPublishPipelineConfigurator, IEndpointConfigurationObserverConnector, IBusObserverConnector, IReceiveObserverConnector, IConsumeObserverConnector, ISendObserverConnector, IPublishObserverConnector, ISqlBusFactoryConfigurator, IBusFactoryConfigurator<ISqlReceiveEndpointConfigurator>, IBusFactoryConfigurator, IReceiveConfigurator, IReceiveConfigurator<ISqlReceiveEndpointConfigurator>, ISqlQueueEndpointConfigurator, ISqlQueueConfigurator, IBusFactory, ISpecification
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BusFactoryConfigurator](../masstransit-configuration/busfactoryconfigurator) → [SqlBusFactoryConfigurator](../masstransit-sqltransport-configuration/sqlbusfactoryconfigurator)<br/>
Implements [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator), [IPipeConfigurator\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserverconnector), [ISagaConfigurationObserverConnector](../../masstransit-abstractions/masstransit/isagaconfigurationobserverconnector), [IHandlerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserverconnector), [IActivityConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iactivityconfigurationobserverconnector), [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver), [ISagaConfigurationObserver](../../masstransit-abstractions/masstransit/isagaconfigurationobserver), [IHandlerConfigurationObserver](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserver), [IActivityConfigurationObserver](../../masstransit-abstractions/masstransit/iactivityconfigurationobserver), [ISendPipelineConfigurator](../../masstransit-abstractions/masstransit/isendpipelineconfigurator), [IPublishPipelineConfigurator](../../masstransit-abstractions/masstransit/ipublishpipelineconfigurator), [IEndpointConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iendpointconfigurationobserverconnector), [IBusObserverConnector](../../masstransit-abstractions/masstransit/ibusobserverconnector), [IReceiveObserverConnector](../../masstransit-abstractions/masstransit/ireceiveobserverconnector), [IConsumeObserverConnector](../../masstransit-abstractions/masstransit/iconsumeobserverconnector), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [ISqlBusFactoryConfigurator](../masstransit/isqlbusfactoryconfigurator), [IBusFactoryConfigurator\<ISqlReceiveEndpointConfigurator\>](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator-1), [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator), [IReceiveConfigurator](../../masstransit-abstractions/masstransit/ireceiveconfigurator), [IReceiveConfigurator\<ISqlReceiveEndpointConfigurator\>](../../masstransit-abstractions/masstransit/ireceiveconfigurator-1), [ISqlQueueEndpointConfigurator](../masstransit/isqlqueueendpointconfigurator), [ISqlQueueConfigurator](../masstransit/isqlqueueconfigurator), [IBusFactory](../masstransit/ibusfactory), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **AutoDeleteOnIdle**

```csharp
public Nullable<TimeSpan> AutoDeleteOnIdle { set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **PollingInterval**

```csharp
public TimeSpan PollingInterval { set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **LockDuration**

```csharp
public TimeSpan LockDuration { set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **MaxLockDuration**

```csharp
public TimeSpan MaxLockDuration { set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **MaxDeliveryCount**

```csharp
public Nullable<int> MaxDeliveryCount { set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **PurgeOnStartup**

```csharp
public bool PurgeOnStartup { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **MaintenanceBatchSize**

```csharp
public int MaintenanceBatchSize { set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **DeadLetterExpiredMessages**

```csharp
public bool DeadLetterExpiredMessages { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **SendTopology**

```csharp
public ISqlSendTopologyConfigurator SendTopology { get; }
```

#### Property Value

[ISqlSendTopologyConfigurator](../masstransit/isqlsendtopologyconfigurator)<br/>

### **PublishTopology**

```csharp
public ISqlPublishTopologyConfigurator PublishTopology { get; }
```

#### Property Value

[ISqlPublishTopologyConfigurator](../masstransit/isqlpublishtopologyconfigurator)<br/>

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

### **AutoStart**

```csharp
public bool AutoStart { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **SqlBusFactoryConfigurator(ISqlBusConfiguration)**

```csharp
public SqlBusFactoryConfigurator(ISqlBusConfiguration busConfiguration)
```

#### Parameters

`busConfiguration` [ISqlBusConfiguration](../masstransit-sqltransport-configuration/isqlbusconfiguration)<br/>

## Methods

### **CreateBusEndpointConfiguration(Action\<IReceiveEndpointConfigurator\>)**

```csharp
public IReceiveEndpointConfiguration CreateBusEndpointConfiguration(Action<IReceiveEndpointConfigurator> configure)
```

#### Parameters

`configure` [Action\<IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IReceiveEndpointConfiguration](../masstransit-configuration/ireceiveendpointconfiguration)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Host(SqlHostSettings)**

```csharp
public void Host(SqlHostSettings settings)
```

#### Parameters

`settings` [SqlHostSettings](../masstransit/sqlhostsettings)<br/>

### **Send\<T\>(Action\<ISqlMessageSendTopologyConfigurator\<T\>\>)**

```csharp
public void Send<T>(Action<ISqlMessageSendTopologyConfigurator<T>> configureTopology)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configureTopology` [Action\<ISqlMessageSendTopologyConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Publish\<T\>(Action\<ISqlMessagePublishTopologyConfigurator\<T\>\>)**

```csharp
public void Publish<T>(Action<ISqlMessagePublishTopologyConfigurator<T>> configureTopology)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configureTopology` [Action\<ISqlMessagePublishTopologyConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Publish(Type, Action\<ISqlMessagePublishTopologyConfigurator\>)**

```csharp
public void Publish(Type messageType, Action<ISqlMessagePublishTopologyConfigurator> configure)
```

#### Parameters

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`configure` [Action\<ISqlMessagePublishTopologyConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **OverrideDefaultBusEndpointQueueName(String)**

```csharp
public void OverrideDefaultBusEndpointQueueName(string queueName)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ReceiveEndpoint(IEndpointDefinition, IEndpointNameFormatter, Action\<ISqlReceiveEndpointConfigurator\>)**

```csharp
public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter, Action<ISqlReceiveEndpointConfigurator> configureEndpoint)
```

#### Parameters

`definition` [IEndpointDefinition](../../masstransit-abstractions/masstransit/iendpointdefinition)<br/>

`endpointNameFormatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

`configureEndpoint` [Action\<ISqlReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ReceiveEndpoint(IEndpointDefinition, IEndpointNameFormatter, Action\<IReceiveEndpointConfigurator\>)**

```csharp
public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter, Action<IReceiveEndpointConfigurator> configureEndpoint)
```

#### Parameters

`definition` [IEndpointDefinition](../../masstransit-abstractions/masstransit/iendpointdefinition)<br/>

`endpointNameFormatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

`configureEndpoint` [Action\<IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ReceiveEndpoint(String, Action\<ISqlReceiveEndpointConfigurator\>)**

```csharp
public void ReceiveEndpoint(string queueName, Action<ISqlReceiveEndpointConfigurator> configureEndpoint)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`configureEndpoint` [Action\<ISqlReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ReceiveEndpoint(String, Action\<IReceiveEndpointConfigurator\>)**

```csharp
public void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`configureEndpoint` [Action\<IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
