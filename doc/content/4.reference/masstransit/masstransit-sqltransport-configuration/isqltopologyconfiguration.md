---

title: ISqlTopologyConfiguration

---

# ISqlTopologyConfiguration

Namespace: MassTransit.SqlTransport.Configuration

```csharp
public interface ISqlTopologyConfiguration : ITopologyConfiguration, ISpecification
```

Implements [ITopologyConfiguration](../masstransit-configuration/itopologyconfiguration), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **Publish**

```csharp
public abstract ISqlPublishTopologyConfigurator Publish { get; }
```

#### Property Value

[ISqlPublishTopologyConfigurator](../masstransit/isqlpublishtopologyconfigurator)<br/>

### **Send**

```csharp
public abstract ISqlSendTopologyConfigurator Send { get; }
```

#### Property Value

[ISqlSendTopologyConfigurator](../masstransit/isqlsendtopologyconfigurator)<br/>

### **Consume**

```csharp
public abstract ISqlConsumeTopologyConfigurator Consume { get; }
```

#### Property Value

[ISqlConsumeTopologyConfigurator](../masstransit/isqlconsumetopologyconfigurator)<br/>
