---

title: ISendPipeConfiguration

---

# ISendPipeConfiguration

Namespace: MassTransit.Configuration

```csharp
public interface ISendPipeConfiguration
```

## Properties

### **Specification**

```csharp
public abstract ISendPipeSpecification Specification { get; }
```

#### Property Value

[ISendPipeSpecification](../../masstransit-abstractions/masstransit-configuration/isendpipespecification)<br/>

### **Configurator**

```csharp
public abstract ISendPipeConfigurator Configurator { get; }
```

#### Property Value

[ISendPipeConfigurator](../../masstransit-abstractions/masstransit/isendpipeconfigurator)<br/>

## Methods

### **CreatePipe()**

```csharp
ISendPipe CreatePipe()
```

#### Returns

[ISendPipe](../../masstransit-abstractions/masstransit-transports/isendpipe)<br/>
