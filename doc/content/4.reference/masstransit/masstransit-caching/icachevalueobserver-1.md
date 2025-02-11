---

title: ICacheValueObserver<TValue>

---

# ICacheValueObserver\<TValue\>

Namespace: MassTransit.Caching

Observes behavior within the cache

```csharp
public interface ICacheValueObserver<TValue>
```

#### Type Parameters

`TValue`<br/>
The value type

## Methods

### **ValueAdded(INode\<TValue\>, TValue)**

Called when a new node is added to the cache, after the node has resolved.

```csharp
void ValueAdded(INode<TValue> node, TValue value)
```

#### Parameters

`node` [INode\<TValue\>](../masstransit-caching/inode-1)<br/>
The cached node

`value` TValue<br/>
The cached value, to avoid awaiting

### **ValueRemoved(INode\<TValue\>, TValue)**

Called when a node is removed from the cache.

```csharp
void ValueRemoved(INode<TValue> node, TValue value)
```

#### Parameters

`node` [INode\<TValue\>](../masstransit-caching/inode-1)<br/>
The cached node

`value` TValue<br/>
The cached value, to avoid awaiting

### **CacheCleared()**

Called when the cache is cleared of all nodes.

```csharp
void CacheCleared()
```
