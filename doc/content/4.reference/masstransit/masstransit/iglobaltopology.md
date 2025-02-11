---

title: IGlobalTopology

---

# IGlobalTopology

Namespace: MassTransit

```csharp
public interface IGlobalTopology
```

## Properties

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

## Methods

### **SeparatePublishFromSend()**

This must be called early, methinks

```csharp
void SeparatePublishFromSend()
```

### **MarkMessageTypeNotConsumable(Type)**

```csharp
void MarkMessageTypeNotConsumable(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **IsConsumableMessageType(Type)**

```csharp
bool IsConsumableMessageType(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
