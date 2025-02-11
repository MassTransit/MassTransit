---

title: ByteTypeConverter

---

# ByteTypeConverter

Namespace: MassTransit.Initializers.TypeConverters

```csharp
public class ByteTypeConverter : ITypeConverter<String, Byte>, ITypeConverter<Byte, String>, ITypeConverter<Byte, Object>, ITypeConverter<Byte, SByte>, ITypeConverter<Byte, Int16>, ITypeConverter<Byte, UInt16>, ITypeConverter<Byte, Int32>, ITypeConverter<Byte, UInt32>, ITypeConverter<Byte, Int64>, ITypeConverter<Byte, UInt64>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ByteTypeConverter](../masstransit-initializers-typeconverters/bytetypeconverter)<br/>
Implements [ITypeConverter\<String, Byte\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Byte, String\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Byte, Object\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Byte, SByte\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Byte, Int16\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Byte, UInt16\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Byte, Int32\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Byte, UInt32\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Byte, Int64\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Byte, UInt64\>](../masstransit-initializers/itypeconverter-2)

## Constructors

### **ByteTypeConverter()**

```csharp
public ByteTypeConverter()
```

## Methods

### **TryConvert(Int32, Byte)**

```csharp
public bool TryConvert(int input, out byte result)
```

#### Parameters

`input` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`result` [Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Int64, Byte)**

```csharp
public bool TryConvert(long input, out byte result)
```

#### Parameters

`input` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

`result` [Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Object, Byte)**

```csharp
public bool TryConvert(object input, out byte result)
```

#### Parameters

`input` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`result` [Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(SByte, Byte)**

```csharp
public bool TryConvert(sbyte input, out byte result)
```

#### Parameters

`input` [SByte](https://learn.microsoft.com/en-us/dotnet/api/system.sbyte)<br/>

`result` [Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Int16, Byte)**

```csharp
public bool TryConvert(short input, out byte result)
```

#### Parameters

`input` [Int16](https://learn.microsoft.com/en-us/dotnet/api/system.int16)<br/>

`result` [Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(String, Byte)**

```csharp
public bool TryConvert(string input, out byte result)
```

#### Parameters

`input` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`result` [Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(UInt32, Byte)**

```csharp
public bool TryConvert(uint input, out byte result)
```

#### Parameters

`input` [UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br/>

`result` [Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(UInt64, Byte)**

```csharp
public bool TryConvert(ulong input, out byte result)
```

#### Parameters

`input` [UInt64](https://learn.microsoft.com/en-us/dotnet/api/system.uint64)<br/>

`result` [Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(UInt16, Byte)**

```csharp
public bool TryConvert(ushort input, out byte result)
```

#### Parameters

`input` [UInt16](https://learn.microsoft.com/en-us/dotnet/api/system.uint16)<br/>

`result` [Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Byte, String)**

```csharp
public bool TryConvert(byte input, out string result)
```

#### Parameters

`input` [Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

`result` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
