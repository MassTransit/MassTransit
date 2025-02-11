---

title: IBucketNode<TValue>

---

# IBucketNode\<TValue\>

Namespace: MassTransit.Caching.Internals

```csharp
public interface IBucketNode<TValue> : INode<TValue>
```

#### Type Parameters

`TValue`<br/>

Implements [INode\<TValue\>](../masstransit-caching/inode-1)

## Properties

### **Bucket**

The node's bucket

```csharp
public abstract Bucket<TValue> Bucket { get; }
```

#### Property Value

[Bucket\<TValue\>](../masstransit-caching-internals/bucket-1)<br/>

### **Next**

Returns the next node in the bucket

```csharp
public abstract IBucketNode<TValue> Next { get; }
```

#### Property Value

[IBucketNode\<TValue\>](../masstransit-caching-internals/ibucketnode-1)<br/>

## Methods

### **SetBucket(Bucket\<TValue\>, IBucketNode\<TValue\>)**

Puts the node's bucket, once the value is resolved, so that the node
 can be tracked.

```csharp
void SetBucket(Bucket<TValue> bucket, IBucketNode<TValue> next)
```

#### Parameters

`bucket` [Bucket\<TValue\>](../masstransit-caching-internals/bucket-1)<br/>

`next` [IBucketNode\<TValue\>](../masstransit-caching-internals/ibucketnode-1)<br/>

### **AssignToBucket(Bucket\<TValue\>)**

Assigns the node to a new bucket, but doesn't change the next node
 until it's cleaned up

```csharp
void AssignToBucket(Bucket<TValue> bucket)
```

#### Parameters

`bucket` [Bucket\<TValue\>](../masstransit-caching-internals/bucket-1)<br/>

### **Evict()**

Forcibly evicts the node by setting the internal state to
 nothing.

```csharp
void Evict()
```

### **Pop()**

Remove the node from the bucket, and return the next node

```csharp
IBucketNode<TValue> Pop()
```

#### Returns

[IBucketNode\<TValue\>](../masstransit-caching-internals/ibucketnode-1)<br/>
