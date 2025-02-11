---

title: ToNullableTypeConverter<T, TInput>

---

# ToNullableTypeConverter\<T, TInput\>

Namespace: MassTransit.Initializers.TypeConverters

```csharp
public class ToNullableTypeConverter<T, TInput> : ITypeConverter<Nullable<T>, TInput>
```

#### Type Parameters

`T`<br/>

`TInput`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ToNullableTypeConverter\<T, TInput\>](../masstransit-initializers-typeconverters/tonullabletypeconverter-2)<br/>
Implements [ITypeConverter\<Nullable\<T\>, TInput\>](../masstransit-initializers/itypeconverter-2)

## Constructors

### **ToNullableTypeConverter(ITypeConverter\<T, TInput\>)**

```csharp
public ToNullableTypeConverter(ITypeConverter<T, TInput> typeConverter)
```

#### Parameters

`typeConverter` [ITypeConverter\<T, TInput\>](../masstransit-initializers/itypeconverter-2)<br/>

## Methods

### **TryConvert(TInput, Nullable\<T\>)**

```csharp
public bool TryConvert(TInput input, out Nullable<T> result)
```

#### Parameters

`input` TInput<br/>

`result` [Nullable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
