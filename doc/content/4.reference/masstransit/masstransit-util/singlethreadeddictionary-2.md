---

title: SingleThreadedDictionary<TKey, TValue>

---

# SingleThreadedDictionary\<TKey, TValue\>

Namespace: MassTransit.Util

```csharp
public class SingleThreadedDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
```

#### Type Parameters

`TKey`<br/>

`TValue`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SingleThreadedDictionary\<TKey, TValue\>](../masstransit-util/singlethreadeddictionary-2)<br/>
Implements [IReadOnlyDictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2), [IReadOnlyCollection\<KeyValuePair\<TKey, TValue\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlycollection-1), [IEnumerable\<KeyValuePair\<TKey, TValue\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Properties

### **Count**

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Item**

```csharp
public TValue Item { get; }
```

#### Property Value

TValue<br/>

### **Keys**

```csharp
public IEnumerable<TKey> Keys { get; }
```

#### Property Value

[IEnumerable\<TKey\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Values**

```csharp
public IEnumerable<TValue> Values { get; }
```

#### Property Value

[IEnumerable\<TValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

## Constructors

### **SingleThreadedDictionary(IEqualityComparer\<TKey\>)**

```csharp
public SingleThreadedDictionary(IEqualityComparer<TKey> comparer)
```

#### Parameters

`comparer` [IEqualityComparer\<TKey\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iequalitycomparer-1)<br/>

## Methods

### **GetEnumerator()**

```csharp
public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
```

#### Returns

[IEnumerator\<KeyValuePair\<TKey, TValue\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerator-1)<br/>

### **ContainsKey(TKey)**

```csharp
public bool ContainsKey(TKey key)
```

#### Parameters

`key` TKey<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetValue(TKey, TValue)**

```csharp
public bool TryGetValue(TKey key, out TValue value)
```

#### Parameters

`key` TKey<br/>

`value` TValue<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Clear()**

```csharp
public void Clear()
```

### **TryAdd(TKey, Func\<TKey, TValue\>)**

```csharp
public bool TryAdd(TKey key, Func<TKey, TValue> valueFactory)
```

#### Parameters

`key` TKey<br/>

`valueFactory` [Func\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryRemove(TKey, TValue)**

```csharp
public bool TryRemove(TKey key, out TValue value)
```

#### Parameters

`key` TKey<br/>

`value` TValue<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
