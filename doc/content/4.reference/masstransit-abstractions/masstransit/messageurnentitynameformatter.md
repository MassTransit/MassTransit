---

title: MessageUrnEntityNameFormatter

---

# MessageUrnEntityNameFormatter

Namespace: MassTransit

This is the simplest thing, it uses the built-in URN for a message type
 as the entity name, which can include illegal characters for most message
 brokers. It's nice for in-memory though, which doesn't give a hoot about the
 string.

```csharp
public class MessageUrnEntityNameFormatter : IEntityNameFormatter
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageUrnEntityNameFormatter](../masstransit/messageurnentitynameformatter)<br/>
Implements [IEntityNameFormatter](../masstransit/ientitynameformatter)

## Constructors

### **MessageUrnEntityNameFormatter()**

```csharp
public MessageUrnEntityNameFormatter()
```

## Methods

### **FormatEntityName\<T\>()**

```csharp
public string FormatEntityName<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
