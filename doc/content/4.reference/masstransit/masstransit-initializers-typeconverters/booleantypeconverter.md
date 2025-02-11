---

title: BooleanTypeConverter

---

# BooleanTypeConverter

Namespace: MassTransit.Initializers.TypeConverters

```csharp
public class BooleanTypeConverter : ITypeConverter<String, Boolean>, ITypeConverter<Boolean, String>, ITypeConverter<Boolean, Object>, ITypeConverter<Boolean, SByte>, ITypeConverter<Boolean, Byte>, ITypeConverter<Boolean, Int16>, ITypeConverter<Boolean, UInt16>, ITypeConverter<Boolean, Int32>, ITypeConverter<Boolean, UInt32>, ITypeConverter<Boolean, Int64>, ITypeConverter<Boolean, UInt64>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BooleanTypeConverter](../masstransit-initializers-typeconverters/booleantypeconverter)<br/>
Implements [ITypeConverter\<String, Boolean\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Boolean, String\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Boolean, Object\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Boolean, SByte\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Boolean, Byte\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Boolean, Int16\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Boolean, UInt16\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Boolean, Int32\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Boolean, UInt32\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Boolean, Int64\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Boolean, UInt64\>](../masstransit-initializers/itypeconverter-2)

## Constructors

### **BooleanTypeConverter()**

```csharp
public BooleanTypeConverter()
```

## Methods

### **TryConvert(Byte, Boolean)**

```csharp
public bool TryConvert(byte input, out bool result)
```

#### Parameters

`input` [Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

`result` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Int32, Boolean)**

```csharp
public bool TryConvert(int input, out bool result)
```

#### Parameters

`input` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`result` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Int64, Boolean)**

```csharp
public bool TryConvert(long input, out bool result)
```

#### Parameters

`input` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

`result` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Object, Boolean)**

```csharp
public bool TryConvert(object input, out bool result)
```

#### Parameters

`input` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`result` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(SByte, Boolean)**

```csharp
public bool TryConvert(sbyte input, out bool result)
```

#### Parameters

`input` [SByte](https://learn.microsoft.com/en-us/dotnet/api/system.sbyte)<br/>

`result` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Int16, Boolean)**

```csharp
public bool TryConvert(short input, out bool result)
```

#### Parameters

`input` [Int16](https://learn.microsoft.com/en-us/dotnet/api/system.int16)<br/>

`result` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(String, Boolean)**

```csharp
public bool TryConvert(string input, out bool result)
```

#### Parameters

`input` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`result` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(UInt32, Boolean)**

```csharp
public bool TryConvert(uint input, out bool result)
```

#### Parameters

`input` [UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br/>

`result` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(UInt64, Boolean)**

```csharp
public bool TryConvert(ulong input, out bool result)
```

#### Parameters

`input` [UInt64](https://learn.microsoft.com/en-us/dotnet/api/system.uint64)<br/>

`result` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(UInt16, Boolean)**

```csharp
public bool TryConvert(ushort input, out bool result)
```

#### Parameters

`input` [UInt16](https://learn.microsoft.com/en-us/dotnet/api/system.uint16)<br/>

`result` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Boolean, String)**

```csharp
public bool TryConvert(bool input, out string result)
```

#### Parameters

`input` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

`result` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
