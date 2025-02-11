---

title: TaskPropertyProvider<TInput, TProperty>

---

# TaskPropertyProvider\<TInput, TProperty\>

Namespace: MassTransit.Initializers.PropertyProviders

```csharp
public class TaskPropertyProvider<TInput, TProperty> : IPropertyProvider<TInput, Task<TProperty>>
```

#### Type Parameters

`TInput`<br/>

`TProperty`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TaskPropertyProvider\<TInput, TProperty\>](../masstransit-initializers-propertyproviders/taskpropertyprovider-2)<br/>
Implements [IPropertyProvider\<TInput, Task\<TProperty\>\>](../masstransit-initializers/ipropertyprovider-2)

## Constructors

### **TaskPropertyProvider(IPropertyProvider\<TInput, TProperty\>)**

```csharp
public TaskPropertyProvider(IPropertyProvider<TInput, TProperty> provider)
```

#### Parameters

`provider` [IPropertyProvider\<TInput, TProperty\>](../masstransit-initializers/ipropertyprovider-2)<br/>

## Methods

### **GetProperty\<T\>(InitializeContext\<T, TInput\>)**

```csharp
public Task<Task<TProperty>> GetProperty<T>(InitializeContext<T, TInput> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [InitializeContext\<T, TInput\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-2)<br/>

#### Returns

[Task\<Task\<TProperty\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
