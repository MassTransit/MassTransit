---

title: ShortTypeConverter

---

# ShortTypeConverter

Namespace: MassTransit.Initializers.TypeConverters

```csharp
public class ShortTypeConverter : ITypeConverter<String, Int16>, ITypeConverter<Int16, String>, ITypeConverter<Int16, Object>, ITypeConverter<Int16, SByte>, ITypeConverter<Int16, Byte>, ITypeConverter<Int16, UInt16>, ITypeConverter<Int16, Int32>, ITypeConverter<Int16, UInt32>, ITypeConverter<Int16, Int64>, ITypeConverter<Int16, UInt64>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ShortTypeConverter](../masstransit-initializers-typeconverters/shorttypeconverter)<br/>
Implements [ITypeConverter\<String, Int16\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Int16, String\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Int16, Object\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Int16, SByte\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Int16, Byte\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Int16, UInt16\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Int16, Int32\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Int16, UInt32\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Int16, Int64\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Int16, UInt64\>](../masstransit-initializers/itypeconverter-2)

## Constructors

### **ShortTypeConverter()**

```csharp
public ShortTypeConverter()
```

## Methods

### **TryConvert(Byte, Int16)**

```csharp
public bool TryConvert(byte input, out short result)
```

#### Parameters

`input` [Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

`result` [Int16](https://learn.microsoft.com/en-us/dotnet/api/system.int16)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Int32, Int16)**

```csharp
public bool TryConvert(int input, out short result)
```

#### Parameters

`input` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`result` [Int16](https://learn.microsoft.com/en-us/dotnet/api/system.int16)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Int64, Int16)**

```csharp
public bool TryConvert(long input, out short result)
```

#### Parameters

`input` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

`result` [Int16](https://learn.microsoft.com/en-us/dotnet/api/system.int16)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Object, Int16)**

```csharp
public bool TryConvert(object input, out short result)
```

#### Parameters

`input` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`result` [Int16](https://learn.microsoft.com/en-us/dotnet/api/system.int16)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(SByte, Int16)**

```csharp
public bool TryConvert(sbyte input, out short result)
```

#### Parameters

`input` [SByte](https://learn.microsoft.com/en-us/dotnet/api/system.sbyte)<br/>

`result` [Int16](https://learn.microsoft.com/en-us/dotnet/api/system.int16)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(String, Int16)**

```csharp
public bool TryConvert(string input, out short result)
```

#### Parameters

`input` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`result` [Int16](https://learn.microsoft.com/en-us/dotnet/api/system.int16)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(UInt32, Int16)**

```csharp
public bool TryConvert(uint input, out short result)
```

#### Parameters

`input` [UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br/>

`result` [Int16](https://learn.microsoft.com/en-us/dotnet/api/system.int16)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(UInt64, Int16)**

```csharp
public bool TryConvert(ulong input, out short result)
```

#### Parameters

`input` [UInt64](https://learn.microsoft.com/en-us/dotnet/api/system.uint64)<br/>

`result` [Int16](https://learn.microsoft.com/en-us/dotnet/api/system.int16)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(UInt16, Int16)**

```csharp
public bool TryConvert(ushort input, out short result)
```

#### Parameters

`input` [UInt16](https://learn.microsoft.com/en-us/dotnet/api/system.uint16)<br/>

`result` [Int16](https://learn.microsoft.com/en-us/dotnet/api/system.int16)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Int16, String)**

```csharp
public bool TryConvert(short input, out string result)
```

#### Parameters

`input` [Int16](https://learn.microsoft.com/en-us/dotnet/api/system.int16)<br/>

`result` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
