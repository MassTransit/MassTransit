---

title: CacheValue<TValue>

---

# CacheValue\<TValue\>

Namespace: MassTransit.Internals.Caching

```csharp
public class CacheValue<TValue> : ICacheValue<TValue>, ICacheValue
```

#### Type Parameters

`TValue`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CacheValue\<TValue\>](../masstransit-internals-caching/cachevalue-1)<br/>
Implements [ICacheValue\<TValue\>](../masstransit-internals-caching/icachevalue-1), [ICacheValue](../masstransit-internals-caching/icachevalue)

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

### **CacheValue(Action)**

```csharp
public CacheValue(Action remove)
```

#### Parameters

`remove` [Action](https://learn.microsoft.com/en-us/dotnet/api/system.action)<br/>

## Methods

### **GetValue(Func\<IPendingValue\<TValue\>\>)**

```csharp
public Task<TValue> GetValue(Func<IPendingValue<TValue>> pendingValueFactory)
```

#### Parameters

`pendingValueFactory` [Func\<IPendingValue\<TValue\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

#### Returns

[Task\<TValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Evict()**

```csharp
public Task Evict()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
