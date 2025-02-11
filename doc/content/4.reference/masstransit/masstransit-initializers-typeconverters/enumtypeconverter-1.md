---

title: EnumTypeConverter<T>

---

# EnumTypeConverter\<T\>

Namespace: MassTransit.Initializers.TypeConverters

```csharp
public class EnumTypeConverter<T> : ITypeConverter<T, String>, ITypeConverter<T, Object>, ITypeConverter<T, SByte>, ITypeConverter<T, Byte>, ITypeConverter<T, Int16>, ITypeConverter<T, UInt16>, ITypeConverter<T, Int32>, ITypeConverter<T, UInt32>, ITypeConverter<T, Int64>, ITypeConverter<T, UInt64>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [EnumTypeConverter\<T\>](../masstransit-initializers-typeconverters/enumtypeconverter-1)<br/>
Implements [ITypeConverter\<T, String\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<T, Object\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<T, SByte\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<T, Byte\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<T, Int16\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<T, UInt16\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<T, Int32\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<T, UInt32\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<T, Int64\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<T, UInt64\>](../masstransit-initializers/itypeconverter-2)

## Constructors

### **EnumTypeConverter()**

```csharp
public EnumTypeConverter()
```

## Methods

### **TryConvert(Byte, T)**

```csharp
public bool TryConvert(byte input, out T result)
```

#### Parameters

`input` [Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

`result` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Int32, T)**

```csharp
public bool TryConvert(int input, out T result)
```

#### Parameters

`input` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`result` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Int64, T)**

```csharp
public bool TryConvert(long input, out T result)
```

#### Parameters

`input` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

`result` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Object, T)**

```csharp
public bool TryConvert(object input, out T result)
```

#### Parameters

`input` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`result` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(SByte, T)**

```csharp
public bool TryConvert(sbyte input, out T result)
```

#### Parameters

`input` [SByte](https://learn.microsoft.com/en-us/dotnet/api/system.sbyte)<br/>

`result` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Int16, T)**

```csharp
public bool TryConvert(short input, out T result)
```

#### Parameters

`input` [Int16](https://learn.microsoft.com/en-us/dotnet/api/system.int16)<br/>

`result` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(String, T)**

```csharp
public bool TryConvert(string input, out T result)
```

#### Parameters

`input` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`result` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(UInt32, T)**

```csharp
public bool TryConvert(uint input, out T result)
```

#### Parameters

`input` [UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br/>

`result` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(UInt64, T)**

```csharp
public bool TryConvert(ulong input, out T result)
```

#### Parameters

`input` [UInt64](https://learn.microsoft.com/en-us/dotnet/api/system.uint64)<br/>

`result` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(UInt16, T)**

```csharp
public bool TryConvert(ushort input, out T result)
```

#### Parameters

`input` [UInt16](https://learn.microsoft.com/en-us/dotnet/api/system.uint16)<br/>

`result` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
