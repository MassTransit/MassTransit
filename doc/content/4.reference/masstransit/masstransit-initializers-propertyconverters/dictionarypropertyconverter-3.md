---

title: DictionaryPropertyConverter<TKey, TElement, TInputElement>

---

# DictionaryPropertyConverter\<TKey, TElement, TInputElement\>

Namespace: MassTransit.Initializers.PropertyConverters

```csharp
public class DictionaryPropertyConverter<TKey, TElement, TInputElement> : IPropertyConverter<Dictionary<TKey, TElement>, IEnumerable<KeyValuePair<TKey, TInputElement>>>, IPropertyConverter<IDictionary<TKey, TElement>, IEnumerable<KeyValuePair<TKey, TInputElement>>>, IPropertyConverter<IReadOnlyDictionary<TKey, TElement>, IEnumerable<KeyValuePair<TKey, TInputElement>>>, IPropertyConverter<IEnumerable<KeyValuePair<TKey, TElement>>, IEnumerable<KeyValuePair<TKey, TInputElement>>>
```

#### Type Parameters

`TKey`<br/>

`TElement`<br/>

`TInputElement`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DictionaryPropertyConverter\<TKey, TElement, TInputElement\>](../masstransit-initializers-propertyconverters/dictionarypropertyconverter-3)<br/>
Implements [IPropertyConverter\<Dictionary\<TKey, TElement\>, IEnumerable\<KeyValuePair\<TKey, TInputElement\>\>\>](../masstransit-initializers/ipropertyconverter-2), [IPropertyConverter\<IDictionary\<TKey, TElement\>, IEnumerable\<KeyValuePair\<TKey, TInputElement\>\>\>](../masstransit-initializers/ipropertyconverter-2), [IPropertyConverter\<IReadOnlyDictionary\<TKey, TElement\>, IEnumerable\<KeyValuePair\<TKey, TInputElement\>\>\>](../masstransit-initializers/ipropertyconverter-2), [IPropertyConverter\<IEnumerable\<KeyValuePair\<TKey, TElement\>\>, IEnumerable\<KeyValuePair\<TKey, TInputElement\>\>\>](../masstransit-initializers/ipropertyconverter-2)

## Constructors

### **DictionaryPropertyConverter(IPropertyConverter\<TElement, TInputElement\>)**

```csharp
public DictionaryPropertyConverter(IPropertyConverter<TElement, TInputElement> converter)
```

#### Parameters

`converter` [IPropertyConverter\<TElement, TInputElement\>](../masstransit-initializers/ipropertyconverter-2)<br/>

## Methods

### **Convert\<TMessage\>(InitializeContext\<TMessage\>, IEnumerable\<KeyValuePair\<TKey, TInputElement\>\>)**

```csharp
public Task<Dictionary<TKey, TElement>> Convert<TMessage>(InitializeContext<TMessage> context, IEnumerable<KeyValuePair<TKey, TInputElement>> input)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`context` [InitializeContext\<TMessage\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>

`input` [IEnumerable\<KeyValuePair\<TKey, TInputElement\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

#### Returns

[Task\<Dictionary\<TKey, TElement\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
