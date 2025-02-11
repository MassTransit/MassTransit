---

title: DecimalTypeConverter

---

# DecimalTypeConverter

Namespace: MassTransit.Initializers.TypeConverters

```csharp
public class DecimalTypeConverter : ITypeConverter<String, Decimal>, ITypeConverter<Decimal, String>, ITypeConverter<Decimal, Object>, ITypeConverter<Decimal, SByte>, ITypeConverter<Decimal, Byte>, ITypeConverter<Decimal, Int16>, ITypeConverter<Decimal, UInt16>, ITypeConverter<Decimal, Int32>, ITypeConverter<Decimal, UInt32>, ITypeConverter<Decimal, Int64>, ITypeConverter<Decimal, UInt64>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DecimalTypeConverter](../masstransit-initializers-typeconverters/decimaltypeconverter)<br/>
Implements [ITypeConverter\<String, Decimal\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Decimal, String\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Decimal, Object\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Decimal, SByte\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Decimal, Byte\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Decimal, Int16\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Decimal, UInt16\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Decimal, Int32\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Decimal, UInt32\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Decimal, Int64\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<Decimal, UInt64\>](../masstransit-initializers/itypeconverter-2)

## Constructors

### **DecimalTypeConverter()**

```csharp
public DecimalTypeConverter()
```

## Methods

### **TryConvert(Byte, Decimal)**

```csharp
public bool TryConvert(byte input, out decimal result)
```

#### Parameters

`input` [Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

`result` [Decimal](https://learn.microsoft.com/en-us/dotnet/api/system.decimal)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Int32, Decimal)**

```csharp
public bool TryConvert(int input, out decimal result)
```

#### Parameters

`input` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`result` [Decimal](https://learn.microsoft.com/en-us/dotnet/api/system.decimal)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Int64, Decimal)**

```csharp
public bool TryConvert(long input, out decimal result)
```

#### Parameters

`input` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

`result` [Decimal](https://learn.microsoft.com/en-us/dotnet/api/system.decimal)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Object, Decimal)**

```csharp
public bool TryConvert(object input, out decimal result)
```

#### Parameters

`input` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`result` [Decimal](https://learn.microsoft.com/en-us/dotnet/api/system.decimal)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(SByte, Decimal)**

```csharp
public bool TryConvert(sbyte input, out decimal result)
```

#### Parameters

`input` [SByte](https://learn.microsoft.com/en-us/dotnet/api/system.sbyte)<br/>

`result` [Decimal](https://learn.microsoft.com/en-us/dotnet/api/system.decimal)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Int16, Decimal)**

```csharp
public bool TryConvert(short input, out decimal result)
```

#### Parameters

`input` [Int16](https://learn.microsoft.com/en-us/dotnet/api/system.int16)<br/>

`result` [Decimal](https://learn.microsoft.com/en-us/dotnet/api/system.decimal)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(String, Decimal)**

```csharp
public bool TryConvert(string input, out decimal result)
```

#### Parameters

`input` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`result` [Decimal](https://learn.microsoft.com/en-us/dotnet/api/system.decimal)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(UInt32, Decimal)**

```csharp
public bool TryConvert(uint input, out decimal result)
```

#### Parameters

`input` [UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br/>

`result` [Decimal](https://learn.microsoft.com/en-us/dotnet/api/system.decimal)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(UInt64, Decimal)**

```csharp
public bool TryConvert(ulong input, out decimal result)
```

#### Parameters

`input` [UInt64](https://learn.microsoft.com/en-us/dotnet/api/system.uint64)<br/>

`result` [Decimal](https://learn.microsoft.com/en-us/dotnet/api/system.decimal)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(UInt16, Decimal)**

```csharp
public bool TryConvert(ushort input, out decimal result)
```

#### Parameters

`input` [UInt16](https://learn.microsoft.com/en-us/dotnet/api/system.uint16)<br/>

`result` [Decimal](https://learn.microsoft.com/en-us/dotnet/api/system.decimal)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Decimal, String)**

```csharp
public bool TryConvert(decimal input, out string result)
```

#### Parameters

`input` [Decimal](https://learn.microsoft.com/en-us/dotnet/api/system.decimal)<br/>

`result` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
