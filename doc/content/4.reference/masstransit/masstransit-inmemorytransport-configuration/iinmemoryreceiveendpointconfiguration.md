---

title: IInMemoryReceiveEndpointConfiguration

---

# IInMemoryReceiveEndpointConfiguration

Namespace: MassTransit.InMemoryTransport.Configuration

```csharp
public interface IInMemoryReceiveEndpointConfiguration : IReceiveEndpointConfiguration, IEndpointConfiguration, IConsumePipeConfigurator, IPipeConfigurator<ConsumeContext>, IConsumerConfigurationObserverConnector, ISagaConfigurationObserverConnector, IHandlerConfigurationObserverConnector, IActivityConfigurationObserverConnector, IConsumerConfigurationObserver, ISagaConfigurationObserver, IHandlerConfigurationObserver, IActivityConfigurationObserver, ISendPipelineConfigurator, IPublishPipelineConfigurator, IReceivePipelineConfigurator, ISpecification, IReceiveEndpointObserverConnector, IReceiveEndpointDependentConnector, IInMemoryEndpointConfiguration
```

Implements [IReceiveEndpointConfiguration](../masstransit-configuration/ireceiveendpointconfiguration), [IEndpointConfiguration](../masstransit-configuration/iendpointconfiguration), [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator), [IPipeConfigurator\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserverconnector), [ISagaConfigurationObserverConnector](../../masstransit-abstractions/masstransit/isagaconfigurationobserverconnector), [IHandlerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserverconnector), [IActivityConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iactivityconfigurationobserverconnector), [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver), [ISagaConfigurationObserver](../../masstransit-abstractions/masstransit/isagaconfigurationobserver), [IHandlerConfigurationObserver](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserver), [IActivityConfigurationObserver](../../masstransit-abstractions/masstransit/iactivityconfigurationobserver), [ISendPipelineConfigurator](../../masstransit-abstractions/masstransit/isendpipelineconfigurator), [IPublishPipelineConfigurator](../../masstransit-abstractions/masstransit/ipublishpipelineconfigurator), [IReceivePipelineConfigurator](../../masstransit-abstractions/masstransit/ireceivepipelineconfigurator), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IReceiveEndpointObserverConnector](../../masstransit-abstractions/masstransit/ireceiveendpointobserverconnector), [IReceiveEndpointDependentConnector](../../masstransit-abstractions/masstransit/ireceiveendpointdependentconnector), [IInMemoryEndpointConfiguration](../masstransit-inmemorytransport-configuration/iinmemoryendpointconfiguration)

## Properties

### **Configurator**

```csharp
public abstract IInMemoryReceiveEndpointConfigurator Configurator { get; }
```

#### Property Value

[IInMemoryReceiveEndpointConfigurator](../masstransit/iinmemoryreceiveendpointconfigurator)<br/>

## Methods

### **Build(IHost)**

```csharp
void Build(IHost host)
```

#### Parameters

`host` [IHost](../masstransit-transports/ihost)<br/>
