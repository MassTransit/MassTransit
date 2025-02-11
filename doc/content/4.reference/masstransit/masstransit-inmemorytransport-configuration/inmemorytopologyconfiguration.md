---

title: InMemoryTopologyConfiguration

---

# InMemoryTopologyConfiguration

Namespace: MassTransit.InMemoryTransport.Configuration

```csharp
public class InMemoryTopologyConfiguration : IInMemoryTopologyConfiguration, ITopologyConfiguration, ISpecification
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InMemoryTopologyConfiguration](../masstransit-inmemorytransport-configuration/inmemorytopologyconfiguration)<br/>
Implements [IInMemoryTopologyConfiguration](../masstransit-inmemorytransport-configuration/iinmemorytopologyconfiguration), [ITopologyConfiguration](../masstransit-configuration/itopologyconfiguration), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **InMemoryTopologyConfiguration(IMessageTopologyConfigurator)**

```csharp
public InMemoryTopologyConfiguration(IMessageTopologyConfigurator messageTopology)
```

#### Parameters

`messageTopology` [IMessageTopologyConfigurator](../../masstransit-abstractions/masstransit-configuration/imessagetopologyconfigurator)<br/>

### **InMemoryTopologyConfiguration(IInMemoryTopologyConfiguration)**

```csharp
public InMemoryTopologyConfiguration(IInMemoryTopologyConfiguration topologyConfiguration)
```

#### Parameters

`topologyConfiguration` [IInMemoryTopologyConfiguration](../masstransit-inmemorytransport-configuration/iinmemorytopologyconfiguration)<br/>

## Methods

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
