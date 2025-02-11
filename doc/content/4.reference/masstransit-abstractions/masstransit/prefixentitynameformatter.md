---

title: PrefixEntityNameFormatter

---

# PrefixEntityNameFormatter

Namespace: MassTransit

```csharp
public class PrefixEntityNameFormatter : IEntityNameFormatter
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PrefixEntityNameFormatter](../masstransit/prefixentitynameformatter)<br/>
Implements [IEntityNameFormatter](../masstransit/ientitynameformatter)

## Constructors

### **PrefixEntityNameFormatter(IEntityNameFormatter, String)**

```csharp
public PrefixEntityNameFormatter(IEntityNameFormatter entityNameFormatter, string prefix)
```

#### Parameters

`entityNameFormatter` [IEntityNameFormatter](../masstransit/ientitynameformatter)<br/>

`prefix` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **FormatEntityName\<T\>()**

```csharp
public string FormatEntityName<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
