---

title: TimeToLiveCacheValue<TValue>

---

# TimeToLiveCacheValue\<TValue\>

Namespace: MassTransit.Internals.Caching

```csharp
public class TimeToLiveCacheValue<TValue> : CacheValue<TValue>, ICacheValue<TValue>, ICacheValue, ITimeToLiveCacheValue<TValue>
```

#### Type Parameters

`TValue`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CacheValue\<TValue\>](../masstransit-internals-caching/cachevalue-1) → [TimeToLiveCacheValue\<TValue\>](../masstransit-internals-caching/timetolivecachevalue-1)<br/>
Implements [ICacheValue\<TValue\>](../masstransit-internals-caching/icachevalue-1), [ICacheValue](../masstransit-internals-caching/icachevalue), [ITimeToLiveCacheValue\<TValue\>](../masstransit-internals-caching/itimetolivecachevalue-1)

## Properties

### **Timestamp**

```csharp
public long Timestamp { get; set; }
```

#### Property Value

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

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

### **IsFaultedOrCanceled**

```csharp
public bool IsFaultedOrCanceled { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Usage**

```csharp
public int Usage { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **TimeToLiveCacheValue(Action, Int64)**

```csharp
public TimeToLiveCacheValue(Action remove, long timestamp)
```

#### Parameters

`remove` [Action](https://learn.microsoft.com/en-us/dotnet/api/system.action)<br/>

`timestamp` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>
