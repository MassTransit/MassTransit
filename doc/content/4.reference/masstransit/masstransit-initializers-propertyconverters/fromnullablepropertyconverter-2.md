---

title: FromNullablePropertyConverter<TResult, TInput>

---

# FromNullablePropertyConverter\<TResult, TInput\>

Namespace: MassTransit.Initializers.PropertyConverters

```csharp
public class FromNullablePropertyConverter<TResult, TInput> : IPropertyConverter<TResult, Nullable<TInput>>
```

#### Type Parameters

`TResult`<br/>

`TInput`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FromNullablePropertyConverter\<TResult, TInput\>](../masstransit-initializers-propertyconverters/fromnullablepropertyconverter-2)<br/>
Implements [IPropertyConverter\<TResult, Nullable\<TInput\>\>](../masstransit-initializers/ipropertyconverter-2)

## Constructors

### **FromNullablePropertyConverter(IPropertyConverter\<TResult, TInput\>)**

```csharp
public FromNullablePropertyConverter(IPropertyConverter<TResult, TInput> converter)
```

#### Parameters

`converter` [IPropertyConverter\<TResult, TInput\>](../masstransit-initializers/ipropertyconverter-2)<br/>
