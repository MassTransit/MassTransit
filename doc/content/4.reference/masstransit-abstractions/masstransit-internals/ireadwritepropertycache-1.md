---

title: IReadWritePropertyCache<T>

---

# IReadWritePropertyCache\<T\>

Namespace: MassTransit.Internals

```csharp
public interface IReadWritePropertyCache<T> : IEnumerable<ReadWriteProperty<T>>, IEnumerable
```

#### Type Parameters

`T`<br/>

Implements [IEnumerable\<ReadWriteProperty\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Properties

### **Item**

```csharp
public abstract ReadWriteProperty<T> Item { get; }
```

#### Property Value

[ReadWriteProperty\<T\>](../masstransit-internals/readwriteproperty-1)<br/>

## Methods

### **TryGetValue(String, ReadWriteProperty\<T\>)**

```csharp
bool TryGetValue(string key, out ReadWriteProperty<T> value)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [ReadWriteProperty\<T\>](../masstransit-internals/readwriteproperty-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetProperty(String, ReadWriteProperty\<T\>)**

```csharp
bool TryGetProperty(string propertyName, out ReadWriteProperty<T> property)
```

#### Parameters

`propertyName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`property` [ReadWriteProperty\<T\>](../masstransit-internals/readwriteproperty-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
