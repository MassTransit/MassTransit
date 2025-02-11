---

title: TaskPropertyConverter<TResult, TInput>

---

# TaskPropertyConverter\<TResult, TInput\>

Namespace: MassTransit.Initializers.PropertyConverters

Converts a [Task\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1) to {T} by awaiting the result

```csharp
public class TaskPropertyConverter<TResult, TInput> : IPropertyConverter<TResult, Task<TInput>>, IPropertyConverter<Task<TResult>, TInput>
```

#### Type Parameters

`TResult`<br/>

`TInput`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TaskPropertyConverter\<TResult, TInput\>](../masstransit-initializers-propertyconverters/taskpropertyconverter-2)<br/>
Implements [IPropertyConverter\<TResult, Task\<TInput\>\>](../masstransit-initializers/ipropertyconverter-2), [IPropertyConverter\<Task\<TResult\>, TInput\>](../masstransit-initializers/ipropertyconverter-2)

## Constructors

### **TaskPropertyConverter(IPropertyConverter\<TResult, TInput\>)**

```csharp
public TaskPropertyConverter(IPropertyConverter<TResult, TInput> converter)
```

#### Parameters

`converter` [IPropertyConverter\<TResult, TInput\>](../masstransit-initializers/ipropertyconverter-2)<br/>

## Methods

### **Convert\<T\>(InitializeContext\<T\>, TInput)**

```csharp
public Task<Task<TResult>> Convert<T>(InitializeContext<T> context, TInput input)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [InitializeContext\<T\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>

`input` TInput<br/>

#### Returns

[Task\<Task\<TResult\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
