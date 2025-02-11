---

title: DictionaryPropertyConverter<TKey, TElement>

---

# DictionaryPropertyConverter\<TKey, TElement\>

Namespace: MassTransit.Initializers.PropertyConverters

```csharp
public class DictionaryPropertyConverter<TKey, TElement> : IPropertyConverter<Dictionary<TKey, TElement>, IEnumerable<KeyValuePair<TKey, TElement>>>, IPropertyConverter<IDictionary<TKey, TElement>, IEnumerable<KeyValuePair<TKey, TElement>>>, IPropertyConverter<IReadOnlyDictionary<TKey, TElement>, IEnumerable<KeyValuePair<TKey, TElement>>>, IPropertyConverter<IEnumerable<KeyValuePair<TKey, TElement>>, IEnumerable<KeyValuePair<TKey, TElement>>>
```

#### Type Parameters

`TKey`<br/>

`TElement`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DictionaryPropertyConverter\<TKey, TElement\>](../masstransit-initializers-propertyconverters/dictionarypropertyconverter-2)<br/>
Implements [IPropertyConverter\<Dictionary\<TKey, TElement\>, IEnumerable\<KeyValuePair\<TKey, TElement\>\>\>](../masstransit-initializers/ipropertyconverter-2), [IPropertyConverter\<IDictionary\<TKey, TElement\>, IEnumerable\<KeyValuePair\<TKey, TElement\>\>\>](../masstransit-initializers/ipropertyconverter-2), [IPropertyConverter\<IReadOnlyDictionary\<TKey, TElement\>, IEnumerable\<KeyValuePair\<TKey, TElement\>\>\>](../masstransit-initializers/ipropertyconverter-2), [IPropertyConverter\<IEnumerable\<KeyValuePair\<TKey, TElement\>\>, IEnumerable\<KeyValuePair\<TKey, TElement\>\>\>](../masstransit-initializers/ipropertyconverter-2)

## Constructors

### **DictionaryPropertyConverter()**

```csharp
public DictionaryPropertyConverter()
```

## Methods

### **Convert\<TMessage\>(InitializeContext\<TMessage\>, IEnumerable\<KeyValuePair\<TKey, TElement\>\>)**

```csharp
public Task<Dictionary<TKey, TElement>> Convert<TMessage>(InitializeContext<TMessage> context, IEnumerable<KeyValuePair<TKey, TElement>> input)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`context` [InitializeContext\<TMessage\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>

`input` [IEnumerable\<KeyValuePair\<TKey, TElement\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

#### Returns

[Task\<Dictionary\<TKey, TElement\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
