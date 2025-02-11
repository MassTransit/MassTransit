---

title: ICache<TKey, TValue>

---

# ICache\<TKey, TValue\>

Namespace: MassTransit.Internals.Caching

```csharp
public interface ICache<TKey, TValue>
```

#### Type Parameters

`TKey`<br/>

`TValue`<br/>

## Properties

### **Count**

```csharp
public abstract int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **HitRatio**

```csharp
public abstract double HitRatio { get; }
```

#### Property Value

[Double](https://learn.microsoft.com/en-us/dotnet/api/system.double)<br/>

### **Values**

```csharp
public abstract Task<IEnumerable<TValue>> Values { get; }
```

#### Property Value

[Task\<IEnumerable\<TValue\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

## Methods

### **GetOrAdd(TKey, MissingValueFactory\<TKey, TValue\>)**

```csharp
Task<TValue> GetOrAdd(TKey key, MissingValueFactory<TKey, TValue> missingValueFactory)
```

#### Parameters

`key` TKey<br/>

`missingValueFactory` [MissingValueFactory\<TKey, TValue\>](../masstransit-internals-caching/missingvaluefactory-2)<br/>

#### Returns

[Task\<TValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Get(TKey)**

```csharp
Task<TValue> Get(TKey key)
```

#### Parameters

`key` TKey<br/>

#### Returns

[Task\<TValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Remove(TKey)**

```csharp
Task<bool> Remove(TKey key)
```

#### Parameters

`key` TKey<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Clear()**

```csharp
Task Clear()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
