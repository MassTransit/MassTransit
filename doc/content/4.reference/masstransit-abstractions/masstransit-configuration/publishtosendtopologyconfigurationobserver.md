---

title: PublishToSendTopologyConfigurationObserver

---

# PublishToSendTopologyConfigurationObserver

Namespace: MassTransit.Configuration

```csharp
public class PublishToSendTopologyConfigurationObserver : IPublishTopologyConfigurationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PublishToSendTopologyConfigurationObserver](../masstransit-configuration/publishtosendtopologyconfigurationobserver)<br/>
Implements [IPublishTopologyConfigurationObserver](../masstransit-configuration/ipublishtopologyconfigurationobserver)

## Constructors

### **PublishToSendTopologyConfigurationObserver(ISendTopology)**

```csharp
public PublishToSendTopologyConfigurationObserver(ISendTopology sendTopology)
```

#### Parameters

`sendTopology` [ISendTopology](../masstransit/isendtopology)<br/>

## Methods

### **MessageTopologyCreated\<T\>(IMessagePublishTopologyConfigurator\<T\>)**

```csharp
public void MessageTopologyCreated<T>(IMessagePublishTopologyConfigurator<T> configurator)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IMessagePublishTopologyConfigurator\<T\>](../masstransit/imessagepublishtopologyconfigurator-1)<br/>
