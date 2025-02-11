---

title: Bucket<TValue>

---

# Bucket\<TValue\>

Namespace: MassTransit.Caching.Internals

```csharp
public class Bucket<TValue>
```

#### Type Parameters

`TValue`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Bucket\<TValue\>](../masstransit-caching-internals/bucket-1)

## Properties

### **Head**

```csharp
public IBucketNode<TValue> Head { get; }
```

#### Property Value

[IBucketNode\<TValue\>](../masstransit-caching-internals/ibucketnode-1)<br/>

### **Count**

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **Bucket(INodeTracker\<TValue\>)**

```csharp
public Bucket(INodeTracker<TValue> tracker)
```

#### Parameters

`tracker` [INodeTracker\<TValue\>](../masstransit-caching-internals/inodetracker-1)<br/>

## Methods

### **HasExpired(DateTime)**

```csharp
public bool HasExpired(DateTime expirationTime)
```

#### Parameters

`expirationTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsOldEnough(DateTime)**

```csharp
public bool IsOldEnough(DateTime agedTime)
```

#### Parameters

`agedTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Clear()**

Clear the bucket, no node cleanup is performed

```csharp
public void Clear()
```

### **Stop(DateTime)**

```csharp
public void Stop(DateTime now)
```

#### Parameters

`now` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **Start(DateTime)**

```csharp
public void Start(DateTime now)
```

#### Parameters

`now` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **Push(IBucketNode\<TValue\>)**

Push a node to the front of the bucket, and set the node's bucket to this bucket

```csharp
public void Push(IBucketNode<TValue> node)
```

#### Parameters

`node` [IBucketNode\<TValue\>](../masstransit-caching-internals/ibucketnode-1)<br/>

### **Used(IBucketNode\<TValue\>)**

When a node is used, check and rebucket if necessary to keep it in the cache

```csharp
public void Used(IBucketNode<TValue> node)
```

#### Parameters

`node` [IBucketNode\<TValue\>](../masstransit-caching-internals/ibucketnode-1)<br/>
