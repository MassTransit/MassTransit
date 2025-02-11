---

title: PublishPipeConfiguration

---

# PublishPipeConfiguration

Namespace: MassTransit.Configuration

```csharp
public class PublishPipeConfiguration : IPublishPipeConfiguration
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PublishPipeConfiguration](../masstransit-configuration/publishpipeconfiguration)<br/>
Implements [IPublishPipeConfiguration](../masstransit-configuration/ipublishpipeconfiguration)

## Properties

### **Specification**

```csharp
public IPublishPipeSpecification Specification { get; }
```

#### Property Value

[IPublishPipeSpecification](../../masstransit-abstractions/masstransit-configuration/ipublishpipespecification)<br/>

### **Configurator**

```csharp
public IPublishPipeConfigurator Configurator { get; }
```

#### Property Value

[IPublishPipeConfigurator](../../masstransit-abstractions/masstransit/ipublishpipeconfigurator)<br/>

## Constructors

### **PublishPipeConfiguration(IPublishTopology)**

```csharp
public PublishPipeConfiguration(IPublishTopology publishTopology)
```

#### Parameters

`publishTopology` [IPublishTopology](../../masstransit-abstractions/masstransit/ipublishtopology)<br/>

### **PublishPipeConfiguration(IPublishPipeSpecification)**

```csharp
public PublishPipeConfiguration(IPublishPipeSpecification parentSpecification)
```

#### Parameters

`parentSpecification` [IPublishPipeSpecification](../../masstransit-abstractions/masstransit-configuration/ipublishpipespecification)<br/>

## Methods

### **CreatePipe()**

```csharp
public IPublishPipe CreatePipe()
```

#### Returns

[IPublishPipe](../../masstransit-abstractions/masstransit-transports/ipublishpipe)<br/>
