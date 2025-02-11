---

title: DictionarySendHeaders

---

# DictionarySendHeaders

Namespace: MassTransit.Serialization

```csharp
public class DictionarySendHeaders : SendHeaders, Headers, IEnumerable<HeaderValue>, IEnumerable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DictionarySendHeaders](../masstransit-serialization/dictionarysendheaders)<br/>
Implements [SendHeaders](../../masstransit-abstractions/masstransit/sendheaders), [Headers](../../masstransit-abstractions/masstransit/headers), [IEnumerable\<HeaderValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Constructors

### **DictionarySendHeaders()**

```csharp
public DictionarySendHeaders()
```

### **DictionarySendHeaders(IDictionary\<String, Object\>)**

```csharp
public DictionarySendHeaders(IDictionary<string, object> headers)
```

#### Parameters

`headers` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

### **DictionarySendHeaders(IDictionary\<String, Object\>, Boolean)**

```csharp
public DictionarySendHeaders(IDictionary<string, object> headers, bool useExistingDictionary)
```

#### Parameters

`headers` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`useExistingDictionary` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Methods

### **Set(String, String)**

```csharp
public void Set(string key, string value)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Set(String, Object, Boolean)**

```csharp
public void Set(string key, object value, bool overwrite)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`overwrite` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetHeader(String, Object)**

```csharp
public bool TryGetHeader(string key, out object value)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetAll()**

```csharp
public IEnumerable<KeyValuePair<string, object>> GetAll()
```

#### Returns

[IEnumerable\<KeyValuePair\<String, Object\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

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
