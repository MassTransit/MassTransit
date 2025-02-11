---

title: Base32Parser

---

# Base32Parser

Namespace: MassTransit.NewIdParsers

```csharp
public class Base32Parser : INewIdParser
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Base32Parser](../masstransit-newidparsers/base32parser)<br/>
Implements [INewIdParser](../masstransit/inewidparser)

## Constructors

### **Base32Parser()**

```csharp
public Base32Parser()
```

### **Base32Parser(String)**

```csharp
public Base32Parser(in string chars)
```

#### Parameters

`chars` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **Parse(String)**

```csharp
public NewId Parse(in string text)
```

#### Parameters

`text` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[NewId](../masstransit/newid)<br/>
