---

title: Bucket<TValue, TCacheValue>

---

# Bucket\<TValue, TCacheValue\>

Namespace: MassTransit.Internals.Caching

```csharp
public class Bucket<TValue, TCacheValue> : IBucket<TValue, TCacheValue>
```

#### Type Parameters

`TValue`<br/>

`TCacheValue`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Bucket\<TValue, TCacheValue\>](../masstransit-internals-caching/bucket-2)<br/>
Implements [IBucket\<TValue, TCacheValue\>](../masstransit-internals-caching/ibucket-2)

## Properties

### **Count**

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Capacity**

```csharp
public int Capacity { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **Bucket(IValueTracker\<TValue, TCacheValue\>, Int32)**

```csharp
public Bucket(IValueTracker<TValue, TCacheValue> valueTracker, int capacity)
```

#### Parameters

`valueTracker` [IValueTracker\<TValue, TCacheValue\>](../masstransit-internals-caching/ivaluetracker-2)<br/>

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
