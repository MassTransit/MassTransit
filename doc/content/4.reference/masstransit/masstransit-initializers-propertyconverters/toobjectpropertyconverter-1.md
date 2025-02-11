---

title: ToObjectPropertyConverter<TInput>

---

# ToObjectPropertyConverter\<TInput\>

Namespace: MassTransit.Initializers.PropertyConverters

```csharp
public class ToObjectPropertyConverter<TInput> : IPropertyConverter<Object, TInput>
```

#### Type Parameters

`TInput`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ToObjectPropertyConverter\<TInput\>](../masstransit-initializers-propertyconverters/toobjectpropertyconverter-1)<br/>
Implements [IPropertyConverter\<Object, TInput\>](../masstransit-initializers/ipropertyconverter-2)

## Constructors

### **ToObjectPropertyConverter()**

```csharp
public ToObjectPropertyConverter()
```

## Methods

### **Convert\<T\>(InitializeContext\<T\>, TInput)**

```csharp
public Task<object> Convert<T>(InitializeContext<T> context, TInput input)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [InitializeContext\<T\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>

`input` TInput<br/>

#### Returns

[Task\<Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
