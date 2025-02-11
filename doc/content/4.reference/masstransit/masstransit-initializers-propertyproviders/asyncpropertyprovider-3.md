---

title: AsyncPropertyProvider<TInput, TProperty, TTask>

---

# AsyncPropertyProvider\<TInput, TProperty, TTask\>

Namespace: MassTransit.Initializers.PropertyProviders

Awaits a [Task\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1) property, returning the property value.

```csharp
public class AsyncPropertyProvider<TInput, TProperty, TTask> : IPropertyProvider<TInput, TProperty>
```

#### Type Parameters

`TInput`<br/>

`TProperty`<br/>

`TTask`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AsyncPropertyProvider\<TInput, TProperty, TTask\>](../masstransit-initializers-propertyproviders/asyncpropertyprovider-3)<br/>
Implements [IPropertyProvider\<TInput, TProperty\>](../masstransit-initializers/ipropertyprovider-2)

## Constructors

### **AsyncPropertyProvider(IPropertyProvider\<TInput, Task\<TTask\>\>, IPropertyConverter\<TProperty, TTask\>)**

```csharp
public AsyncPropertyProvider(IPropertyProvider<TInput, Task<TTask>> provider, IPropertyConverter<TProperty, TTask> converter)
```

#### Parameters

`provider` [IPropertyProvider\<TInput, Task\<TTask\>\>](../masstransit-initializers/ipropertyprovider-2)<br/>

`converter` [IPropertyConverter\<TProperty, TTask\>](../masstransit-initializers/ipropertyconverter-2)<br/>
