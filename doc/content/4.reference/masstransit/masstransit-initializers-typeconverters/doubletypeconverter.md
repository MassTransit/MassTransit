---

title: DoubleTypeConverter

---

# DoubleTypeConverter

Namespace: MassTransit.Initializers.TypeConverters

```csharp
public class DoubleTypeConverter : ITypeConverter<String, Double>, ITypeConverter<Double, String>, ITypeConverter<Double, Object>, ITypeConverter<Double, SByte>, ITypeConverter<Double, Byte>, ITypeConverter<Double, Int16>, ITypeConverter<Double, UInt16>, ITypeConverter<Double, Int32>, ITypeConverter<Double, UInt32>, ITypeConverter<Double, Int64>, ITypeConverter<Double, UInt64>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DoubleTypeConverter](../masstransit-initializers-typeconverters/doubletypeconverter)<br/>
Implements [ITypeConverter\<String, Double\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Double, String\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Double, Object\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Double, SByte\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Double, Byte\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Double, Int16\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Double, UInt16\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Double, Int32\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Double, UInt32\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Double, Int64\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Double, UInt64\>](../masstransit-initializers/itypeconverter-2)

## Constructors

### **DoubleTypeConverter()**

```csharp
public DoubleTypeConverter()
```

## Methods

### **TryConvert(Byte, Double)**

```csharp
public bool TryConvert(byte input, out double result)
```

#### Parameters

`input` [Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

`result` [Double](https://learn.microsoft.com/en-us/dotnet/api/system.double)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Int32, Double)**

```csharp
public bool TryConvert(int input, out double result)
```

#### Parameters

`input` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`result` [Double](https://learn.microsoft.com/en-us/dotnet/api/system.double)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Int64, Double)**

```csharp
public bool TryConvert(long input, out double result)
```

#### Parameters

`input` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

`result` [Double](https://learn.microsoft.com/en-us/dotnet/api/system.double)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Object, Double)**

```csharp
public bool TryConvert(object input, out double result)
```

#### Parameters

`input` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`result` [Double](https://learn.microsoft.com/en-us/dotnet/api/system.double)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(SByte, Double)**

```csharp
public bool TryConvert(sbyte input, out double result)
```

#### Parameters

`input` [SByte](https://learn.microsoft.com/en-us/dotnet/api/system.sbyte)<br/>

`result` [Double](https://learn.microsoft.com/en-us/dotnet/api/system.double)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Int16, Double)**

```csharp
public bool TryConvert(short input, out double result)
```

#### Parameters

`input` [Int16](https://learn.microsoft.com/en-us/dotnet/api/system.int16)<br/>

`result` [Double](https://learn.microsoft.com/en-us/dotnet/api/system.double)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(String, Double)**

```csharp
public bool TryConvert(string input, out double result)
```

#### Parameters

`input` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`result` [Double](https://learn.microsoft.com/en-us/dotnet/api/system.double)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(UInt32, Double)**

```csharp
public bool TryConvert(uint input, out double result)
```

#### Parameters

`input` [UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br/>

`result` [Double](https://learn.microsoft.com/en-us/dotnet/api/system.double)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(UInt64, Double)**

```csharp
public bool TryConvert(ulong input, out double result)
```

#### Parameters

`input` [UInt64](https://learn.microsoft.com/en-us/dotnet/api/system.uint64)<br/>

`result` [Double](https://learn.microsoft.com/en-us/dotnet/api/system.double)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(UInt16, Double)**

```csharp
public bool TryConvert(ushort input, out double result)
```

#### Parameters

`input` [UInt16](https://learn.microsoft.com/en-us/dotnet/api/system.uint16)<br/>

`result` [Double](https://learn.microsoft.com/en-us/dotnet/api/system.double)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Double, String)**

```csharp
public bool TryConvert(double input, out string result)
```

#### Parameters

`input` [Double](https://learn.microsoft.com/en-us/dotnet/api/system.double)<br/>

`result` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
