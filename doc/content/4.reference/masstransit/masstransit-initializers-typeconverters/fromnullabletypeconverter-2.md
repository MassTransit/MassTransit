---

title: FromNullableTypeConverter<T, TInput>

---

# FromNullableTypeConverter\<T, TInput\>

Namespace: MassTransit.Initializers.TypeConverters

```csharp
public class FromNullableTypeConverter<T, TInput> : ITypeConverter<T, Nullable<TInput>>
```

#### Type Parameters

`T`<br/>

`TInput`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FromNullableTypeConverter\<T, TInput\>](../masstransit-initializers-typeconverters/fromnullabletypeconverter-2)<br/>
Implements [ITypeConverter\<T, Nullable\<TInput\>\>](../masstransit-initializers/itypeconverter-2)

## Constructors

### **FromNullableTypeConverter(ITypeConverter\<T, TInput\>)**

```csharp
public FromNullableTypeConverter(ITypeConverter<T, TInput> typeConverter)
```

#### Parameters

`typeConverter` [ITypeConverter\<T, TInput\>](../masstransit-initializers/itypeconverter-2)<br/>

## Methods

### **TryConvert(Nullable\<TInput\>, T)**

```csharp
public bool TryConvert(Nullable<TInput> input, out T result)
```

#### Parameters

`input` [Nullable\<TInput\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`result` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
