---

title: IValueTracker<TValue, TCacheValue>

---

# IValueTracker\<TValue, TCacheValue\>

Namespace: MassTransit.Internals.Caching

```csharp
public interface IValueTracker<TValue, TCacheValue>
```

#### Type Parameters

`TValue`<br/>

`TCacheValue`<br/>

## Properties

### **Capacity**

```csharp
public abstract int Capacity { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Methods

### **Add(TCacheValue)**

```csharp
Task Add(TCacheValue value)
```

#### Parameters

`value` TCacheValue<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ReBucket(IBucket\<TValue, TCacheValue\>, TCacheValue)**

```csharp
Task ReBucket(IBucket<TValue, TCacheValue> source, TCacheValue value)
```

#### Parameters

`source` [IBucket\<TValue, TCacheValue\>](../masstransit-internals-caching/ibucket-2)<br/>

`value` TCacheValue<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Clear()**

```csharp
Task Clear()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
