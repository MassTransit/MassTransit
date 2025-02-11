---

title: TypePropertyConverter<TResult, TInput>

---

# TypePropertyConverter\<TResult, TInput\>

Namespace: MassTransit.Initializers.PropertyConverters

Calls the property type converter, returning either the result or default.

```csharp
public class TypePropertyConverter<TResult, TInput> : IPropertyConverter<TResult, TInput>
```

#### Type Parameters

`TResult`<br/>

`TInput`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TypePropertyConverter\<TResult, TInput\>](../masstransit-initializers-propertyconverters/typepropertyconverter-2)<br/>
Implements [IPropertyConverter\<TResult, TInput\>](../masstransit-initializers/ipropertyconverter-2)

## Constructors

### **TypePropertyConverter(ITypeConverter\<TResult, TInput\>)**

```csharp
public TypePropertyConverter(ITypeConverter<TResult, TInput> converter)
```

#### Parameters

`converter` [ITypeConverter\<TResult, TInput\>](../masstransit-initializers/itypeconverter-2)<br/>
