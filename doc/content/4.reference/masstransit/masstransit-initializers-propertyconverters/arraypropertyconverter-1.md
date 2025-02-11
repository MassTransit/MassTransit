---

title: ArrayPropertyConverter<TElement>

---

# ArrayPropertyConverter\<TElement\>

Namespace: MassTransit.Initializers.PropertyConverters

```csharp
public class ArrayPropertyConverter<TElement> : IPropertyConverter<TElement[], IEnumerable<TElement>>
```

#### Type Parameters

`TElement`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ArrayPropertyConverter\<TElement\>](../masstransit-initializers-propertyconverters/arraypropertyconverter-1)<br/>
Implements [IPropertyConverter\<TElement[], IEnumerable\<TElement\>\>](../masstransit-initializers/ipropertyconverter-2)

## Constructors

### **ArrayPropertyConverter()**

```csharp
public ArrayPropertyConverter()
```

## Methods

### **Convert\<TMessage\>(InitializeContext\<TMessage\>, IEnumerable\<TElement\>)**

```csharp
public Task<TElement[]> Convert<TMessage>(InitializeContext<TMessage> context, IEnumerable<TElement> input)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`context` [InitializeContext\<TMessage\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>

`input` [IEnumerable\<TElement\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

#### Returns

[Task\<TElement[]\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
