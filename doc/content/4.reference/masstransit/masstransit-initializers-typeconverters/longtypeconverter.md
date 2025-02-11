---

title: LongTypeConverter

---

# LongTypeConverter

Namespace: MassTransit.Initializers.TypeConverters

```csharp
public class LongTypeConverter : ITypeConverter<String, Int64>, ITypeConverter<Int64, String>, ITypeConverter<Int64, Object>, ITypeConverter<Int64, SByte>, ITypeConverter<Int64, Byte>, ITypeConverter<Int64, Int16>, ITypeConverter<Int64, UInt16>, ITypeConverter<Int64, Int32>, ITypeConverter<Int64, UInt32>, ITypeConverter<Int64, UInt64>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [LongTypeConverter](../masstransit-initializers-typeconverters/longtypeconverter)<br/>
Implements [ITypeConverter\<String, Int64\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Int64, String\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Int64, Object\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Int64, SByte\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Int64, Byte\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Int64, Int16\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Int64, UInt16\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Int64, Int32\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Int64, UInt32\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Int64, UInt64\>](../masstransit-initializers/itypeconverter-2)

## Constructors

### **LongTypeConverter()**

```csharp
public LongTypeConverter()
```

## Methods

### **TryConvert(Byte, Int64)**

```csharp
public bool TryConvert(byte input, out long result)
```

#### Parameters

`input` [Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

`result` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Int32, Int64)**

```csharp
public bool TryConvert(int input, out long result)
```

#### Parameters

`input` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`result` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Object, Int64)**

```csharp
public bool TryConvert(object input, out long result)
```

#### Parameters

`input` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`result` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(SByte, Int64)**

```csharp
public bool TryConvert(sbyte input, out long result)
```

#### Parameters

`input` [SByte](https://learn.microsoft.com/en-us/dotnet/api/system.sbyte)<br/>

`result` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Int16, Int64)**

```csharp
public bool TryConvert(short input, out long result)
```

#### Parameters

`input` [Int16](https://learn.microsoft.com/en-us/dotnet/api/system.int16)<br/>

`result` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(String, Int64)**

```csharp
public bool TryConvert(string input, out long result)
```

#### Parameters

`input` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`result` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(UInt32, Int64)**

```csharp
public bool TryConvert(uint input, out long result)
```

#### Parameters

`input` [UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br/>

`result` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(UInt64, Int64)**

```csharp
public bool TryConvert(ulong input, out long result)
```

#### Parameters

`input` [UInt64](https://learn.microsoft.com/en-us/dotnet/api/system.uint64)<br/>

`result` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(UInt16, Int64)**

```csharp
public bool TryConvert(ushort input, out long result)
```

#### Parameters

`input` [UInt16](https://learn.microsoft.com/en-us/dotnet/api/system.uint16)<br/>

`result` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Int64, String)**

```csharp
public bool TryConvert(long input, out string result)
```

#### Parameters

`input` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

`result` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
