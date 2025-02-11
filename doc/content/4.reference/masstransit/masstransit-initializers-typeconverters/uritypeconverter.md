---

title: UriTypeConverter

---

# UriTypeConverter

Namespace: MassTransit.Initializers.TypeConverters

```csharp
public class UriTypeConverter : ITypeConverter<String, Uri>, ITypeConverter<Uri, String>, ITypeConverter<Uri, Object>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [UriTypeConverter](../masstransit-initializers-typeconverters/uritypeconverter)<br/>
Implements [ITypeConverter\<String, Uri\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Uri, String\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Uri, Object\>](../masstransit-initializers/itypeconverter-2)

## Constructors

### **UriTypeConverter()**

```csharp
public UriTypeConverter()
```

## Methods

### **TryConvert(Uri, String)**

```csharp
public bool TryConvert(Uri input, out string result)
```

#### Parameters

`input` Uri<br/>

`result` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Object, Uri)**

```csharp
public bool TryConvert(object input, out Uri result)
```

#### Parameters

`input` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`result` Uri<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(String, Uri)**

```csharp
public bool TryConvert(string input, out Uri result)
```

#### Parameters

`input` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`result` Uri<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
