---

title: BucketNode<TValue>

---

# BucketNode\<TValue\>

Namespace: MassTransit.Caching.Internals

A bucket node has been stored in a bucket, and is a fully resolved value.

```csharp
public class BucketNode<TValue> : IBucketNode<TValue>, INode<TValue>
```

#### Type Parameters

`TValue`<br/>
The value type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BucketNode\<TValue\>](../masstransit-caching-internals/bucketnode-1)<br/>
Implements [IBucketNode\<TValue\>](../masstransit-caching-internals/ibucketnode-1), [INode\<TValue\>](../masstransit-caching/inode-1)

## Properties

### **Value**

```csharp
public Task<TValue> Value { get; }
```

#### Property Value

[Task\<TValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **HasValue**

```csharp
public bool HasValue { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsValid**

```csharp
public bool IsValid { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Bucket**

```csharp
public Bucket<TValue> Bucket { get; }
```

#### Property Value

[Bucket\<TValue\>](../masstransit-caching-internals/bucket-1)<br/>

### **Next**

```csharp
public IBucketNode<TValue> Next { get; }
```

#### Property Value

[IBucketNode\<TValue\>](../masstransit-caching-internals/ibucketnode-1)<br/>

## Constructors

### **BucketNode(TValue)**

```csharp
public BucketNode(TValue value)
```

#### Parameters

`value` TValue<br/>

## Methods

### **GetValue(IPendingValue\<TValue\>)**

```csharp
public Task<TValue> GetValue(IPendingValue<TValue> pendingValue)
```

#### Parameters

`pendingValue` [IPendingValue\<TValue\>](../masstransit-caching/ipendingvalue-1)<br/>

#### Returns

[Task\<TValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **SetBucket(Bucket\<TValue\>, IBucketNode\<TValue\>)**

```csharp
public void SetBucket(Bucket<TValue> bucket, IBucketNode<TValue> next)
```

#### Parameters

`bucket` [Bucket\<TValue\>](../masstransit-caching-internals/bucket-1)<br/>

`next` [IBucketNode\<TValue\>](../masstransit-caching-internals/ibucketnode-1)<br/>

### **AssignToBucket(Bucket\<TValue\>)**

```csharp
public void AssignToBucket(Bucket<TValue> bucket)
```

#### Parameters

`bucket` [Bucket\<TValue\>](../masstransit-caching-internals/bucket-1)<br/>

### **Evict()**

```csharp
public void Evict()
```

### **Pop()**

```csharp
public IBucketNode<TValue> Pop()
```

#### Returns

[IBucketNode\<TValue\>](../masstransit-caching-internals/ibucketnode-1)<br/>
