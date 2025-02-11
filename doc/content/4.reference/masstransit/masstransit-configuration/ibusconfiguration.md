---

title: IBusConfiguration

---

# IBusConfiguration

Namespace: MassTransit.Configuration

The configuration of a bus

```csharp
public interface IBusConfiguration : IEndpointConfiguration, IConsumePipeConfigurator, IPipeConfigurator<ConsumeContext>, IConsumerConfigurationObserverConnector, ISagaConfigurationObserverConnector, IHandlerConfigurationObserverConnector, IActivityConfigurationObserverConnector, IConsumerConfigurationObserver, ISagaConfigurationObserver, IHandlerConfigurationObserver, IActivityConfigurationObserver, ISendPipelineConfigurator, IPublishPipelineConfigurator, IReceivePipelineConfigurator, ISpecification, IBusObserverConnector, IEndpointConfigurationObserverConnector
```

Implements [IEndpointConfiguration](../masstransit-configuration/iendpointconfiguration), [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator), [IPipeConfigurator\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserverconnector), [ISagaConfigurationObserverConnector](../../masstransit-abstractions/masstransit/isagaconfigurationobserverconnector), [IHandlerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserverconnector), [IActivityConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iactivityconfigurationobserverconnector), [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver), [ISagaConfigurationObserver](../../masstransit-abstractions/masstransit/isagaconfigurationobserver), [IHandlerConfigurationObserver](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserver), [IActivityConfigurationObserver](../../masstransit-abstractions/masstransit/iactivityconfigurationobserver), [ISendPipelineConfigurator](../../masstransit-abstractions/masstransit/isendpipelineconfigurator), [IPublishPipelineConfigurator](../../masstransit-abstractions/masstransit/ipublishpipelineconfigurator), [IReceivePipelineConfigurator](../../masstransit-abstractions/masstransit/ireceivepipelineconfigurator), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IBusObserverConnector](../../masstransit-abstractions/masstransit/ibusobserverconnector), [IEndpointConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iendpointconfigurationobserverconnector)

## Properties

### **HostConfiguration**

```csharp
public abstract IHostConfiguration HostConfiguration { get; }
```

#### Property Value

[IHostConfiguration](../masstransit-configuration/ihostconfiguration)<br/>

### **BusEndpointConfiguration**

```csharp
public abstract IEndpointConfiguration BusEndpointConfiguration { get; }
```

#### Property Value

[IEndpointConfiguration](../masstransit-configuration/iendpointconfiguration)<br/>

### **BusObservers**

```csharp
public abstract IBusObserver BusObservers { get; }
```

#### Property Value

[IBusObserver](../../masstransit-abstractions/masstransit/ibusobserver)<br/>
