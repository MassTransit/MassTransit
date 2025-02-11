---

title: GreenCache<TValue>

---

# GreenCache\<TValue\>

Namespace: MassTransit.Caching

```csharp
public class GreenCache<TValue> : ICache<TValue>, IConnectCacheValueObserver<TValue>
```

#### Type Parameters

`TValue`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [GreenCache\<TValue\>](../masstransit-caching/greencache-1)<br/>
Implements [ICache\<TValue\>](../masstransit-caching/icache-1), [IConnectCacheValueObserver\<TValue\>](../masstransit-caching/iconnectcachevalueobserver-1)

## Properties

### **Statistics**

```csharp
public CacheStatistics Statistics { get; }
```

#### Property Value

[CacheStatistics](../masstransit-caching-internals/cachestatistics)<br/>

## Constructors

### **GreenCache(CacheSettings)**

Create a cache using the specified cache settings

```csharp
public GreenCache(CacheSettings settings)
```

#### Parameters

`settings` [CacheSettings](../masstransit-caching/cachesettings)<br/>
The cache settings

## Methods

### **AddIndex\<TKey\>(String, KeyProvider\<TKey, TValue\>, MissingValueFactory\<TKey, TValue\>)**

```csharp
public IIndex<TKey, TValue> AddIndex<TKey>(string name, KeyProvider<TKey, TValue> keyProvider, MissingValueFactory<TKey, TValue> missingValueFactory)
```

#### Type Parameters

`TKey`<br/>

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`keyProvider` [KeyProvider\<TKey, TValue\>](../masstransit-caching/keyprovider-2)<br/>

`missingValueFactory` [MissingValueFactory\<TKey, TValue\>](../masstransit-caching/missingvaluefactory-2)<br/>

#### Returns

[IIndex\<TKey, TValue\>](../masstransit-caching/iindex-2)<br/>

### **GetIndex\<TKey\>(String)**

```csharp
public IIndex<TKey, TValue> GetIndex<TKey>(string name)
```

#### Type Parameters

`TKey`<br/>

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[IIndex\<TKey, TValue\>](../masstransit-caching/iindex-2)<br/>

### **Add(TValue)**

```csharp
public void Add(TValue value)
```

#### Parameters

`value` TValue<br/>

### **Clear()**

```csharp
public void Clear()
```

### **GetAll()**

```csharp
public IEnumerable<Task<TValue>> GetAll()
```

#### Returns

[IEnumerable\<Task\<TValue\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Connect(ICacheValueObserver\<TValue\>)**

```csharp
public ConnectHandle Connect(ICacheValueObserver<TValue> observer)
```

#### Parameters

`observer` [ICacheValueObserver\<TValue\>](../masstransit-caching/icachevalueobserver-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
