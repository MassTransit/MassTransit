---

title: ToNullablePropertyConverter<TResult, TInput>

---

# ToNullablePropertyConverter\<TResult, TInput\>

Namespace: MassTransit.Initializers.PropertyConverters

```csharp
public class ToNullablePropertyConverter<TResult, TInput> : IPropertyConverter<Nullable<TResult>, TInput>
```

#### Type Parameters

`TResult`<br/>

`TInput`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ToNullablePropertyConverter\<TResult, TInput\>](../masstransit-initializers-propertyconverters/tonullablepropertyconverter-2)<br/>
Implements [IPropertyConverter\<Nullable\<TResult\>, TInput\>](../masstransit-initializers/ipropertyconverter-2)

## Constructors

### **ToNullablePropertyConverter(IPropertyConverter\<TResult, TInput\>)**

```csharp
public ToNullablePropertyConverter(IPropertyConverter<TResult, TInput> converter)
```

#### Parameters

`converter` [IPropertyConverter\<TResult, TInput\>](../masstransit-initializers/ipropertyconverter-2)<br/>

## Methods

### **Convert\<T\>(InitializeContext\<T\>, TInput)**

```csharp
public Task<Nullable<TResult>> Convert<T>(InitializeContext<T> context, TInput input)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [InitializeContext\<T\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>

`input` TInput<br/>

#### Returns

[Task\<Nullable\<TResult\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
