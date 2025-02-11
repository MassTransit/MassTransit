---

title: DateTimeOffsetTypeConverter

---

# DateTimeOffsetTypeConverter

Namespace: MassTransit.Initializers.TypeConverters

```csharp
public class DateTimeOffsetTypeConverter : ITypeConverter<String, DateTimeOffset>, ITypeConverter<Int32, DateTimeOffset>, ITypeConverter<Int64, DateTimeOffset>, ITypeConverter<DateTimeOffset, String>, ITypeConverter<DateTimeOffset, Object>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DateTimeOffsetTypeConverter](../masstransit-initializers-typeconverters/datetimeoffsettypeconverter)<br/>
Implements [ITypeConverter\<String, DateTimeOffset\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Int32, DateTimeOffset\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Int64, DateTimeOffset\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<DateTimeOffset, String\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<DateTimeOffset, Object\>](../masstransit-initializers/itypeconverter-2)

## Constructors

### **DateTimeOffsetTypeConverter()**

```csharp
public DateTimeOffsetTypeConverter()
```

## Methods

### **TryConvert(Object, DateTimeOffset)**

```csharp
public bool TryConvert(object input, out DateTimeOffset result)
```

#### Parameters

`input` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`result` [DateTimeOffset](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(String, DateTimeOffset)**

```csharp
public bool TryConvert(string input, out DateTimeOffset result)
```

#### Parameters

`input` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`result` [DateTimeOffset](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(DateTimeOffset, Int32)**

```csharp
public bool TryConvert(DateTimeOffset input, out int result)
```

#### Parameters

`input` [DateTimeOffset](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset)<br/>

`result` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(DateTimeOffset, Int64)**

```csharp
public bool TryConvert(DateTimeOffset input, out long result)
```

#### Parameters

`input` [DateTimeOffset](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset)<br/>

`result` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(DateTimeOffset, String)**

```csharp
public bool TryConvert(DateTimeOffset input, out string result)
```

#### Parameters

`input` [DateTimeOffset](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset)<br/>

`result` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
