---

title: DelegateSendTopologyConfigurationObserver

---

# DelegateSendTopologyConfigurationObserver

Namespace: MassTransit.Configuration

```csharp
public class DelegateSendTopologyConfigurationObserver : ISendTopologyConfigurationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DelegateSendTopologyConfigurationObserver](../masstransit-configuration/delegatesendtopologyconfigurationobserver)<br/>
Implements [ISendTopologyConfigurationObserver](../masstransit-configuration/isendtopologyconfigurationobserver)

## Constructors

### **DelegateSendTopologyConfigurationObserver(ISendTopology)**

```csharp
public DelegateSendTopologyConfigurationObserver(ISendTopology sendTopology)
```

#### Parameters

`sendTopology` [ISendTopology](../masstransit/isendtopology)<br/>

## Methods

### **MessageTopologyCreated\<T\>(IMessageSendTopologyConfigurator\<T\>)**

```csharp
public void MessageTopologyCreated<T>(IMessageSendTopologyConfigurator<T> configuration)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configuration` [IMessageSendTopologyConfigurator\<T\>](../masstransit/imessagesendtopologyconfigurator-1)<br/>
