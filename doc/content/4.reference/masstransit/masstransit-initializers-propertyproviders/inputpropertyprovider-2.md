---

title: InputPropertyProvider<TInput, TProperty>

---

# InputPropertyProvider\<TInput, TProperty\>

Namespace: MassTransit.Initializers.PropertyProviders

Copies the input property, as-is, for the property value

```csharp
public class InputPropertyProvider<TInput, TProperty> : IPropertyProvider<TInput, TProperty>
```

#### Type Parameters

`TInput`<br/>

`TProperty`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InputPropertyProvider\<TInput, TProperty\>](../masstransit-initializers-propertyproviders/inputpropertyprovider-2)<br/>
Implements [IPropertyProvider\<TInput, TProperty\>](../masstransit-initializers/ipropertyprovider-2)

## Constructors

### **InputPropertyProvider(PropertyInfo)**

```csharp
public InputPropertyProvider(PropertyInfo propertyInfo)
```

#### Parameters

`propertyInfo` [PropertyInfo](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo)<br/>

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
