---

title: HexFormatter

---

# HexFormatter

Namespace: MassTransit.NewIdFormatters

```csharp
public class HexFormatter : INewIdFormatter
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [HexFormatter](../masstransit-newidformatters/hexformatter)<br/>
Implements [INewIdFormatter](../masstransit/inewidformatter)

## Constructors

### **HexFormatter(Boolean)**

```csharp
public HexFormatter(bool upperCase)
```

#### Parameters

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
