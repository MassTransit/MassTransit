---

title: ValueTracker<TValue, TCacheValue>

---

# ValueTracker\<TValue, TCacheValue\>

Namespace: MassTransit.Internals.Caching

```csharp
public class ValueTracker<TValue, TCacheValue> : IValueTracker<TValue, TCacheValue>
```

#### Type Parameters

`TValue`<br/>

`TCacheValue`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueTracker\<TValue, TCacheValue\>](../masstransit-internals-caching/valuetracker-2)<br/>
Implements [IValueTracker\<TValue, TCacheValue\>](../masstransit-internals-caching/ivaluetracker-2)

## Properties

### **Capacity**

```csharp
public int Capacity { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **ValueTracker(ICachePolicy\<TValue, TCacheValue\>, Int32)**

```csharp
public ValueTracker(ICachePolicy<TValue, TCacheValue> policy, int capacity)
```

#### Parameters

`policy` [ICachePolicy\<TValue, TCacheValue\>](../masstransit-internals-caching/icachepolicy-2)<br/>

`capacity` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Methods

### **Add(TCacheValue)**

```csharp
public Task Add(TCacheValue value)
```

#### Parameters

`value` TCacheValue<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Clear()**

```csharp
public Task Clear()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ReBucket(IBucket\<TValue, TCacheValue\>, TCacheValue)**

```csharp
public Task ReBucket(IBucket<TValue, TCacheValue> source, TCacheValue value)
```

#### Parameters

`source` [IBucket\<TValue, TCacheValue\>](../masstransit-internals-caching/ibucket-2)<br/>

`value` TCacheValue<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
