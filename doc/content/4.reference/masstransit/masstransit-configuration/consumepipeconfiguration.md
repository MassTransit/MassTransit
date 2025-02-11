---

title: ConsumePipeConfiguration

---

# ConsumePipeConfiguration

Namespace: MassTransit.Configuration

```csharp
public class ConsumePipeConfiguration : IConsumePipeConfiguration
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumePipeConfiguration](../masstransit-configuration/consumepipeconfiguration)<br/>
Implements [IConsumePipeConfiguration](../masstransit-configuration/iconsumepipeconfiguration)

## Properties

### **Specification**

```csharp
public IConsumePipeSpecification Specification { get; }
```

#### Property Value

[IConsumePipeSpecification](../../masstransit-abstractions/masstransit-configuration/iconsumepipespecification)<br/>

### **Configurator**

```csharp
public IConsumePipeConfigurator Configurator { get; }
```

#### Property Value

[IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>

## Constructors

### **ConsumePipeConfiguration(IConsumeTopology)**

```csharp
public ConsumePipeConfiguration(IConsumeTopology consumeTopology)
```

#### Parameters

`consumeTopology` [IConsumeTopology](../../masstransit-abstractions/masstransit/iconsumetopology)<br/>

### **ConsumePipeConfiguration(IConsumePipeSpecification)**

```csharp
public ConsumePipeConfiguration(IConsumePipeSpecification parentSpecification)
```

#### Parameters

`parentSpecification` [IConsumePipeSpecification](../../masstransit-abstractions/masstransit-configuration/iconsumepipespecification)<br/>
