---

title: PropertyProviderFactory<TInput>

---

# PropertyProviderFactory\<TInput\>

Namespace: MassTransit.Initializers.PropertyProviders

For an input type, builds the property providers for the requested result types

```csharp
public class PropertyProviderFactory<TInput> : IPropertyProviderFactory<TInput>
```

#### Type Parameters

`TInput`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PropertyProviderFactory\<TInput\>](../masstransit-initializers-propertyproviders/propertyproviderfactory-1)<br/>
Implements [IPropertyProviderFactory\<TInput\>](../masstransit-initializers/ipropertyproviderfactory-1)

## Constructors

### **PropertyProviderFactory()**

```csharp
public PropertyProviderFactory()
```

## Methods

### **TryGetPropertyProvider\<TResult\>(PropertyInfo, IPropertyProvider\<TInput, TResult\>)**

Return the factory to create a property provider for the specified type  using the
  as the source.

```csharp
public bool TryGetPropertyProvider<TResult>(PropertyInfo propertyInfo, out IPropertyProvider<TInput, TResult> provider)
```

#### Type Parameters

`TResult`<br/>

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>
The input property

`provider` [IPropertyProvider\<TInput, TResult\>](../masstransit-initializers/ipropertyprovider-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetPropertyConverter\<T, TProperty\>(IPropertyConverter\<T, TProperty\>)**

```csharp
public bool TryGetPropertyConverter<T, TProperty>(out IPropertyConverter<T, TProperty> converter)
```

#### Type Parameters

`T`<br/>

`TProperty`<br/>

#### Parameters

`converter` [IPropertyConverter\<T, TProperty\>](../masstransit-initializers/ipropertyconverter-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
