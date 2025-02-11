---

title: ToNullableTypeConverter<T>

---

# ToNullableTypeConverter\<T\>

Namespace: MassTransit.Initializers.TypeConverters

```csharp
public class ToNullableTypeConverter<T> : ITypeConverter<Nullable<T>, T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ToNullableTypeConverter\<T\>](../masstransit-initializers-typeconverters/tonullabletypeconverter-1)<br/>
Implements [ITypeConverter\<Nullable\<T\>, T\>](../masstransit-initializers/itypeconverter-2)

## Constructors

### **ToNullableTypeConverter()**

```csharp
public ToNullableTypeConverter()
```

## Methods

### **TryConvert(T, Nullable\<T\>)**

```csharp
public bool TryConvert(T input, out Nullable<T> result)
```

#### Parameters

`input` T<br/>

`result` [Nullable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
