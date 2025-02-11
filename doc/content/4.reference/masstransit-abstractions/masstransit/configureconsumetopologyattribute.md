---

title: ConfigureConsumeTopologyAttribute

---

# ConfigureConsumeTopologyAttribute

Namespace: MassTransit

Specify whether the message type should be used to configure the broker topology for the consumer.
 if configured. Types will this attribute will not have their matching topic/exchange bound to the
 receive endpoint queue.

```csharp
public class ConfigureConsumeTopologyAttribute : Attribute
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Attribute](https://learn.microsoft.com/en-us/dotnet/api/system.attribute) → [ConfigureConsumeTopologyAttribute](../masstransit/configureconsumetopologyattribute)

## Properties

### **ConfigureConsumeTopology**

```csharp
public bool ConfigureConsumeTopology { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TypeId**

```csharp
public object TypeId { get; }
```

#### Property Value

[Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

## Constructors

### **ConfigureConsumeTopologyAttribute()**

```csharp
public ConfigureConsumeTopologyAttribute()
```

### **ConfigureConsumeTopologyAttribute(Boolean)**

```csharp
public ConfigureConsumeTopologyAttribute(bool configureConsumeTopology)
```

#### Parameters

`configureConsumeTopology` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
When false, the consume topology will not be configured
