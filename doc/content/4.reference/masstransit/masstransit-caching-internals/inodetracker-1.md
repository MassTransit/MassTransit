---

title: INodeTracker<TValue>

---

# INodeTracker\<TValue\>

Namespace: MassTransit.Caching.Internals

```csharp
public interface INodeTracker<TValue> : IConnectCacheValueObserver<TValue>
```

#### Type Parameters

`TValue`<br/>

Implements [IConnectCacheValueObserver\<TValue\>](../masstransit-caching/iconnectcachevalueobserver-1)

## Properties

### **Statistics**

Maintains statistics for the cache

```csharp
public abstract CacheStatistics Statistics { get; }
```

#### Property Value

[CacheStatistics](../masstransit-caching-internals/cachestatistics)<br/>

## Methods

### **Add(INodeValueFactory\<TValue\>)**

Adds a pending node to the cache, that once resolved, is published
 to the indices

```csharp
void Add(INodeValueFactory<TValue> nodeValueFactory)
```

#### Parameters

`nodeValueFactory` [INodeValueFactory\<TValue\>](../masstransit-caching-internals/inodevaluefactory-1)<br/>

### **Add(TValue)**

Just add the value, straight up

```csharp
void Add(TValue value)
```

#### Parameters

`value` TValue<br/>

### **Rebucket(IBucketNode\<TValue\>)**

Assigns the node to the current bucket, likely do it being touched.

```csharp
void Rebucket(IBucketNode<TValue> node)
```

#### Parameters

`node` [IBucketNode\<TValue\>](../masstransit-caching-internals/ibucketnode-1)<br/>

### **Remove(INode\<TValue\>)**

Remove a node from the cache, notifying all observers that it was removed
 (which updates the indices as well).

```csharp
void Remove(INode<TValue> existingNode)
```

#### Parameters

`existingNode` [INode\<TValue\>](../masstransit-caching/inode-1)<br/>
The node being removed

### **GetAll()**

Returns every known node in the cache from the valid buckets

```csharp
IEnumerable<INode<TValue>> GetAll()
```

#### Returns

[IEnumerable\<INode\<TValue\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Clear()**

Clear the cache, throw out the buckets, time to start over

```csharp
void Clear()
```
