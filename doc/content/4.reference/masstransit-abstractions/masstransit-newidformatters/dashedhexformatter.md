---

title: DashedHexFormatter

---

# DashedHexFormatter

Namespace: MassTransit.NewIdFormatters

```csharp
public class DashedHexFormatter : INewIdFormatter
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DashedHexFormatter](../masstransit-newidformatters/dashedhexformatter)<br/>
Implements [INewIdFormatter](../masstransit/inewidformatter)

## Constructors

### **DashedHexFormatter(Char, Char, Boolean)**

```csharp
public DashedHexFormatter(char prefix, char suffix, bool upperCase)
```

#### Parameters

`prefix` [Char](https://learn.microsoft.com/en-us/dotnet/api/system.char)<br/>

`suffix` [Char](https://learn.microsoft.com/en-us/dotnet/api/system.char)<br/>

`upperCase` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Methods

### **Format(Byte[])**

```csharp
public string Format(in Byte[] bytes)
```

#### Parameters

`bytes` [Byte[]](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
