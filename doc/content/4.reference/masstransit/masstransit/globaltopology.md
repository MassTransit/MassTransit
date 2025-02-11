---

title: GlobalTopology

---

# GlobalTopology

Namespace: MassTransit

This represents the global topology configuration, which is delegated to by
 all topology instances, unless for some radical reason a bus is configured
 without any topology delegation.
 YES, I hate globals, but they are serving a purpose in that these are really
 just defining the default behavior of message types, rather than actually
 behaving like the nasty evil global variables.

```csharp
public class GlobalTopology : IGlobalTopology
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [GlobalTopology](../masstransit/globaltopology)<br/>
Implements [IGlobalTopology](../masstransit/iglobaltopology)

## Properties

### **Send**

```csharp
public static ISendTopologyConfigurator Send { get; }
```

#### Property Value

[ISendTopologyConfigurator](../../masstransit-abstractions/masstransit/isendtopologyconfigurator)<br/>

### **Publish**

```csharp
public static IPublishTopologyConfigurator Publish { get; }
```

#### Property Value

[IPublishTopologyConfigurator](../../masstransit-abstractions/masstransit/ipublishtopologyconfigurator)<br/>

## Methods

### **MarkMessageTypeNotConsumable(Type)**

Mark the specified message type such that it will not be configured by the consume topology,
 and therefore not bound/subscribed on the message broker.

```csharp
public static void MarkMessageTypeNotConsumable(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **IsConsumableMessageType(Type)**

```csharp
public static bool IsConsumableMessageType(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **SeparatePublishFromSend()**

Call before configuring any topology, so that publish is handled separately
 from send. Note, this can cause some really bad things to happen with internal
 types so use with caution...

```csharp
public static void SeparatePublishFromSend()
```
