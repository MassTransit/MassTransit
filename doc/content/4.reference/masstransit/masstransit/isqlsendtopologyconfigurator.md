---

title: ISqlSendTopologyConfigurator

---

# ISqlSendTopologyConfigurator

Namespace: MassTransit

```csharp
public interface ISqlSendTopologyConfigurator : ISendTopologyConfigurator, ISendTopology, ISendTopologyConfigurationObserverConnector, ISpecification, ISqlSendTopology
```

Implements [ISendTopologyConfigurator](../../masstransit-abstractions/masstransit/isendtopologyconfigurator), [ISendTopology](../../masstransit-abstractions/masstransit/isendtopology), [ISendTopologyConfigurationObserverConnector](../../masstransit-abstractions/masstransit-configuration/isendtopologyconfigurationobserverconnector), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [ISqlSendTopology](../masstransit/isqlsendtopology)

## Properties

### **ConfigureErrorSettings**

```csharp
public abstract Action<ISqlQueueConfigurator> ConfigureErrorSettings { set; }
```

#### Property Value

[Action\<ISqlQueueConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ConfigureDeadLetterSettings**

```csharp
public abstract Action<ISqlQueueConfigurator> ConfigureDeadLetterSettings { set; }
```

#### Property Value

[Action\<ISqlQueueConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
