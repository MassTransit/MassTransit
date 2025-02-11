---

title: DateTimeTypeConverter

---

# DateTimeTypeConverter

Namespace: MassTransit.Initializers.TypeConverters

```csharp
public class DateTimeTypeConverter : ITypeConverter<String, DateTime>, ITypeConverter<Int32, DateTime>, ITypeConverter<Int64, DateTime>, ITypeConverter<DateTime, String>, ITypeConverter<DateTime, Object>, ITypeConverter<DateTime, DateTimeOffset>, ITypeConverter<DateTime, Int32>, ITypeConverter<DateTime, Int64>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DateTimeTypeConverter](../masstransit-initializers-typeconverters/datetimetypeconverter)<br/>
Implements [ITypeConverter\<String, DateTime\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Int32, DateTime\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Int64, DateTime\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<DateTime, String\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<DateTime, Object\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<DateTime, DateTimeOffset\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<DateTime, Int32\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<DateTime, Int64\>](../masstransit-initializers/itypeconverter-2)

## Constructors

### **DateTimeTypeConverter()**

```csharp
public DateTimeTypeConverter()
```

## Methods

### **TryConvert(DateTimeOffset, DateTime)**

```csharp
public bool TryConvert(DateTimeOffset input, out DateTime result)
```

#### Parameters

`input` [DateTimeOffset](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset)<br/>

`result` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Int32, DateTime)**

```csharp
public bool TryConvert(int input, out DateTime result)
```

#### Parameters

`input` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`result` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Int64, DateTime)**

```csharp
public bool TryConvert(long input, out DateTime result)
```

#### Parameters

`input` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

`result` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Object, DateTime)**

```csharp
public bool TryConvert(object input, out DateTime result)
```

#### Parameters

`input` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`result` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(String, DateTime)**

```csharp
public bool TryConvert(string input, out DateTime result)
```

#### Parameters

`input` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`result` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(DateTime, Int32)**

```csharp
public bool TryConvert(DateTime input, out int result)
```

#### Parameters

`input` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`result` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(DateTime, Int64)**

```csharp
public bool TryConvert(DateTime input, out long result)
```

#### Parameters

`input` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`result` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(DateTime, String)**

```csharp
public bool TryConvert(DateTime input, out string result)
```

#### Parameters

`input` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`result` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
