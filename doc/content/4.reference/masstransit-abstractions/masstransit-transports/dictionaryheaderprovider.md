---

title: DictionaryHeaderProvider

---

# DictionaryHeaderProvider

Namespace: MassTransit.Transports

A simple in-memory header collection for use with the in memory transport

```csharp
public class DictionaryHeaderProvider : IHeaderProvider
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DictionaryHeaderProvider](../masstransit-transports/dictionaryheaderprovider)<br/>
Implements [IHeaderProvider](../masstransit-transports/iheaderprovider)

## Constructors

### **DictionaryHeaderProvider(IDictionary\<String, Object\>)**

```csharp
public DictionaryHeaderProvider(IDictionary<string, object> headers)
```

#### Parameters

`headers` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

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
