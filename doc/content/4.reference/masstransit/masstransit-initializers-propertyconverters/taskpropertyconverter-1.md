---

title: TaskPropertyConverter<TResult>

---

# TaskPropertyConverter\<TResult\>

Namespace: MassTransit.Initializers.PropertyConverters

Converts a [Task\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1) to {T} by awaiting the result

```csharp
public class TaskPropertyConverter<TResult> : IPropertyConverter<TResult, Task<TResult>>, IPropertyConverter<Task<TResult>, TResult>
```

#### Type Parameters

`TResult`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TaskPropertyConverter\<TResult\>](../masstransit-initializers-propertyconverters/taskpropertyconverter-1)<br/>
Implements [IPropertyConverter\<TResult, Task\<TResult\>\>](../masstransit-initializers/ipropertyconverter-2), [IPropertyConverter\<Task\<TResult\>, TResult\>](../masstransit-initializers/ipropertyconverter-2)

## Constructors

### **TaskPropertyConverter()**

```csharp
public TaskPropertyConverter()
```

## Methods

### **Convert\<TMessage\>(InitializeContext\<TMessage\>, TResult)**

```csharp
public Task<Task<TResult>> Convert<TMessage>(InitializeContext<TMessage> context, TResult input)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`context` [InitializeContext\<TMessage\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>

`input` TResult<br/>

#### Returns

[Task\<Task\<TResult\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
