---

title: TimeSpanTypeConverter

---

# TimeSpanTypeConverter

Namespace: MassTransit.Initializers.TypeConverters

```csharp
public class TimeSpanTypeConverter : ITypeConverter<String, TimeSpan>, ITypeConverter<TimeSpan, String>, ITypeConverter<TimeSpan, Object>, ITypeConverter<TimeSpan, SByte>, ITypeConverter<TimeSpan, Byte>, ITypeConverter<TimeSpan, Int16>, ITypeConverter<TimeSpan, UInt16>, ITypeConverter<TimeSpan, Int32>, ITypeConverter<TimeSpan, UInt32>, ITypeConverter<TimeSpan, Int64>, ITypeConverter<TimeSpan, UInt64>, ITypeConverter<TimeSpan, Double>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TimeSpanTypeConverter](../masstransit-initializers-typeconverters/timespantypeconverter)<br/>
Implements [ITypeConverter\<String, TimeSpan\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<TimeSpan, String\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<TimeSpan, Object\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<TimeSpan, SByte\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<TimeSpan, Byte\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<TimeSpan, Int16\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<TimeSpan, UInt16\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<TimeSpan, Int32\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<TimeSpan, UInt32\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<TimeSpan, Int64\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<TimeSpan, UInt64\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<TimeSpan, Double\>](../masstransit-initializers/itypeconverter-2)

## Constructors

### **TimeSpanTypeConverter()**

```csharp
public TimeSpanTypeConverter()
```

## Methods

### **TryConvert(TimeSpan, String)**

```csharp
public bool TryConvert(TimeSpan input, out string result)
```

#### Parameters

`input` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`result` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Byte, TimeSpan)**

```csharp
public bool TryConvert(byte input, out TimeSpan result)
```

#### Parameters

`input` [Byte](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

`result` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Double, TimeSpan)**

```csharp
public bool TryConvert(double input, out TimeSpan result)
```

#### Parameters

`input` [Double](https://learn.microsoft.com/en-us/dotnet/api/system.double)<br/>

`result` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Int32, TimeSpan)**

```csharp
public bool TryConvert(int input, out TimeSpan result)
```

#### Parameters

`input` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`result` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Int64, TimeSpan)**

```csharp
public bool TryConvert(long input, out TimeSpan result)
```

#### Parameters

`input` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

`result` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Object, TimeSpan)**

```csharp
public bool TryConvert(object input, out TimeSpan result)
```

#### Parameters

`input` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`result` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(SByte, TimeSpan)**

```csharp
public bool TryConvert(sbyte input, out TimeSpan result)
```

#### Parameters

`input` [SByte](https://learn.microsoft.com/en-us/dotnet/api/system.sbyte)<br/>

`result` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Int16, TimeSpan)**

```csharp
public bool TryConvert(short input, out TimeSpan result)
```

#### Parameters

`input` [Int16](https://learn.microsoft.com/en-us/dotnet/api/system.int16)<br/>

`result` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(String, TimeSpan)**

```csharp
public bool TryConvert(string input, out TimeSpan result)
```

#### Parameters

`input` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`result` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(UInt32, TimeSpan)**

```csharp
public bool TryConvert(uint input, out TimeSpan result)
```

#### Parameters

`input` [UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br/>

`result` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(UInt64, TimeSpan)**

```csharp
public bool TryConvert(ulong input, out TimeSpan result)
```

#### Parameters

`input` [UInt64](https://learn.microsoft.com/en-us/dotnet/api/system.uint64)<br/>

`result` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(UInt16, TimeSpan)**

```csharp
public bool TryConvert(ushort input, out TimeSpan result)
```

#### Parameters

`input` [UInt16](https://learn.microsoft.com/en-us/dotnet/api/system.uint16)<br/>

`result` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
