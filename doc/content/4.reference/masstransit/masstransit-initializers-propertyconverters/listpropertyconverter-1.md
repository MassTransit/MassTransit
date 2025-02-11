---

title: ListPropertyConverter<TElement>

---

# ListPropertyConverter\<TElement\>

Namespace: MassTransit.Initializers.PropertyConverters

```csharp
public class ListPropertyConverter<TElement> : IPropertyConverter<List<TElement>, IEnumerable<TElement>>, IPropertyConverter<IList<TElement>, IEnumerable<TElement>>, IPropertyConverter<IReadOnlyList<TElement>, IEnumerable<TElement>>, IPropertyConverter<IEnumerable<TElement>, IEnumerable<TElement>>, IPropertyConverter<ICollection<TElement>, IEnumerable<TElement>>
```

#### Type Parameters

`TElement`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ListPropertyConverter\<TElement\>](../masstransit-initializers-propertyconverters/listpropertyconverter-1)<br/>
Implements [IPropertyConverter\<List\<TElement\>, IEnumerable\<TElement\>\>](../masstransit-initializers/ipropertyconverter-2), [IPropertyConverter\<IList\<TElement\>, IEnumerable\<TElement\>\>](../masstransit-initializers/ipropertyconverter-2), [IPropertyConverter\<IReadOnlyList\<TElement\>, IEnumerable\<TElement\>\>](../masstransit-initializers/ipropertyconverter-2), [IPropertyConverter\<IEnumerable\<TElement\>, IEnumerable\<TElement\>\>](../masstransit-initializers/ipropertyconverter-2), [IPropertyConverter\<ICollection\<TElement\>, IEnumerable\<TElement\>\>](../masstransit-initializers/ipropertyconverter-2)

## Constructors

### **ListPropertyConverter()**

```csharp
public ListPropertyConverter()
```

## Methods

### **Convert\<TMessage\>(InitializeContext\<TMessage\>, IEnumerable\<TElement\>)**

```csharp
public Task<List<TElement>> Convert<TMessage>(InitializeContext<TMessage> context, IEnumerable<TElement> input)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`context` [InitializeContext\<TMessage\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>

`input` [IEnumerable\<TElement\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

#### Returns

[Task\<List\<TElement\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
