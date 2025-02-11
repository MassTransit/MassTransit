---

title: ITopologyConfiguration

---

# ITopologyConfiguration

Namespace: MassTransit.Configuration

```csharp
public interface ITopologyConfiguration : ISpecification
```

Implements [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **Message**

```csharp
public abstract IMessageTopologyConfigurator Message { get; }
```

#### Property Value

[IMessageTopologyConfigurator](../../masstransit-abstractions/masstransit-configuration/imessagetopologyconfigurator)<br/>

### **Send**

```csharp
public abstract ISendTopologyConfigurator Send { get; }
```

#### Property Value

[ISendTopologyConfigurator](../../masstransit-abstractions/masstransit/isendtopologyconfigurator)<br/>

### **Publish**

```csharp
public abstract IPublishTopologyConfigurator Publish { get; }
```

#### Property Value

[IPublishTopologyConfigurator](../../masstransit-abstractions/masstransit/ipublishtopologyconfigurator)<br/>

### **Consume**

```csharp
public abstract IConsumeTopologyConfigurator Consume { get; }
```

#### Property Value

[IConsumeTopologyConfigurator](../../masstransit-abstractions/masstransit/iconsumetopologyconfigurator)<br/>
