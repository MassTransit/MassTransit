---

title: ISqlBusFactoryConfigurator

---

# ISqlBusFactoryConfigurator

Namespace: MassTransit

```csharp
public interface ISqlBusFactoryConfigurator : IBusFactoryConfigurator<ISqlReceiveEndpointConfigurator>, IBusFactoryConfigurator, IReceiveConfigurator, IEndpointConfigurationObserverConnector, IConsumePipeConfigurator, IPipeConfigurator<ConsumeContext>, IConsumerConfigurationObserverConnector, ISagaConfigurationObserverConnector, IHandlerConfigurationObserverConnector, IActivityConfigurationObserverConnector, IConsumerConfigurationObserver, ISagaConfigurationObserver, IHandlerConfigurationObserver, IActivityConfigurationObserver, ISendPipelineConfigurator, IPublishPipelineConfigurator, IBusObserverConnector, IReceiveObserverConnector, IConsumeObserverConnector, ISendObserverConnector, IPublishObserverConnector, IReceiveConfigurator<ISqlReceiveEndpointConfigurator>, ISqlQueueEndpointConfigurator, ISqlQueueConfigurator
```

Implements [IBusFactoryConfigurator\<ISqlReceiveEndpointConfigurator\>](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator-1), [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator), [IReceiveConfigurator](../../masstransit-abstractions/masstransit/ireceiveconfigurator), [IEndpointConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iendpointconfigurationobserverconnector), [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator), [IPipeConfigurator\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserverconnector), [ISagaConfigurationObserverConnector](../../masstransit-abstractions/masstransit/isagaconfigurationobserverconnector), [IHandlerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserverconnector), [IActivityConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iactivityconfigurationobserverconnector), [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver), [ISagaConfigurationObserver](../../masstransit-abstractions/masstransit/isagaconfigurationobserver), [IHandlerConfigurationObserver](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserver), [IActivityConfigurationObserver](../../masstransit-abstractions/masstransit/iactivityconfigurationobserver), [ISendPipelineConfigurator](../../masstransit-abstractions/masstransit/isendpipelineconfigurator), [IPublishPipelineConfigurator](../../masstransit-abstractions/masstransit/ipublishpipelineconfigurator), [IBusObserverConnector](../../masstransit-abstractions/masstransit/ibusobserverconnector), [IReceiveObserverConnector](../../masstransit-abstractions/masstransit/ireceiveobserverconnector), [IConsumeObserverConnector](../../masstransit-abstractions/masstransit/iconsumeobserverconnector), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [IReceiveConfigurator\<ISqlReceiveEndpointConfigurator\>](../../masstransit-abstractions/masstransit/ireceiveconfigurator-1), [ISqlQueueEndpointConfigurator](../masstransit/isqlqueueendpointconfigurator), [ISqlQueueConfigurator](../masstransit/isqlqueueconfigurator)

## Properties

### **SendTopology**

```csharp
public abstract ISqlSendTopologyConfigurator SendTopology { get; }
```

#### Property Value

[ISqlSendTopologyConfigurator](../masstransit/isqlsendtopologyconfigurator)<br/>

### **PublishTopology**

```csharp
public abstract ISqlPublishTopologyConfigurator PublishTopology { get; }
```

#### Property Value

[ISqlPublishTopologyConfigurator](../masstransit/isqlpublishtopologyconfigurator)<br/>

## Methods

### **Send\<T\>(Action\<ISqlMessageSendTopologyConfigurator\<T\>\>)**

Configure the send topology of the message type

```csharp
void Send<T>(Action<ISqlMessageSendTopologyConfigurator<T>> configureTopology)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configureTopology` [Action\<ISqlMessageSendTopologyConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Publish\<T\>(Action\<ISqlMessagePublishTopologyConfigurator\<T\>\>)**

Configure the send topology of the message type

```csharp
void Publish<T>(Action<ISqlMessagePublishTopologyConfigurator<T>> configureTopology)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configureTopology` [Action\<ISqlMessagePublishTopologyConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Publish(Type, Action\<ISqlMessagePublishTopologyConfigurator\>)**

```csharp
void Publish(Type messageType, Action<ISqlMessagePublishTopologyConfigurator> configure)
```

#### Parameters

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`configure` [Action\<ISqlMessagePublishTopologyConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **OverrideDefaultBusEndpointQueueName(String)**

In most cases, this is not needed and should not be used. However, if for any reason the default bus
 endpoint queue name needs to be changed, this will do it. Do NOT set it to the same name as a receive
 endpoint or you will screw things up.

```csharp
void OverrideDefaultBusEndpointQueueName(string value)
```

#### Parameters

`value` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Host(SqlHostSettings)**

Configure a Host that can be connected. If only one host is specified, it is used as the default
 host for receive endpoints.

```csharp
void Host(SqlHostSettings settings)
```

#### Parameters

`settings` [SqlHostSettings](../masstransit/sqlhostsettings)<br/>
