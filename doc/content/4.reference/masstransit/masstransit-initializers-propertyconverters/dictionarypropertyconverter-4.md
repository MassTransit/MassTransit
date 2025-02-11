---

title: DictionaryPropertyConverter<TKey, TElement, TInputKey, TInputElement>

---

# DictionaryPropertyConverter\<TKey, TElement, TInputKey, TInputElement\>

Namespace: MassTransit.Initializers.PropertyConverters

```csharp
public class DictionaryPropertyConverter<TKey, TElement, TInputKey, TInputElement> : IPropertyConverter<Dictionary<TKey, TElement>, IEnumerable<KeyValuePair<TInputKey, TInputElement>>>, IPropertyConverter<IDictionary<TKey, TElement>, IEnumerable<KeyValuePair<TInputKey, TInputElement>>>, IPropertyConverter<IReadOnlyDictionary<TKey, TElement>, IEnumerable<KeyValuePair<TInputKey, TInputElement>>>, IPropertyConverter<IEnumerable<KeyValuePair<TKey, TElement>>, IEnumerable<KeyValuePair<TInputKey, TInputElement>>>
```

#### Type Parameters

`TKey`<br/>

`TElement`<br/>

`TInputKey`<br/>

`TInputElement`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DictionaryPropertyConverter\<TKey, TElement, TInputKey, TInputElement\>](../masstransit-initializers-propertyconverters/dictionarypropertyconverter-4)<br/>
Implements [IPropertyConverter\<Dictionary\<TKey, TElement\>, IEnumerable\<KeyValuePair\<TInputKey, TInputElement\>\>\>](../masstransit-initializers/ipropertyconverter-2), [IPropertyConverter\<IDictionary\<TKey, TElement\>, IEnumerable\<KeyValuePair\<TInputKey, TInputElement\>\>\>](../masstransit-initializers/ipropertyconverter-2), [IPropertyConverter\<IReadOnlyDictionary\<TKey, TElement\>, IEnumerable\<KeyValuePair\<TInputKey, TInputElement\>\>\>](../masstransit-initializers/ipropertyconverter-2), [IPropertyConverter\<IEnumerable\<KeyValuePair\<TKey, TElement\>\>, IEnumerable\<KeyValuePair\<TInputKey, TInputElement\>\>\>](../masstransit-initializers/ipropertyconverter-2)

## Constructors

### **DictionaryPropertyConverter(IPropertyConverter\<TKey, TInputKey\>, IPropertyConverter\<TElement, TInputElement\>)**

```csharp
public DictionaryPropertyConverter(IPropertyConverter<TKey, TInputKey> keyConverter, IPropertyConverter<TElement, TInputElement> elementConverter)
```

#### Parameters

`keyConverter` [IPropertyConverter\<TKey, TInputKey\>](../masstransit-initializers/ipropertyconverter-2)<br/>

`elementConverter` [IPropertyConverter\<TElement, TInputElement\>](../masstransit-initializers/ipropertyconverter-2)<br/>

## Methods

### **Convert\<TMessage\>(InitializeContext\<TMessage\>, IEnumerable\<KeyValuePair\<TInputKey, TInputElement\>\>)**

```csharp
public Task<Dictionary<TKey, TElement>> Convert<TMessage>(InitializeContext<TMessage> context, IEnumerable<KeyValuePair<TInputKey, TInputElement>> input)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`context` [InitializeContext\<TMessage\>](../../masstransit-abstractions/masstransit-initializers/initializecontext-1)<br/>

`input` [IEnumerable\<KeyValuePair\<TInputKey, TInputElement\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

#### Returns

[Task\<Dictionary\<TKey, TElement\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
