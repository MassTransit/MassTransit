---

title: GuidTypeConverter

---

# GuidTypeConverter

Namespace: MassTransit.Initializers.TypeConverters

```csharp
public class GuidTypeConverter : ITypeConverter<String, Guid>, ITypeConverter<Guid, String>, ITypeConverter<Guid, NewId>, ITypeConverter<Guid, Object>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [GuidTypeConverter](../masstransit-initializers-typeconverters/guidtypeconverter)<br/>
Implements [ITypeConverter\<String, Guid\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Guid, String\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Guid, NewId\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Guid, Object\>](../masstransit-initializers/itypeconverter-2)

## Constructors

### **GuidTypeConverter()**

```csharp
public GuidTypeConverter()
```

## Methods

### **TryConvert(NewId, Guid)**

```csharp
public bool TryConvert(NewId input, out Guid result)
```

#### Parameters

`input` [NewId](../../masstransit-abstractions/masstransit/newid)<br/>

`result` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Object, Guid)**

```csharp
public bool TryConvert(object input, out Guid result)
```

#### Parameters

`input` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`result` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(String, Guid)**

```csharp
public bool TryConvert(string input, out Guid result)
```

#### Parameters

`input` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`result` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Guid, String)**

```csharp
public bool TryConvert(Guid input, out string result)
```

#### Parameters

`input` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`result` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
