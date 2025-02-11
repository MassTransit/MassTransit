---

title: IInMemoryBusFactoryConfigurator

---

# IInMemoryBusFactoryConfigurator

Namespace: MassTransit

```csharp
public interface IInMemoryBusFactoryConfigurator : IBusFactoryConfigurator<IInMemoryReceiveEndpointConfigurator>, IBusFactoryConfigurator, IReceiveConfigurator, IEndpointConfigurationObserverConnector, IConsumePipeConfigurator, IPipeConfigurator<ConsumeContext>, IConsumerConfigurationObserverConnector, ISagaConfigurationObserverConnector, IHandlerConfigurationObserverConnector, IActivityConfigurationObserverConnector, IConsumerConfigurationObserver, ISagaConfigurationObserver, IHandlerConfigurationObserver, IActivityConfigurationObserver, ISendPipelineConfigurator, IPublishPipelineConfigurator, IBusObserverConnector, IReceiveObserverConnector, IConsumeObserverConnector, ISendObserverConnector, IPublishObserverConnector, IReceiveConfigurator<IInMemoryReceiveEndpointConfigurator>
```

Implements [IBusFactoryConfigurator\<IInMemoryReceiveEndpointConfigurator\>](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator-1), [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator), [IReceiveConfigurator](../../masstransit-abstractions/masstransit/ireceiveconfigurator), [IEndpointConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iendpointconfigurationobserverconnector), [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator), [IPipeConfigurator\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserverconnector), [ISagaConfigurationObserverConnector](../../masstransit-abstractions/masstransit/isagaconfigurationobserverconnector), [IHandlerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserverconnector), [IActivityConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iactivityconfigurationobserverconnector), [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver), [ISagaConfigurationObserver](../../masstransit-abstractions/masstransit/isagaconfigurationobserver), [IHandlerConfigurationObserver](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserver), [IActivityConfigurationObserver](../../masstransit-abstractions/masstransit/iactivityconfigurationobserver), [ISendPipelineConfigurator](../../masstransit-abstractions/masstransit/isendpipelineconfigurator), [IPublishPipelineConfigurator](../../masstransit-abstractions/masstransit/ipublishpipelineconfigurator), [IBusObserverConnector](../../masstransit-abstractions/masstransit/ibusobserverconnector), [IReceiveObserverConnector](../../masstransit-abstractions/masstransit/ireceiveobserverconnector), [IConsumeObserverConnector](../../masstransit-abstractions/masstransit/iconsumeobserverconnector), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [IReceiveConfigurator\<IInMemoryReceiveEndpointConfigurator\>](../../masstransit-abstractions/masstransit/ireceiveconfigurator-1)

## Properties

### **PublishTopology**

```csharp
public abstract IInMemoryPublishTopologyConfigurator PublishTopology { get; }
```

#### Property Value

[IInMemoryPublishTopologyConfigurator](../masstransit/iinmemorypublishtopologyconfigurator)<br/>

## Methods

### **Publish\<T\>(Action\<IInMemoryMessagePublishTopologyConfigurator\<T\>\>)**

Configure the send topology of the message type

```csharp
void Publish<T>(Action<IInMemoryMessagePublishTopologyConfigurator<T>> configureTopology)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configureTopology` [Action\<IInMemoryMessagePublishTopologyConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Publish(Type, Action\<IInMemoryMessagePublishTopologyConfigurator\>)**

```csharp
void Publish(Type messageType, Action<IInMemoryMessagePublishTopologyConfigurator> configure)
```

#### Parameters

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`configure` [Action\<IInMemoryMessagePublishTopologyConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Host(Action\<IInMemoryHostConfigurator\>)**

Configure the base address for the host

```csharp
void Host(Action<IInMemoryHostConfigurator> configure)
```

#### Parameters

`configure` [Action\<IInMemoryHostConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Host(Uri, Action\<IInMemoryHostConfigurator\>)**

Configure the base address for the host

```csharp
void Host(Uri baseAddress, Action<IInMemoryHostConfigurator> configure)
```

#### Parameters

`baseAddress` Uri<br/>
The base address for the in-memory host

`configure` [Action\<IInMemoryHostConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Host(String, Action\<IInMemoryHostConfigurator\>)**

Configure the virtual host, to differentiate in-memory bus instances

```csharp
void Host(string virtualHost, Action<IInMemoryHostConfigurator> configure)
```

#### Parameters

`virtualHost` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The virtual host path

`configure` [Action\<IInMemoryHostConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
