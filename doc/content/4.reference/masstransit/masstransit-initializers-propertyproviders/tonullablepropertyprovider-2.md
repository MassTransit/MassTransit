---

title: ToNullablePropertyProvider<TInput, TProperty>

---

# ToNullablePropertyProvider\<TInput, TProperty\>

Namespace: MassTransit.Initializers.PropertyProviders

```csharp
public class ToNullablePropertyProvider<TInput, TProperty> : IPropertyProvider<TInput, Nullable<TProperty>>
```

#### Type Parameters

`TInput`<br/>

`TProperty`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ToNullablePropertyProvider\<TInput, TProperty\>](../masstransit-initializers-propertyproviders/tonullablepropertyprovider-2)<br/>
Implements [IPropertyProvider\<TInput, Nullable\<TProperty\>\>](../masstransit-initializers/ipropertyprovider-2)

## Constructors

### **ToNullablePropertyProvider(IPropertyProvider\<TInput, TProperty\>)**

```csharp
public ToNullablePropertyProvider(IPropertyProvider<TInput, TProperty> provider)
```

#### Parameters

`provider` [IPropertyProvider\<TInput, TProperty\>](../masstransit-initializers/ipropertyprovider-2)<br/>

## Methods

### **GetProperty\<T\>(InitializeContext\<T, TInput\>)**

```csharp
public Task<Nullable<TProperty>> GetProperty<T>(InitializeContext<T, TInput> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [InitializeContext\<T, TInput\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-2)<br/>

#### Returns

[Task\<Nullable\<TProperty\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
