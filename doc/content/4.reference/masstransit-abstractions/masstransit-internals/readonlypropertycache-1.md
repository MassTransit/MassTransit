---

title: ReadOnlyPropertyCache<T>

---

# ReadOnlyPropertyCache\<T\>

Namespace: MassTransit.Internals

```csharp
public class ReadOnlyPropertyCache<T> : IReadOnlyPropertyCache<T>, IEnumerable<ReadOnlyProperty<T>>, IEnumerable
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReadOnlyPropertyCache\<T\>](../masstransit-internals/readonlypropertycache-1)<br/>
Implements [IReadOnlyPropertyCache\<T\>](../masstransit-internals/ireadonlypropertycache-1), [IEnumerable\<ReadOnlyProperty\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Constructors

### **ReadOnlyPropertyCache()**

```csharp
public ReadOnlyPropertyCache()
```

## Methods

### **GetEnumerator()**

```csharp
public IEnumerator<ReadOnlyProperty<T>> GetEnumerator()
```

#### Returns

[IEnumerator\<ReadOnlyProperty\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerator-1)<br/>

### **TryGetValue(String, ReadOnlyProperty\<T\>)**

```csharp
public bool TryGetValue(string key, out ReadOnlyProperty<T> value)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [ReadOnlyProperty\<T\>](../masstransit-internals/readonlyproperty-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Get(Expression\<Func\<T, Object\>\>, T)**

```csharp
public object Get(Expression<Func<T, object>> propertyExpression, T instance)
```

#### Parameters

`propertyExpression` Expression\<Func\<T, Object\>\><br/>

`instance` T<br/>

#### Returns

[Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
