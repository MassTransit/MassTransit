---

title: ConstantPropertyProvider<TInput, TProperty>

---

# ConstantPropertyProvider\<TInput, TProperty\>

Namespace: MassTransit.Initializers.PropertyProviders

Returns a constant value for the property

```csharp
public class ConstantPropertyProvider<TInput, TProperty> : IPropertyProvider<TInput, TProperty>
```

#### Type Parameters

`TInput`<br/>

`TProperty`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConstantPropertyProvider\<TInput, TProperty\>](../masstransit-initializers-propertyproviders/constantpropertyprovider-2)<br/>
Implements [IPropertyProvider\<TInput, TProperty\>](../masstransit-initializers/ipropertyprovider-2)

## Constructors

### **ConstantPropertyProvider(TProperty)**

```csharp
public ConstantPropertyProvider(TProperty propertyValue)
```

#### Parameters

`propertyValue` TProperty<br/>

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
