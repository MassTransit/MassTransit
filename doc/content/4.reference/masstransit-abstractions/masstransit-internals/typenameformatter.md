---

title: TypeNameFormatter

---

# TypeNameFormatter

Namespace: MassTransit.Internals

```csharp
public class TypeNameFormatter
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TypeNameFormatter](../masstransit-internals/typenameformatter)

## Constructors

### **TypeNameFormatter()**

```csharp
public TypeNameFormatter()
```

### **TypeNameFormatter(String, String, String, String, String)**

```csharp
public TypeNameFormatter(string genericArgumentSeparator, string genericOpen, string genericClose, string namespaceSeparator, string nestedTypeSeparator)
```

#### Parameters

`genericArgumentSeparator` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`genericOpen` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`genericClose` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`namespaceSeparator` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`nestedTypeSeparator` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **GetTypeName(Type)**

```csharp
public string GetTypeName(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
