---

title: IInMemoryReceiveEndpointConfigurator

---

# IInMemoryReceiveEndpointConfigurator

Namespace: MassTransit

```csharp
public interface IInMemoryReceiveEndpointConfigurator : IReceiveEndpointConfigurator, IEndpointConfigurator, IConsumePipeConfigurator, IPipeConfigurator<ConsumeContext>, IConsumerConfigurationObserverConnector, ISagaConfigurationObserverConnector, IHandlerConfigurationObserverConnector, IActivityConfigurationObserverConnector, IConsumerConfigurationObserver, ISagaConfigurationObserver, IHandlerConfigurationObserver, IActivityConfigurationObserver, ISendPipelineConfigurator, IPublishPipelineConfigurator, IReceivePipelineConfigurator, IReceiveEndpointObserverConnector, IReceiveEndpointDependencyConnector, IReceiveEndpointDependentConnector
```

Implements [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator), [IEndpointConfigurator](../../masstransit-abstractions/masstransit/iendpointconfigurator), [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator), [IPipeConfigurator\<ConsumeContext\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserverconnector), [ISagaConfigurationObserverConnector](../../masstransit-abstractions/masstransit/isagaconfigurationobserverconnector), [IHandlerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserverconnector), [IActivityConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iactivityconfigurationobserverconnector), [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver), [ISagaConfigurationObserver](../../masstransit-abstractions/masstransit/isagaconfigurationobserver), [IHandlerConfigurationObserver](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserver), [IActivityConfigurationObserver](../../masstransit-abstractions/masstransit/iactivityconfigurationobserver), [ISendPipelineConfigurator](../../masstransit-abstractions/masstransit/isendpipelineconfigurator), [IPublishPipelineConfigurator](../../masstransit-abstractions/masstransit/ipublishpipelineconfigurator), [IReceivePipelineConfigurator](../../masstransit-abstractions/masstransit/ireceivepipelineconfigurator), [IReceiveEndpointObserverConnector](../../masstransit-abstractions/masstransit/ireceiveendpointobserverconnector), [IReceiveEndpointDependencyConnector](../../masstransit-abstractions/masstransit/ireceiveendpointdependencyconnector), [IReceiveEndpointDependentConnector](../../masstransit-abstractions/masstransit/ireceiveendpointdependentconnector)

## Methods

### **Bind(String, ExchangeType, String)**

Bind an exchange to the receive endpoint queue

```csharp
void Bind(string exchangeName, ExchangeType exchangeType, string routingKey)
```

#### Parameters

`exchangeName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The exchange name (not case-sensitive)

`exchangeType` [ExchangeType](../masstransit-transports-fabric/exchangetype)<br/>
The exchange type

`routingKey` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
Only valid for direct/topic exchanges

### **Bind\<T\>(ExchangeType, String)**

Bind an exchange to the receive endpoint queue

```csharp
void Bind<T>(ExchangeType exchangeType, string routingKey)
```

#### Type Parameters

`T`<br/>

#### Parameters

`exchangeType` [ExchangeType](../masstransit-transports-fabric/exchangetype)<br/>
The exchange type

`routingKey` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
Only valid for direct/topic exchanges
