---

title: UsageCachePolicy<TValue>

---

# UsageCachePolicy\<TValue\>

Namespace: MassTransit.Internals.Caching

```csharp
public class UsageCachePolicy<TValue> : ICachePolicy<TValue, CacheValue<TValue>>
```

#### Type Parameters

`TValue`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [UsageCachePolicy\<TValue\>](../masstransit-internals-caching/usagecachepolicy-1)<br/>
Implements [ICachePolicy\<TValue, CacheValue\<TValue\>\>](../masstransit-internals-caching/icachepolicy-2)

## Constructors

### **UsageCachePolicy()**

```csharp
public UsageCachePolicy()
```

## Methods

### **CreateValue(Action)**

```csharp
public CacheValue<TValue> CreateValue(Action remove)
```

#### Parameters

`remove` [Action](https://learn.microsoft.com/en-us/dotnet/api/system.action)<br/>

#### Returns

[CacheValue\<TValue\>](../masstransit-internals-caching/cachevalue-1)<br/>

### **IsValid(CacheValue\<TValue\>)**

```csharp
public bool IsValid(CacheValue<TValue> value)
```

#### Parameters

`value` [CacheValue\<TValue\>](../masstransit-internals-caching/cachevalue-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **CheckValue(CacheValue\<TValue\>)**

```csharp
public int CheckValue(CacheValue<TValue> value)
```

#### Parameters

`value` [CacheValue\<TValue\>](../masstransit-internals-caching/cachevalue-1)<br/>

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
