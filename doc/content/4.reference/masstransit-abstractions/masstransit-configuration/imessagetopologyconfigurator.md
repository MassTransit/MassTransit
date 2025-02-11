---

title: IMessageTopologyConfigurator

---

# IMessageTopologyConfigurator

Namespace: MassTransit.Configuration

```csharp
public interface IMessageTopologyConfigurator : IMessageTopology, IMessageTopologyConfigurationObserverConnector
```

Implements [IMessageTopology](../masstransit/imessagetopology), [IMessageTopologyConfigurationObserverConnector](../masstransit-configuration/imessagetopologyconfigurationobserverconnector)

## Methods

### **SetEntityNameFormatter(IEntityNameFormatter)**

Replace the default entity name formatter

```csharp
void SetEntityNameFormatter(IEntityNameFormatter entityNameFormatter)
```

#### Parameters

`entityNameFormatter` [IEntityNameFormatter](../masstransit/ientitynameformatter)<br/>

### **GetMessageTopology\<T\>()**

```csharp
IMessageTopologyConfigurator<T> GetMessageTopology<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IMessageTopologyConfigurator\<T\>](../masstransit-configuration/imessagetopologyconfigurator-1)<br/>
