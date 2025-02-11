---

title: IPropertyConverter<TResult, TProperty>

---

# IPropertyConverter\<TResult, TProperty\>

Namespace: MassTransit.Initializers

A message property converter, which is async, and has access to the context

```csharp
public interface IPropertyConverter<TResult, TProperty>
```

#### Type Parameters

`TResult`<br/>

`TProperty`<br/>

## Methods

### **Convert\<T\>(InitializeContext\<T\>, TProperty)**



```csharp
Task<TResult> Convert<T>(InitializeContext<T> context, TProperty input)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [InitializeContext\<T\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>

`input` TProperty<br/>

#### Returns

[Task\<TResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
