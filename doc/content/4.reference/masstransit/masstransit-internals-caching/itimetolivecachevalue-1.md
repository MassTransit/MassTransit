---

title: ITimeToLiveCacheValue<TValue>

---

# ITimeToLiveCacheValue\<TValue\>

Namespace: MassTransit.Internals.Caching

```csharp
public interface ITimeToLiveCacheValue<TValue> : ICacheValue<TValue>, ICacheValue
```

#### Type Parameters

`TValue`<br/>

Implements [ICacheValue\<TValue\>](../masstransit-internals-caching/icachevalue-1), [ICacheValue](../masstransit-internals-caching/icachevalue)

## Properties

### **Timestamp**

```csharp
public abstract long Timestamp { get; }
```

#### Property Value

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>
