---

title: InputDictionaryPropertyProvider<TInput, TProperty>

---

# InputDictionaryPropertyProvider\<TInput, TProperty\>

Namespace: MassTransit.Initializers.PropertyProviders

Copies the input property, as-is, for the property value

```csharp
public class InputDictionaryPropertyProvider<TInput, TProperty> : IPropertyProvider<TInput, TProperty>
```

#### Type Parameters

`TInput`<br/>

`TProperty`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InputDictionaryPropertyProvider\<TInput, TProperty\>](../masstransit-initializers-propertyproviders/inputdictionarypropertyprovider-2)<br/>
Implements [IPropertyProvider\<TInput, TProperty\>](../masstransit-initializers/ipropertyprovider-2)

## Constructors

### **InputDictionaryPropertyProvider(String)**

```csharp
public InputDictionaryPropertyProvider(string key)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

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
