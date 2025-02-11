---

title: DelegatePropertyProvider<TInput, TProperty>

---

# DelegatePropertyProvider\<TInput, TProperty\>

Namespace: MassTransit.Transformation

Copies the input property, as-is, for the property value

```csharp
public class DelegatePropertyProvider<TInput, TProperty> : IPropertyProvider<TInput, TProperty>
```

#### Type Parameters

`TInput`<br/>

`TProperty`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DelegatePropertyProvider\<TInput, TProperty\>](../masstransit-transformation/delegatepropertyprovider-2)<br/>
Implements [IPropertyProvider\<TInput, TProperty\>](../masstransit-initializers/ipropertyprovider-2)

## Constructors

### **DelegatePropertyProvider(IPropertyProvider\<TInput, TProperty\>, Func\<TransformPropertyContext\<TProperty, TInput\>, Task\<TProperty\>\>)**

```csharp
public DelegatePropertyProvider(IPropertyProvider<TInput, TProperty> inputProvider, Func<TransformPropertyContext<TProperty, TInput>, Task<TProperty>> valueProvider)
```

#### Parameters

`inputProvider` [IPropertyProvider\<TInput, TProperty\>](../masstransit-initializers/ipropertyprovider-2)<br/>

`valueProvider` [Func\<TransformPropertyContext\<TProperty, TInput\>, Task\<TProperty\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

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
