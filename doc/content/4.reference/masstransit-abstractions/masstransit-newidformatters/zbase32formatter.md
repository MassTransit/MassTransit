---

title: ZBase32Formatter

---

# ZBase32Formatter

Namespace: MassTransit.NewIdFormatters

```csharp
public class ZBase32Formatter : INewIdFormatter
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ZBase32Formatter](../masstransit-newidformatters/zbase32formatter)<br/>
Implements [INewIdFormatter](../masstransit/inewidformatter)

## Fields

### **LowerCase**

```csharp
public static INewIdFormatter LowerCase;
```

## Constructors

### **ZBase32Formatter(Boolean)**

```csharp
public ZBase32Formatter(bool upperCase)
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
