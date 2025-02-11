---

title: SendPipeConfiguration

---

# SendPipeConfiguration

Namespace: MassTransit.Configuration

```csharp
public class SendPipeConfiguration : ISendPipeConfiguration
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SendPipeConfiguration](../masstransit-configuration/sendpipeconfiguration)<br/>
Implements [ISendPipeConfiguration](../masstransit-configuration/isendpipeconfiguration)

## Properties

### **Specification**

```csharp
public ISendPipeSpecification Specification { get; }
```

#### Property Value

[ISendPipeSpecification](../../masstransit-abstractions/masstransit-configuration/isendpipespecification)<br/>

### **Configurator**

```csharp
public ISendPipeConfigurator Configurator { get; }
```

#### Property Value

[ISendPipeConfigurator](../../masstransit-abstractions/masstransit/isendpipeconfigurator)<br/>

## Constructors

### **SendPipeConfiguration(ISendTopology)**

```csharp
public SendPipeConfiguration(ISendTopology sendTopology)
```

#### Parameters

`sendTopology` [ISendTopology](../../masstransit-abstractions/masstransit/isendtopology)<br/>

### **SendPipeConfiguration(ISendPipeSpecification)**

```csharp
public SendPipeConfiguration(ISendPipeSpecification parentSpecification)
```

#### Parameters

`parentSpecification` [ISendPipeSpecification](../../masstransit-abstractions/masstransit-configuration/isendpipespecification)<br/>

## Methods

### **CreatePipe()**

```csharp
public ISendPipe CreatePipe()
```

#### Returns

[ISendPipe](../../masstransit-abstractions/masstransit-transports/isendpipe)<br/>
