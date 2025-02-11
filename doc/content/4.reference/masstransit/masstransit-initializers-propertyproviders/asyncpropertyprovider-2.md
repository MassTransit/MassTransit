---

title: AsyncPropertyProvider<TInput, TProperty>

---

# AsyncPropertyProvider\<TInput, TProperty\>

Namespace: MassTransit.Initializers.PropertyProviders

Awaits a [Task\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1) property, returning the property value.

```csharp
public class AsyncPropertyProvider<TInput, TProperty> : IPropertyProvider<TInput, TProperty>
```

#### Type Parameters

`TInput`<br/>

`TProperty`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AsyncPropertyProvider\<TInput, TProperty\>](../masstransit-initializers-propertyproviders/asyncpropertyprovider-2)<br/>
Implements [IPropertyProvider\<TInput, TProperty\>](../masstransit-initializers/ipropertyprovider-2)

## Constructors

### **AsyncPropertyProvider(IPropertyProvider\<TInput, Task\<TProperty\>\>)**

```csharp
public AsyncPropertyProvider(IPropertyProvider<TInput, Task<TProperty>> provider)
```

#### Parameters

`provider` [IPropertyProvider\<TInput, Task\<TProperty\>\>](../masstransit-initializers/ipropertyprovider-2)<br/>
