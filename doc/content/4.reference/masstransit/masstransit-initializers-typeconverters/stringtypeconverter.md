---

title: StringTypeConverter

---

# StringTypeConverter

Namespace: MassTransit.Initializers.TypeConverters

```csharp
public class StringTypeConverter : ITypeConverter<String, Object>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [StringTypeConverter](../masstransit-initializers-typeconverters/stringtypeconverter)<br/>
Implements [ITypeConverter\<String, Object\>](../masstransit-initializers/itypeconverter-2)

## Constructors

### **StringTypeConverter()**

```csharp
public StringTypeConverter()
```

## Methods

### **TryConvert(Object, String)**

```csharp
public bool TryConvert(object input, out string result)
```

#### Parameters

`input` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`result` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
