---

title: IConsumePipeConfigurator

---

# IConsumePipeConfigurator

Namespace: MassTransit

```csharp
public interface IConsumePipeConfigurator : IPipeConfigurator<ConsumeContext>, IConsumerConfigurationObserverConnector, ISagaConfigurationObserverConnector, IHandlerConfigurationObserverConnector, IActivityConfigurationObserverConnector, IConsumerConfigurationObserver, ISagaConfigurationObserver, IHandlerConfigurationObserver, IActivityConfigurationObserver
```

Implements [IPipeConfigurator\<ConsumeContext\>](../masstransit/ipipeconfigurator-1), [IConsumerConfigurationObserverConnector](../masstransit/iconsumerconfigurationobserverconnector), [ISagaConfigurationObserverConnector](../masstransit/isagaconfigurationobserverconnector), [IHandlerConfigurationObserverConnector](../masstransit/ihandlerconfigurationobserverconnector), [IActivityConfigurationObserverConnector](../masstransit/iactivityconfigurationobserverconnector), [IConsumerConfigurationObserver](../masstransit/iconsumerconfigurationobserver), [ISagaConfigurationObserver](../masstransit/isagaconfigurationobserver), [IHandlerConfigurationObserver](../masstransit/ihandlerconfigurationobserver), [IActivityConfigurationObserver](../masstransit/iactivityconfigurationobserver)

## Properties

### **AutoStart**

If set to false, the transport will only be started when a connection is made to the consume pipe.

```csharp
public abstract bool AutoStart { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Methods

### **AddPipeSpecification\<T\>(IPipeSpecification\<ConsumeContext\<T\>\>)**

Adds a type-specific pipe specification to the consume pipe

```csharp
void AddPipeSpecification<T>(IPipeSpecification<ConsumeContext<T>> specification)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`specification` [IPipeSpecification\<ConsumeContext\<T\>\>](../masstransit-configuration/ipipespecification-1)<br/>

### **AddPrePipeSpecification(IPipeSpecification\<ConsumeContext\>)**

Adds a pipe specification prior to the message type router so that a single
 instance is used for all message types

```csharp
void AddPrePipeSpecification(IPipeSpecification<ConsumeContext> specification)
```

#### Parameters

`specification` [IPipeSpecification\<ConsumeContext\>](../masstransit-configuration/ipipespecification-1)<br/>
