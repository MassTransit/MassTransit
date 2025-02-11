---

title: IPublishPipeConfiguration

---

# IPublishPipeConfiguration

Namespace: MassTransit.Configuration

```csharp
public interface IPublishPipeConfiguration
```

## Properties

### **Specification**

```csharp
public abstract IPublishPipeSpecification Specification { get; }
```

#### Property Value

[IPublishPipeSpecification](../../masstransit-abstractions/masstransit-configuration/ipublishpipespecification)<br/>

### **Configurator**

```csharp
public abstract IPublishPipeConfigurator Configurator { get; }
```

#### Property Value

[IPublishPipeConfigurator](../../masstransit-abstractions/masstransit/ipublishpipeconfigurator)<br/>

## Methods

### **CreatePipe()**

```csharp
IPublishPipe CreatePipe()
```

#### Returns

[IPublishPipe](../../masstransit-abstractions/masstransit-transports/ipublishpipe)<br/>
