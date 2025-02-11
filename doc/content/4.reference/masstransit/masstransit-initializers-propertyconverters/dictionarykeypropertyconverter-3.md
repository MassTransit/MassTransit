---

title: DictionaryKeyPropertyConverter<TKey, TInputKey, TElement>

---

# DictionaryKeyPropertyConverter\<TKey, TInputKey, TElement\>

Namespace: MassTransit.Initializers.PropertyConverters

```csharp
public class DictionaryKeyPropertyConverter<TKey, TInputKey, TElement> : IPropertyConverter<Dictionary<TKey, TElement>, IEnumerable<KeyValuePair<TInputKey, TElement>>>, IPropertyConverter<IDictionary<TKey, TElement>, IEnumerable<KeyValuePair<TInputKey, TElement>>>, IPropertyConverter<IReadOnlyDictionary<TKey, TElement>, IEnumerable<KeyValuePair<TInputKey, TElement>>>, IPropertyConverter<IEnumerable<KeyValuePair<TKey, TElement>>, IEnumerable<KeyValuePair<TInputKey, TElement>>>
```

#### Type Parameters

`TKey`<br/>

`TInputKey`<br/>

`TElement`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DictionaryKeyPropertyConverter\<TKey, TInputKey, TElement\>](../masstransit-initializers-propertyconverters/dictionarykeypropertyconverter-3)<br/>
Implements [IPropertyConverter\<Dictionary\<TKey, TElement\>, IEnumerable\<KeyValuePair\<TInputKey, TElement\>\>\>](../masstransit-initializers/ipropertyconverter-2), [IPropertyConverter\<IDictionary\<TKey, TElement\>, IEnumerable\<KeyValuePair\<TInputKey, TElement\>\>\>](../masstransit-initializers/ipropertyconverter-2), [IPropertyConverter\<IReadOnlyDictionary\<TKey, TElement\>, IEnumerable\<KeyValuePair\<TInputKey, TElement\>\>\>](../masstransit-initializers/ipropertyconverter-2), [IPropertyConverter\<IEnumerable\<KeyValuePair\<TKey, TElement\>\>, IEnumerable\<KeyValuePair\<TInputKey, TElement\>\>\>](../masstransit-initializers/ipropertyconverter-2)

## Constructors

### **DictionaryKeyPropertyConverter(IPropertyConverter\<TKey, TInputKey\>)**

```csharp
public DictionaryKeyPropertyConverter(IPropertyConverter<TKey, TInputKey> converter)
```

#### Parameters

`converter` [IPropertyConverter\<TKey, TInputKey\>](../masstransit-initializers/ipropertyconverter-2)<br/>

## Methods

### **Convert\<TMessage\>(InitializeContext\<TMessage\>, IEnumerable\<KeyValuePair\<TInputKey, TElement\>\>)**

```csharp
public Task<Dictionary<TKey, TElement>> Convert<TMessage>(InitializeContext<TMessage> context, IEnumerable<KeyValuePair<TInputKey, TElement>> input)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`context` [InitializeContext\<TMessage\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>

`input` [IEnumerable\<KeyValuePair\<TInputKey, TElement\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

#### Returns

[Task\<Dictionary\<TKey, TElement\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
