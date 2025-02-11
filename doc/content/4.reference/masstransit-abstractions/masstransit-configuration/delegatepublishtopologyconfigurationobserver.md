---

title: DelegatePublishTopologyConfigurationObserver

---

# DelegatePublishTopologyConfigurationObserver

Namespace: MassTransit.Configuration

```csharp
public class DelegatePublishTopologyConfigurationObserver : IPublishTopologyConfigurationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DelegatePublishTopologyConfigurationObserver](../masstransit-configuration/delegatepublishtopologyconfigurationobserver)<br/>
Implements [IPublishTopologyConfigurationObserver](../masstransit-configuration/ipublishtopologyconfigurationobserver)

## Constructors

### **DelegatePublishTopologyConfigurationObserver(IPublishTopologyConfigurator)**

```csharp
public DelegatePublishTopologyConfigurationObserver(IPublishTopologyConfigurator publishTopology)
```

#### Parameters

`publishTopology` [IPublishTopologyConfigurator](../masstransit/ipublishtopologyconfigurator)<br/>

## Methods

### **MessageTopologyCreated\<T\>(IMessagePublishTopologyConfigurator\<T\>)**

```csharp
public void MessageTopologyCreated<T>(IMessagePublishTopologyConfigurator<T> configurator)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IMessagePublishTopologyConfigurator\<T\>](../masstransit/imessagepublishtopologyconfigurator-1)<br/>
