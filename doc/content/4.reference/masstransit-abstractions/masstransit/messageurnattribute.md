---

title: MessageUrnAttribute

---

# MessageUrnAttribute

Namespace: MassTransit

Specify the message type name for this message type

```csharp
public class MessageUrnAttribute : Attribute
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Attribute](https://learn.microsoft.com/en-us/dotnet/api/system.attribute) → [MessageUrnAttribute](../masstransit/messageurnattribute)

## Properties

### **Urn**

```csharp
public Uri Urn { get; }
```

#### Property Value

Uri<br/>

### **TypeId**

```csharp
public object TypeId { get; }
```

#### Property Value

[Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

## Constructors

### **MessageUrnAttribute(String, Boolean)**



```csharp
public MessageUrnAttribute(string urn, bool useDefaultPrefix)
```

#### Parameters

`urn` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The urn value to use for this message type.

`useDefaultPrefix` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
Prefixes with default scheme and namespace if true.
