---

title: MassTransitCache<TKey, TValue, TCacheValue>

---

# MassTransitCache\<TKey, TValue, TCacheValue\>

Namespace: MassTransit.Internals.Caching

```csharp
public class MassTransitCache<TKey, TValue, TCacheValue> : ICache<TKey, TValue, TCacheValue>, ICache<TKey, TValue>
```

#### Type Parameters

`TKey`<br/>

`TValue`<br/>

`TCacheValue`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MassTransitCache\<TKey, TValue, TCacheValue\>](../masstransit-internals-caching/masstransitcache-3)<br/>
Implements [ICache\<TKey, TValue, TCacheValue\>](../masstransit-internals-caching/icache-3), [ICache\<TKey, TValue\>](../masstransit-internals-caching/icache-2)

## Properties

### **Count**

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **HitRatio**

```csharp
public double HitRatio { get; }
```

#### Property Value

[Double](https://learn.microsoft.com/en-us/dotnet/api/system.double)<br/>

### **Values**

```csharp
public Task<IEnumerable<TValue>> Values { get; }
```

#### Property Value

[Task\<IEnumerable\<TValue\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

## Constructors

### **MassTransitCache(ICachePolicy\<TValue, TCacheValue\>, CacheOptions)**

```csharp
public MassTransitCache(ICachePolicy<TValue, TCacheValue> policy, CacheOptions options)
```

#### Parameters

`policy` [ICachePolicy\<TValue, TCacheValue\>](../masstransit-internals-caching/icachepolicy-2)<br/>

`options` [CacheOptions](../masstransit-internals-caching/cacheoptions)<br/>

## Methods

### **Get(TKey)**

```csharp
public Task<TValue> Get(TKey key)
```

#### Parameters

`key` TKey<br/>

#### Returns

[Task\<TValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetOrAdd(TKey, MissingValueFactory\<TKey, TValue\>)**

```csharp
public Task<TValue> GetOrAdd(TKey key, MissingValueFactory<TKey, TValue> missingValueFactory)
```

#### Parameters

`key` TKey<br/>

`missingValueFactory` [MissingValueFactory\<TKey, TValue\>](../masstransit-internals-caching/missingvaluefactory-2)<br/>

#### Returns

[Task\<TValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Remove(TKey)**

```csharp
public Task<bool> Remove(TKey key)
```

#### Parameters

`key` TKey<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Clear()**

```csharp
public Task Clear()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
