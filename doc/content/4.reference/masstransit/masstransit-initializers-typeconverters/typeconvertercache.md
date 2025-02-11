---

title: TypeConverterCache

---

# TypeConverterCache

Namespace: MassTransit.Initializers.TypeConverters

```csharp
public class TypeConverterCache : ITypeConverterCache
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TypeConverterCache](../masstransit-initializers-typeconverters/typeconvertercache)<br/>
Implements [ITypeConverterCache](../masstransit-initializers-typeconverters/itypeconvertercache)

## Methods

### **TryGetTypeConverter\<TProperty, TInputProperty\>(ITypeConverter\<TProperty, TInputProperty\>)**

```csharp
public static bool TryGetTypeConverter<TProperty, TInputProperty>(out ITypeConverter<TProperty, TInputProperty> typeConverter)
```

#### Type Parameters

`TProperty`<br/>

`TInputProperty`<br/>

#### Parameters

`typeConverter` [ITypeConverter\<TProperty, TInputProperty\>](../masstransit-initializers/itypeconverter-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
