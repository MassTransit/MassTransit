---

title: ReadWritePropertyCache<T>

---

# ReadWritePropertyCache\<T\>

Namespace: MassTransit.Internals

```csharp
public class ReadWritePropertyCache<T> : IReadWritePropertyCache<T>, IEnumerable<ReadWriteProperty<T>>, IEnumerable
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReadWritePropertyCache\<T\>](../masstransit-internals/readwritepropertycache-1)<br/>
Implements [IReadWritePropertyCache\<T\>](../masstransit-internals/ireadwritepropertycache-1), [IEnumerable\<ReadWriteProperty\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Properties

### **Item**

```csharp
public ReadWriteProperty<T> Item { get; }
```

#### Property Value

[ReadWriteProperty\<T\>](../masstransit-internals/readwriteproperty-1)<br/>

## Constructors

### **ReadWritePropertyCache()**

```csharp
public ReadWritePropertyCache()
```

### **ReadWritePropertyCache(Boolean)**

```csharp
public ReadWritePropertyCache(bool includeNonPublic)
```

#### Parameters

`includeNonPublic` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Methods

### **GetEnumerator()**

```csharp
public IEnumerator<ReadWriteProperty<T>> GetEnumerator()
```

#### Returns

[IEnumerator\<ReadWriteProperty\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerator-1)<br/>

### **TryGetValue(String, ReadWriteProperty\<T\>)**

```csharp
public bool TryGetValue(string key, out ReadWriteProperty<T> value)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [ReadWriteProperty\<T\>](../masstransit-internals/readwriteproperty-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetProperty(String, ReadWriteProperty\<T\>)**

```csharp
public bool TryGetProperty(string propertyName, out ReadWriteProperty<T> property)
```

#### Parameters

`propertyName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`property` [ReadWriteProperty\<T\>](../masstransit-internals/readwriteproperty-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Set(Expression\<Func\<T, Object\>\>, T, Object)**

```csharp
public void Set(Expression<Func<T, object>> propertyExpression, T instance, object value)
```

#### Parameters

`propertyExpression` Expression\<Func\<T, Object\>\><br/>

`instance` T<br/>

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

### **Get(Expression\<Func\<T, Object\>\>, T)**

```csharp
public object Get(Expression<Func<T, object>> propertyExpression, T instance)
```

#### Parameters

`propertyExpression` Expression\<Func\<T, Object\>\><br/>

`instance` T<br/>

#### Returns

[Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
