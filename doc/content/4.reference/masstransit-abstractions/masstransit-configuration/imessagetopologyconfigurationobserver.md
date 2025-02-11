---

title: IMessageTopologyConfigurationObserver

---

# IMessageTopologyConfigurationObserver

Namespace: MassTransit.Configuration

Observes the configuration of message-specific topology

```csharp
public interface IMessageTopologyConfigurationObserver
```

## Methods

### **MessageTopologyCreated\<T\>(IMessageTopologyConfigurator\<T\>)**

```csharp
void MessageTopologyCreated<T>(IMessageTopologyConfigurator<T> configuration)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configuration` [IMessageTopologyConfigurator\<T\>](../masstransit-configuration/imessagetopologyconfigurator-1)<br/>
