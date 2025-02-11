---

title: CacheStatistics

---

# CacheStatistics

Namespace: MassTransit.Caching.Internals

```csharp
public class CacheStatistics
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CacheStatistics](../masstransit-caching-internals/cachestatistics)

## Properties

### **ValidityCheckInterval**

```csharp
public TimeSpan ValidityCheckInterval { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **MaxAge**

How long a value can live in the cache until being swept during the next cleanup

```csharp
public TimeSpan MaxAge { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **MinAge**

The shortest time a value can live in the cache, even if it means blowing up the cache size

```csharp
public TimeSpan MinAge { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **BucketSize**

How many values each bucket should hold

```csharp
public int BucketSize { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **BucketCount**

How much buckets are maintained

```csharp
public int BucketCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **OldestBucketIndex**

The lowest bucket index with nodes in it

```csharp
public int OldestBucketIndex { get; private set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **CurrentBucketIndex**

The current bucket for nodes

```csharp
public int CurrentBucketIndex { get; private set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Capacity**

The value limit for the cache

```csharp
public int Capacity { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

**Remarks:**

The actual number of values can exceed the limit if items are being added quickly and take a while to reach the minimum age

### **Count**

Current value count

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **TotalCount**

Total number of values added to the cache since it was created

```csharp
public int TotalCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Misses**

Gets the number of times an item was requested from the cache which did not exist yet, since the cache
 was created.

```csharp
public long Misses { get; }
```

#### Property Value

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

### **Hits**

Gets the number of times an existing item was requested from the cache since the cache
 was created.

```csharp
public long Hits { get; }
```

#### Property Value

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

### **CreateFaults**

The number of node creates which faulted

```csharp
public int CreateFaults { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **CacheStatistics(Int32, Int32, Int32, TimeSpan, TimeSpan, TimeSpan)**

```csharp
public CacheStatistics(int capacity, int bucketCount, int bucketSize, TimeSpan minAge, TimeSpan maxAge, TimeSpan validityCheckInterval)
```

#### Parameters

`capacity` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`bucketCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`bucketSize` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`minAge` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`maxAge` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`validityCheckInterval` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

## Methods

### **Reset()**

Resets the statistics.

```csharp
public void Reset()
```

### **ValueAdded()**

```csharp
internal void ValueAdded()
```

### **ValueRemoved()**

```csharp
internal void ValueRemoved()
```

### **Miss()**

```csharp
internal void Miss()
```

### **Hit()**

```csharp
internal void Hit()
```

### **ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **SetBucketIndices(Int32, Int32)**

```csharp
internal void SetBucketIndices(int oldestBucketIndex, int currentBucketIndex)
```

#### Parameters

`oldestBucketIndex` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`currentBucketIndex` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **CreateFaulted()**

```csharp
public void CreateFaulted()
```
