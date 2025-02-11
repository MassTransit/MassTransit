---

title: DictionarySendHeaderProvider

---

# DictionarySendHeaderProvider

Namespace: MassTransit.Transports

A simple in-memory header collection for use with the in memory transport

```csharp
public class DictionarySendHeaderProvider : IHeaderProvider
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DictionarySendHeaderProvider](../masstransit-transports/dictionarysendheaderprovider)<br/>
Implements [IHeaderProvider](../masstransit-transports/iheaderprovider)

## Constructors

### **DictionarySendHeaderProvider(SendHeaders)**

```csharp
public DictionarySendHeaderProvider(SendHeaders headers)
```

#### Parameters

`headers` [SendHeaders](../masstransit/sendheaders)<br/>

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
