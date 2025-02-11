---

title: Index<TKey, TValue>

---

# Index\<TKey, TValue\>

Namespace: MassTransit.Caching.Internals

```csharp
public class Index<TKey, TValue> : ICacheIndex<TValue>, ICacheValueObserver<TValue>, IIndex<TKey, TValue>
```

#### Type Parameters

`TKey`<br/>

`TValue`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Index\<TKey, TValue\>](../masstransit-caching-internals/index-2)<br/>
Implements [ICacheIndex\<TValue\>](../masstransit-caching-internals/icacheindex-1), [ICacheValueObserver\<TValue\>](../masstransit-caching/icachevalueobserver-1), [IIndex\<TKey, TValue\>](../masstransit-caching/iindex-2)

## Constructors

### **Index(INodeTracker\<TValue\>, KeyProvider\<TKey, TValue\>)**

```csharp
public Index(INodeTracker<TValue> nodeTracker, KeyProvider<TKey, TValue> keyProvider)
```

#### Parameters

`nodeTracker` [INodeTracker\<TValue\>](../masstransit-caching-internals/inodetracker-1)<br/>

`keyProvider` [KeyProvider\<TKey, TValue\>](../masstransit-caching/keyprovider-2)<br/>

## Methods

### **Clear()**

```csharp
public void Clear()
```

### **Add(INode\<TValue\>)**

```csharp
public Task<bool> Add(INode<TValue> node)
```

#### Parameters

`node` [INode\<TValue\>](../masstransit-caching/inode-1)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **TryGetExistingNode(TValue, INode\<TValue\>)**

```csharp
public bool TryGetExistingNode(TValue value, out INode<TValue> node)
```

#### Parameters

`value` TValue<br/>

`node` [INode\<TValue\>](../masstransit-caching/inode-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **ValueAdded(INode\<TValue\>, TValue)**

```csharp
public void ValueAdded(INode<TValue> node, TValue value)
```

#### Parameters

`node` [INode\<TValue\>](../masstransit-caching/inode-1)<br/>

`value` TValue<br/>

### **ValueRemoved(INode\<TValue\>, TValue)**

```csharp
public void ValueRemoved(INode<TValue> node, TValue value)
```

#### Parameters

`node` [INode\<TValue\>](../masstransit-caching/inode-1)<br/>

`value` TValue<br/>

### **CacheCleared()**

```csharp
public void CacheCleared()
```

### **Get(TKey, MissingValueFactory\<TKey, TValue\>)**

```csharp
public Task<TValue> Get(TKey key, MissingValueFactory<TKey, TValue> missingValueFactory)
```

#### Parameters

`key` TKey<br/>

`missingValueFactory` [MissingValueFactory\<TKey, TValue\>](../masstransit-caching/missingvaluefactory-2)<br/>

#### Returns

[Task\<TValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Remove(TKey)**

```csharp
public bool Remove(TKey key)
```

#### Parameters

`key` TKey<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
