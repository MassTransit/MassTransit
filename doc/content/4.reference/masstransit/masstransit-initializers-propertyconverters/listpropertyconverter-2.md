---

title: ListPropertyConverter<TElement, TInputElement>

---

# ListPropertyConverter\<TElement, TInputElement\>

Namespace: MassTransit.Initializers.PropertyConverters

```csharp
public class ListPropertyConverter<TElement, TInputElement> : IPropertyConverter<List<TElement>, IEnumerable<TInputElement>>, IPropertyConverter<IList<TElement>, IEnumerable<TInputElement>>, IPropertyConverter<IReadOnlyList<TElement>, IEnumerable<TInputElement>>, IPropertyConverter<IEnumerable<TElement>, IEnumerable<TInputElement>>, IPropertyConverter<ICollection<TElement>, IEnumerable<TInputElement>>
```

#### Type Parameters

`TElement`<br/>

`TInputElement`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ListPropertyConverter\<TElement, TInputElement\>](../masstransit-initializers-propertyconverters/listpropertyconverter-2)<br/>
Implements [IPropertyConverter\<List\<TElement\>, IEnumerable\<TInputElement\>\>](../masstransit-initializers/ipropertyconverter-2), [IPropertyConverter\<IList\<TElement\>, IEnumerable\<TInputElement\>\>](../masstransit-initializers/ipropertyconverter-2), [IPropertyConverter\<IReadOnlyList\<TElement\>, IEnumerable\<TInputElement\>\>](../masstransit-initializers/ipropertyconverter-2), [IPropertyConverter\<IEnumerable\<TElement\>, IEnumerable\<TInputElement\>\>](../masstransit-initializers/ipropertyconverter-2), [IPropertyConverter\<ICollection\<TElement\>, IEnumerable\<TInputElement\>\>](../masstransit-initializers/ipropertyconverter-2)

## Constructors

### **ListPropertyConverter(IPropertyConverter\<TElement, TInputElement\>)**

```csharp
public ListPropertyConverter(IPropertyConverter<TElement, TInputElement> converter)
```

#### Parameters

`converter` [IPropertyConverter\<TElement, TInputElement\>](../masstransit-initializers/ipropertyconverter-2)<br/>

## Methods

### **Convert\<T\>(InitializeContext\<T\>, IEnumerable\<TInputElement\>)**

```csharp
public Task<ICollection<TElement>> Convert<T>(InitializeContext<T> context, IEnumerable<TInputElement> elements)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [InitializeContext\<T\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>

`elements` [IEnumerable\<TInputElement\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

#### Returns

[Task\<ICollection\<TElement\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
