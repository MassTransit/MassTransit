---

title: IInMemoryTopologyConfiguration

---

# IInMemoryTopologyConfiguration

Namespace: MassTransit.InMemoryTransport.Configuration

```csharp
public interface IInMemoryTopologyConfiguration : ITopologyConfiguration, ISpecification
```

Implements [ITopologyConfiguration](../masstransit-configuration/itopologyconfiguration), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **Publish**

```csharp
public abstract IInMemoryPublishTopologyConfigurator Publish { get; }
```

#### Property Value

[IInMemoryPublishTopologyConfigurator](../masstransit/iinmemorypublishtopologyconfigurator)<br/>

### **Consume**

```csharp
public abstract IInMemoryConsumeTopologyConfigurator Consume { get; }
```

#### Property Value

[IInMemoryConsumeTopologyConfigurator](../masstransit/iinmemoryconsumetopologyconfigurator)<br/>
