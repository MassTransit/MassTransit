---

title: ICache<TValue>

---

# ICache\<TValue\>

Namespace: MassTransit.Caching

```csharp
public interface ICache<TValue> : IConnectCacheValueObserver<TValue>
```

#### Type Parameters

`TValue`<br/>

Implements [IConnectCacheValueObserver\<TValue\>](../masstransit-caching/iconnectcachevalueobserver-1)

## Methods

### **AddIndex\<TKey\>(String, KeyProvider\<TKey, TValue\>, MissingValueFactory\<TKey, TValue\>)**

Create an index on the cache for the specified key type

```csharp
IIndex<TKey, TValue> AddIndex<TKey>(string name, KeyProvider<TKey, TValue> keyProvider, MissingValueFactory<TKey, TValue> missingValueFactory)
```

#### Type Parameters

`TKey`<br/>
The key type for the index

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
A unique index name

`keyProvider` [KeyProvider\<TKey, TValue\>](../masstransit-caching/keyprovider-2)<br/>
The key factory for the value

`missingValueFactory` [MissingValueFactory\<TKey, TValue\>](../masstransit-caching/missingvaluefactory-2)<br/>

#### Returns

[IIndex\<TKey, TValue\>](../masstransit-caching/iindex-2)<br/>
The index, which can be used directly to access the cache

### **GetIndex\<TKey\>(String)**

Get an existing cache index by name

```csharp
IIndex<TKey, TValue> GetIndex<TKey>(string name)
```

#### Type Parameters

`TKey`<br/>

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[IIndex\<TKey, TValue\>](../masstransit-caching/iindex-2)<br/>

### **Add(TValue)**

Adds a value, updating indices, before returning

```csharp
void Add(TValue value)
```

#### Parameters

`value` TValue<br/>
The value to add

### **GetAll()**

Returns all the values in the cache

```csharp
IEnumerable<Task<TValue>> GetAll()
```

#### Returns

[IEnumerable\<Task\<TValue\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Clear()**

Forcibly clear the cache immediately (disposal of cached items may take some time, occurs asynchronously)

```csharp
void Clear()
```
