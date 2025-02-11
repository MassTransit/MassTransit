---

title: VariablePropertyProvider<TInput, TProperty, TValue>

---

# VariablePropertyProvider\<TInput, TProperty, TValue\>

Namespace: MassTransit.Initializers.PropertyProviders

Copies the input property, as-is, for the property value

```csharp
public class VariablePropertyProvider<TInput, TProperty, TValue> : IPropertyProvider<TInput, TValue>
```

#### Type Parameters

`TInput`<br/>

`TProperty`<br/>

`TValue`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [VariablePropertyProvider\<TInput, TProperty, TValue\>](../masstransit-initializers-propertyproviders/variablepropertyprovider-3)<br/>
Implements [IPropertyProvider\<TInput, TValue\>](../masstransit-initializers/ipropertyprovider-2)

## Constructors

### **VariablePropertyProvider(IPropertyProvider\<TInput, TProperty\>)**

```csharp
public VariablePropertyProvider(IPropertyProvider<TInput, TProperty> provider)
```

#### Parameters

`provider` [IPropertyProvider\<TInput, TProperty\>](../masstransit-initializers/ipropertyprovider-2)<br/>

## Methods

### **GetProperty\<T\>(InitializeContext\<T, TInput\>)**

```csharp
public Task<TValue> GetProperty<T>(InitializeContext<T, TInput> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [InitializeContext\<T, TInput\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-2)<br/>

#### Returns

[Task\<TValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
