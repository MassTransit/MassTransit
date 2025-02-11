---

title: IPropertyProviderFactory<TInput>

---

# IPropertyProviderFactory\<TInput\>

Namespace: MassTransit.Initializers

```csharp
public interface IPropertyProviderFactory<TInput>
```

#### Type Parameters

`TInput`<br/>

## Methods

### **TryGetPropertyProvider\<TResult\>(PropertyInfo, IPropertyProvider\<TInput, TResult\>)**

Return the factory to create a property provider for the specified type  using the
  as the source.

```csharp
bool TryGetPropertyProvider<TResult>(PropertyInfo propertyInfo, out IPropertyProvider<TInput, TResult> provider)
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
bool TryGetPropertyConverter<T, TProperty>(out IPropertyConverter<T, TProperty> converter)
```

#### Type Parameters

`T`<br/>

`TProperty`<br/>

#### Parameters

`converter` [IPropertyConverter\<T, TProperty\>](../masstransit-initializers/ipropertyconverter-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
