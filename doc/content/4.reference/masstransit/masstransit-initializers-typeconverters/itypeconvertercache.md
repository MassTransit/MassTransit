---

title: ITypeConverterCache

---

# ITypeConverterCache

Namespace: MassTransit.Initializers.TypeConverters

```csharp
public interface ITypeConverterCache
```

## Methods

### **TryGetTypeConverter\<TProperty, TInput\>(ITypeConverter\<TProperty, TInput\>)**

```csharp
bool TryGetTypeConverter<TProperty, TInput>(out ITypeConverter<TProperty, TInput> typeConverter)
```

#### Type Parameters

`TProperty`<br/>

`TInput`<br/>

#### Parameters

`typeConverter` [ITypeConverter\<TProperty, TInput\>](../masstransit-initializers/itypeconverter-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
