---

title: DefaultMessageNameFormatter

---

# DefaultMessageNameFormatter

Namespace: MassTransit.Transports

```csharp
public class DefaultMessageNameFormatter : IMessageNameFormatter
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DefaultMessageNameFormatter](../masstransit-transports/defaultmessagenameformatter)<br/>
Implements [IMessageNameFormatter](../../masstransit-abstractions/masstransit-transports/imessagenameformatter)

## Constructors

### **DefaultMessageNameFormatter(String, String, String, String)**

```csharp
public DefaultMessageNameFormatter(string genericArgumentSeparator, string genericTypeSeparator, string namespaceSeparator, string nestedTypeSeparator)
```

#### Parameters

`genericArgumentSeparator` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`genericTypeSeparator` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`namespaceSeparator` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`nestedTypeSeparator` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **GetMessageName(Type)**

```csharp
public string GetMessageName(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
