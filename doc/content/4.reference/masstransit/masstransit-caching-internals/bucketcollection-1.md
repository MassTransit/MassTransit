---

title: BucketCollection<TValue>

---

# BucketCollection\<TValue\>

Namespace: MassTransit.Caching.Internals

An ordered collection of buckets, used by the node tracker to keep track of nodes

```csharp
public class BucketCollection<TValue>
```

#### Type Parameters

`TValue`<br/>
The value type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BucketCollection\<TValue\>](../masstransit-caching-internals/bucketcollection-1)

## Properties

### **Item**

```csharp
public Bucket<TValue> Item { get; }
```

#### Property Value

[Bucket\<TValue\>](../masstransit-caching-internals/bucket-1)<br/>

### **Count**

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **BucketCollection(INodeTracker\<TValue\>, Int32)**

```csharp
public BucketCollection(INodeTracker<TValue> nodeTracker, int capacity)
```

#### Parameters

`nodeTracker` [INodeTracker\<TValue\>](../masstransit-caching-internals/inodetracker-1)<br/>

`capacity` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Methods

### **Empty()**

Empties every bucket in the collection, evicting all the nodes

```csharp
public void Empty()
```
