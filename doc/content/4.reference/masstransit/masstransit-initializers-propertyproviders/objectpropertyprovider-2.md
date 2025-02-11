---

title: ObjectPropertyProvider<TInput, TProperty>

---

# ObjectPropertyProvider\<TInput, TProperty\>

Namespace: MassTransit.Initializers.PropertyProviders

```csharp
public class ObjectPropertyProvider<TInput, TProperty> : IPropertyProvider<TInput, TProperty>
```

#### Type Parameters

`TInput`<br/>

`TProperty`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ObjectPropertyProvider\<TInput, TProperty\>](../masstransit-initializers-propertyproviders/objectpropertyprovider-2)<br/>
Implements [IPropertyProvider\<TInput, TProperty\>](../masstransit-initializers/ipropertyprovider-2)

## Constructors

### **ObjectPropertyProvider(IPropertyProviderFactory\<TInput\>, IPropertyProvider\<TInput, Object\>)**

```csharp
public ObjectPropertyProvider(IPropertyProviderFactory<TInput> factory, IPropertyProvider<TInput, object> provider)
```

#### Parameters

`factory` [IPropertyProviderFactory\<TInput\>](../masstransit-initializers/ipropertyproviderfactory-1)<br/>

`provider` [IPropertyProvider\<TInput, Object\>](../masstransit-initializers/ipropertyprovider-2)<br/>

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
