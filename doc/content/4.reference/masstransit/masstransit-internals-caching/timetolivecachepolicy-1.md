---

title: TimeToLiveCachePolicy<TValue>

---

# TimeToLiveCachePolicy\<TValue\>

Namespace: MassTransit.Internals.Caching

```csharp
public class TimeToLiveCachePolicy<TValue> : ICachePolicy<TValue, ITimeToLiveCacheValue<TValue>>
```

#### Type Parameters

`TValue`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TimeToLiveCachePolicy\<TValue\>](../masstransit-internals-caching/timetolivecachepolicy-1)<br/>
Implements [ICachePolicy\<TValue, ITimeToLiveCacheValue\<TValue\>\>](../masstransit-internals-caching/icachepolicy-2)

## Constructors

### **TimeToLiveCachePolicy(TimeSpan)**

```csharp
public TimeToLiveCachePolicy(TimeSpan timeToLive)
```

#### Parameters

`timeToLive` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

## Methods

### **CreateValue(Action)**

```csharp
public ITimeToLiveCacheValue<TValue> CreateValue(Action remove)
```

#### Parameters

`remove` [Action](https://learn.microsoft.com/en-us/dotnet/api/system.action)<br/>

#### Returns

[ITimeToLiveCacheValue\<TValue\>](../masstransit-internals-caching/itimetolivecachevalue-1)<br/>

### **IsValid(ITimeToLiveCacheValue\<TValue\>)**

```csharp
public bool IsValid(ITimeToLiveCacheValue<TValue> value)
```

#### Parameters

`value` [ITimeToLiveCacheValue\<TValue\>](../masstransit-internals-caching/itimetolivecachevalue-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **CheckValue(ITimeToLiveCacheValue\<TValue\>)**

```csharp
public int CheckValue(ITimeToLiveCacheValue<TValue> value)
```

#### Parameters

`value` [ITimeToLiveCacheValue\<TValue\>](../masstransit-internals-caching/itimetolivecachevalue-1)<br/>

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
