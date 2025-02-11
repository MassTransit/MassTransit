---

title: ReadOnlyDictionaryHeaders

---

# ReadOnlyDictionaryHeaders

Namespace: MassTransit.Serialization

When a message envelope is deserialized, encapsulate the headers such that objects can be deserialized from the
 body using the message deserializer.

```csharp
public class ReadOnlyDictionaryHeaders : Headers, IEnumerable<HeaderValue>, IEnumerable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReadOnlyDictionaryHeaders](../masstransit-serialization/readonlydictionaryheaders)<br/>
Implements [Headers](../../masstransit-abstractions/masstransit/headers), [IEnumerable\<HeaderValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Constructors

### **ReadOnlyDictionaryHeaders(IObjectDeserializer, IReadOnlyDictionary\<String, Object\>)**

```csharp
public ReadOnlyDictionaryHeaders(IObjectDeserializer deserializer, IReadOnlyDictionary<string, object> headers)
```

#### Parameters

`deserializer` [IObjectDeserializer](../../masstransit-abstractions/masstransit/iobjectdeserializer)<br/>

`headers` [IReadOnlyDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2)<br/>

## Methods

### **GetAll()**

```csharp
public IEnumerable<KeyValuePair<string, object>> GetAll()
```

#### Returns

[IEnumerable\<KeyValuePair\<String, Object\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **TryGetHeader(String, Object)**

```csharp
public bool TryGetHeader(string key, out object value)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Get\<T\>(String, T)**

```csharp
public T Get<T>(string key, T defaultValue)
```

#### Type Parameters

`T`<br/>

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`defaultValue` T<br/>

#### Returns

T<br/>

### **Get\<T\>(String, Nullable\<T\>)**

```csharp
public Nullable<T> Get<T>(string key, Nullable<T> defaultValue)
```

#### Type Parameters

`T`<br/>

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`defaultValue` [Nullable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[Nullable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **GetEnumerator()**

```csharp
public IEnumerator<HeaderValue> GetEnumerator()
```

#### Returns

[IEnumerator\<HeaderValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerator-1)<br/>
