---

title: TestCacheSettings

---

# TestCacheSettings

Namespace: MassTransit.Caching

```csharp
public class TestCacheSettings : CacheSettings
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CacheSettings](../masstransit-caching/cachesettings) → [TestCacheSettings](../masstransit-caching/testcachesettings)

## Properties

### **CurrentTime**

```csharp
public DateTime CurrentTime { get; set; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **Capacity**

The number of items allowed in the cache. This isn't a hard limit, but the cache will shrink
 the cache size to be under the capacity when possible.

```csharp
public int Capacity { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **MaxAge**

The maximum length of time an unused item will remain in the cache

```csharp
public TimeSpan MaxAge { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **MinAge**

The minimum length of time an item will remain in the cache before it is eligible for removal

```csharp
public TimeSpan MinAge { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **NowProvider**

Provides the current time, which is used to manage item lifetime. Replace this to allow unit
 tests to move time forward quickly.

```csharp
public CurrentTimeProvider NowProvider { get; set; }
```

#### Property Value

[CurrentTimeProvider](../masstransit-caching/currenttimeprovider)<br/>

### **BucketCount**

The number of buckets to create per time slot (do NOT change this unless you're very smart)

```csharp
public int BucketCount { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **TimeSlots**

The number of time slots per bucket (do NOT change this unless you're very smart)

```csharp
public int TimeSlots { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **TestCacheSettings(Int32, Nullable\<TimeSpan\>, Nullable\<TimeSpan\>)**

```csharp
public TestCacheSettings(int capacity, Nullable<TimeSpan> minAge, Nullable<TimeSpan> maxAge)
```

#### Parameters

`capacity` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`minAge` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`maxAge` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
