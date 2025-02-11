---

title: FromNullablePropertyProvider<TInput, TProperty>

---

# FromNullablePropertyProvider\<TInput, TProperty\>

Namespace: MassTransit.Initializers.PropertyProviders

```csharp
public class FromNullablePropertyProvider<TInput, TProperty> : IPropertyProvider<TInput, TProperty>
```

#### Type Parameters

`TInput`<br/>

`TProperty`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FromNullablePropertyProvider\<TInput, TProperty\>](../masstransit-initializers-propertyproviders/fromnullablepropertyprovider-2)<br/>
Implements [IPropertyProvider\<TInput, TProperty\>](../masstransit-initializers/ipropertyprovider-2)

## Constructors

### **FromNullablePropertyProvider(IPropertyProvider\<TInput, Nullable\<TProperty\>\>)**

```csharp
public FromNullablePropertyProvider(IPropertyProvider<TInput, Nullable<TProperty>> provider)
```

#### Parameters

`provider` [IPropertyProvider\<TInput, Nullable\<TProperty\>\>](../masstransit-initializers/ipropertyprovider-2)<br/>

## Methods

### **GetProperty\<T\>(InitializeContext\<T, TInput\>)**

```csharp
public Task<TProperty> GetProperty<T>(InitializeContext<T, TInput> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [InitializeContext\<T, TInput\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-2)<br/>

#### Returns

[Task\<TProperty\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
