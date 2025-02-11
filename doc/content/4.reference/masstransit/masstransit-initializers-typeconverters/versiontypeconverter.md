---

title: VersionTypeConverter

---

# VersionTypeConverter

Namespace: MassTransit.Initializers.TypeConverters

```csharp
public class VersionTypeConverter : ITypeConverter<String, Version>, ITypeConverter<Version, String>, ITypeConverter<Version, Object>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [VersionTypeConverter](../masstransit-initializers-typeconverters/versiontypeconverter)<br/>
Implements [ITypeConverter\<String, Version\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Version, String\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Version, Object\>](../masstransit-initializers/itypeconverter-2)

## Constructors

### **VersionTypeConverter()**

```csharp
public VersionTypeConverter()
```

## Methods

### **TryConvert(Version, String)**

```csharp
public bool TryConvert(Version input, out string result)
```

#### Parameters

`input` [Version](https://learn.microsoft.com/en-us/dotnet/api/system.version)<br/>

`result` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Object, Version)**

```csharp
public bool TryConvert(object input, out Version result)
```

#### Parameters

`input` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`result` [Version](https://learn.microsoft.com/en-us/dotnet/api/system.version)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(String, Version)**

```csharp
public bool TryConvert(string input, out Version result)
```

#### Parameters

`input` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`result` [Version](https://learn.microsoft.com/en-us/dotnet/api/system.version)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
