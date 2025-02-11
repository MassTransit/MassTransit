---

title: ToNullablePropertyConverter<TResult>

---

# ToNullablePropertyConverter\<TResult\>

Namespace: MassTransit.Initializers.PropertyConverters

```csharp
public class ToNullablePropertyConverter<TResult> : IPropertyConverter<Nullable<TResult>, TResult>
```

#### Type Parameters

`TResult`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ToNullablePropertyConverter\<TResult\>](../masstransit-initializers-propertyconverters/tonullablepropertyconverter-1)<br/>
Implements [IPropertyConverter\<Nullable\<TResult\>, TResult\>](../masstransit-initializers/ipropertyconverter-2)

## Constructors

### **ToNullablePropertyConverter()**

```csharp
public ToNullablePropertyConverter()
```

## Methods

### **Convert\<T\>(InitializeContext\<T\>, TResult)**

```csharp
public Task<Nullable<TResult>> Convert<T>(InitializeContext<T> context, TResult input)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [InitializeContext\<T\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>

`input` TResult<br/>

#### Returns

[Task\<Nullable\<TResult\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
