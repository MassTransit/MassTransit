---

title: Base32Formatter

---

# Base32Formatter

Namespace: MassTransit.NewIdFormatters

```csharp
public class Base32Formatter : INewIdFormatter
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Base32Formatter](../masstransit-newidformatters/base32formatter)<br/>
Implements [INewIdFormatter](../masstransit/inewidformatter)

## Constructors

### **Base32Formatter(Boolean)**

```csharp
public Base32Formatter(bool upperCase)
```

#### Parameters

`upperCase` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Base32Formatter(String)**

```csharp
public Base32Formatter(in string chars)
```

#### Parameters

`chars` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **Format(Byte[])**

```csharp
public string Format(in Byte[] bytes)
```

#### Parameters

`bytes` [Byte[]](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
