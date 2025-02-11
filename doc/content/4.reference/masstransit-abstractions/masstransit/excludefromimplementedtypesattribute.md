---

title: ExcludeFromImplementedTypesAttribute

---

# ExcludeFromImplementedTypesAttribute

Namespace: MassTransit

Typically added to base messages types, such as IMessage, IEvent, etc.
 so that scoped filters are not created on the message type.

```csharp
public class ExcludeFromImplementedTypesAttribute : Attribute
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Attribute](https://learn.microsoft.com/en-us/dotnet/api/system.attribute) → [ExcludeFromImplementedTypesAttribute](../masstransit/excludefromimplementedtypesattribute)

## Properties

### **TypeId**

```csharp
public object TypeId { get; }
```

#### Property Value

[Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

## Constructors

### **ExcludeFromImplementedTypesAttribute()**

```csharp
public ExcludeFromImplementedTypesAttribute()
```
