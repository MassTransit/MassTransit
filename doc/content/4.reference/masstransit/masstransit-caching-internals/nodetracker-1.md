---

title: NodeTracker<TValue>

---

# NodeTracker\<TValue\>

Namespace: MassTransit.Caching.Internals

```csharp
public class NodeTracker<TValue> : INodeTracker<TValue>, IConnectCacheValueObserver<TValue>
```

#### Type Parameters

`TValue`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [NodeTracker\<TValue\>](../masstransit-caching-internals/nodetracker-1)<br/>
Implements [INodeTracker\<TValue\>](../masstransit-caching-internals/inodetracker-1), [IConnectCacheValueObserver\<TValue\>](../masstransit-caching/iconnectcachevalueobserver-1)

## Properties

### **Statistics**

```csharp
public CacheStatistics Statistics { get; }
```

#### Property Value

[CacheStatistics](../masstransit-caching-internals/cachestatistics)<br/>

## Constructors

### **NodeTracker(CacheSettings)**

```csharp
public NodeTracker(CacheSettings settings)
```

#### Parameters

`settings` [CacheSettings](../masstransit-caching/cachesettings)<br/>

## Methods

### **Add(INodeValueFactory\<TValue\>)**

```csharp
public void Add(INodeValueFactory<TValue> nodeValueFactory)
```

#### Parameters

`nodeValueFactory` [INodeValueFactory\<TValue\>](../masstransit-caching-internals/inodevaluefactory-1)<br/>

### **Add(TValue)**

```csharp
public void Add(TValue value)
```

#### Parameters

`value` TValue<br/>

### **Remove(INode\<TValue\>)**

```csharp
public void Remove(INode<TValue> node)
```

#### Parameters

`node` [INode\<TValue\>](../masstransit-caching/inode-1)<br/>

### **GetAll()**

```csharp
public IEnumerable<INode<TValue>> GetAll()
```

#### Returns

[IEnumerable\<INode\<TValue\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Clear()**

```csharp
public void Clear()
```

### **Rebucket(IBucketNode\<TValue\>)**

```csharp
public void Rebucket(IBucketNode<TValue> node)
```

#### Parameters

`node` [IBucketNode\<TValue\>](../masstransit-caching-internals/ibucketnode-1)<br/>

### **Connect(ICacheValueObserver\<TValue\>)**

```csharp
public ConnectHandle Connect(ICacheValueObserver<TValue> observer)
```

#### Parameters

`observer` [ICacheValueObserver\<TValue\>](../masstransit-caching/icachevalueobserver-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
