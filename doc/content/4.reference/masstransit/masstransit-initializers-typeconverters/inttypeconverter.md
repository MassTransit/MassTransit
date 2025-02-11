---

title: IntTypeConverter

---

# IntTypeConverter

Namespace: MassTransit.Initializers.TypeConverters

```csharp
public class IntTypeConverter : ITypeConverter<String, Int32>, ITypeConverter<Int32, Object>, ITypeConverter<Int32, String>, ITypeConverter<Int32, SByte>, ITypeConverter<Int32, Byte>, ITypeConverter<Int32, Int16>, ITypeConverter<Int32, UInt16>, ITypeConverter<Int32, UInt32>, ITypeConverter<Int32, Int64>, ITypeConverter<Int32, UInt64>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [IntTypeConverter](../masstransit-initializers-typeconverters/inttypeconverter)<br/>
Implements [ITypeConverter\<String, Int32\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Int32, Object\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Int32, String\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Int32, SByte\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Int32, Byte\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Int32, Int16\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Int32, UInt16\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Int32, UInt32\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Int32, Int64\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Int32, UInt64\>](../masstransit-initializers/itypeconverter-2)

## Constructors

### **IntTypeConverter()**

```csharp
public IntTypeConverter()
```

## Methods

### **TryConvert(Byte, Int32)**

```csharp
public bool TryConvert(byte input, out int result)
```

#### Parameters

`input` [Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

`result` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Int64, Int32)**

```csharp
public bool TryConvert(long input, out int result)
```

#### Parameters

`input` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

`result` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Object, Int32)**

```csharp
public bool TryConvert(object input, out int result)
```

#### Parameters

`input` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`result` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(SByte, Int32)**

```csharp
public bool TryConvert(sbyte input, out int result)
```

#### Parameters

`input` [SByte](https://learn.microsoft.com/en-us/dotnet/api/system.sbyte)<br/>

`result` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Int16, Int32)**

```csharp
public bool TryConvert(short input, out int result)
```

#### Parameters

`input` [Int16](https://learn.microsoft.com/en-us/dotnet/api/system.int16)<br/>

`result` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(String, Int32)**

```csharp
public bool TryConvert(string input, out int result)
```

#### Parameters

`input` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`result` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(UInt32, Int32)**

```csharp
public bool TryConvert(uint input, out int result)
```

#### Parameters

`input` [UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br/>

`result` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(UInt64, Int32)**

```csharp
public bool TryConvert(ulong input, out int result)
```

#### Parameters

`input` [UInt64](https://learn.microsoft.com/en-us/dotnet/api/system.uint64)<br/>

`result` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(UInt16, Int32)**

```csharp
public bool TryConvert(ushort input, out int result)
```

#### Parameters

`input` [UInt16](https://learn.microsoft.com/en-us/dotnet/api/system.uint16)<br/>

`result` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Int32, String)**

```csharp
public bool TryConvert(int input, out string result)
```

#### Parameters

`input` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`result` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
