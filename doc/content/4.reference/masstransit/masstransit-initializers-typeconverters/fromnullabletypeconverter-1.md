---

title: FromNullableTypeConverter<T>

---

# FromNullableTypeConverter\<T\>

Namespace: MassTransit.Initializers.TypeConverters

```csharp
public class FromNullableTypeConverter<T> : ITypeConverter<T, Nullable<T>>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FromNullableTypeConverter\<T\>](../masstransit-initializers-typeconverters/fromnullabletypeconverter-1)<br/>
Implements [ITypeConverter\<T, Nullable\<T\>\>](../masstransit-initializers/itypeconverter-2)

## Constructors

### **FromNullableTypeConverter()**

```csharp
public FromNullableTypeConverter()
```

## Methods

### **TryConvert(Nullable\<T\>, T)**

```csharp
public bool TryConvert(Nullable<T> input, out T result)
```

#### Parameters

`input` [Nullable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`result` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
