---

title: ToObjectTypeConverter<T>

---

# ToObjectTypeConverter\<T\>

Namespace: MassTransit.Initializers.TypeConverters

```csharp
public class ToObjectTypeConverter<T> : ITypeConverter<Object, T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ToObjectTypeConverter\<T\>](../masstransit-initializers-typeconverters/toobjecttypeconverter-1)<br/>
Implements [ITypeConverter\<Object, T\>](../masstransit-initializers/itypeconverter-2)

## Constructors

### **ToObjectTypeConverter()**

```csharp
public ToObjectTypeConverter()
```

## Methods

### **TryConvert(T, Object)**

```csharp
public bool TryConvert(T input, out object result)
```

#### Parameters

`input` T<br/>

`result` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
