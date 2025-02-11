---

title: ExceptionTypeConverter

---

# ExceptionTypeConverter

Namespace: MassTransit.Initializers.TypeConverters

```csharp
public class ExceptionTypeConverter : ITypeConverter<String, Exception>, ITypeConverter<ExceptionInfo, Exception>, ITypeConverter<ExceptionInfo, Object>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExceptionTypeConverter](../masstransit-initializers-typeconverters/exceptiontypeconverter)<br/>
Implements [ITypeConverter\<String, Exception\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<ExceptionInfo, Exception\>](../masstransit-initializers/itypeconverter-2), [ITypeConverter\<ExceptionInfo, Object\>](../masstransit-initializers/itypeconverter-2)

## Constructors

### **ExceptionTypeConverter()**

```csharp
public ExceptionTypeConverter()
```

## Methods

### **TryConvert(Exception, ExceptionInfo)**

```csharp
public bool TryConvert(Exception input, out ExceptionInfo result)
```

#### Parameters

`input` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`result` [ExceptionInfo](../../masstransit-abstractions/masstransit/exceptioninfo)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Object, ExceptionInfo)**

```csharp
public bool TryConvert(object input, out ExceptionInfo result)
```

#### Parameters

`input` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`result` [ExceptionInfo](../../masstransit-abstractions/masstransit/exceptioninfo)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryConvert(Exception, String)**

```csharp
public bool TryConvert(Exception input, out string result)
```

#### Parameters

`input` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`result` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
