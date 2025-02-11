---

title: ExcludeFromTopologyAttribute

---

# ExcludeFromTopologyAttribute

Namespace: MassTransit

When added to a message type (class, record, or interface), prevents
 MassTransit from creating an exchange or topic on the broker for the message
 type when it is an inherited type (such as IMessage, IEvent, etc.).

```csharp
public class ExcludeFromTopologyAttribute : Attribute
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Attribute](https://learn.microsoft.com/en-us/dotnet/api/system.attribute) → [ExcludeFromTopologyAttribute](../masstransit/excludefromtopologyattribute)

## Properties

### **TypeId**

```csharp
public object TypeId { get; }
```

#### Property Value

[Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

## Constructors

### **ExcludeFromTopologyAttribute()**

```csharp
public ExcludeFromTopologyAttribute()
```
