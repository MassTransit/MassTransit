---

title: JobServiceConsumerConfigurationObserver

---

# JobServiceConsumerConfigurationObserver

Namespace: MassTransit.Configuration

```csharp
public class JobServiceConsumerConfigurationObserver : IConsumerConfigurationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobServiceConsumerConfigurationObserver](../masstransit-configuration/jobserviceconsumerconfigurationobserver)<br/>
Implements [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver)

## Constructors

### **JobServiceConsumerConfigurationObserver(IReceiveEndpointConfigurator, JobServiceSettings, Action\<IReceiveEndpointConfigurator\>)**

```csharp
public JobServiceConsumerConfigurationObserver(IReceiveEndpointConfigurator configurator, JobServiceSettings settings, Action<IReceiveEndpointConfigurator> configureEndpoint)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`settings` [JobServiceSettings](../masstransit-jobservice/jobservicesettings)<br/>

`configureEndpoint` [Action\<IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

## Methods

### **ConsumerConfigured\<T\>(IConsumerConfigurator\<T\>)**

```csharp
public void ConsumerConfigured<T>(IConsumerConfigurator<T> configurator)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IConsumerConfigurator\<T\>](../../masstransit-abstractions/masstransit/iconsumerconfigurator-1)<br/>

### **ConsumerMessageConfigured\<T, TMessage\>(IConsumerMessageConfigurator\<T, TMessage\>)**

```csharp
public void ConsumerMessageConfigured<T, TMessage>(IConsumerMessageConfigurator<T, TMessage> configurator)
```

#### Type Parameters

`T`<br/>

`TMessage`<br/>

#### Parameters

`configurator` [IConsumerMessageConfigurator\<T, TMessage\>](../../masstransit-abstractions/masstransit/iconsumermessageconfigurator-2)<br/>
