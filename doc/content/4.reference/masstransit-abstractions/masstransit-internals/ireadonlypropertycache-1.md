---

title: IReadOnlyPropertyCache<T>

---

# IReadOnlyPropertyCache\<T\>

Namespace: MassTransit.Internals

```csharp
public interface IReadOnlyPropertyCache<T> : IEnumerable<ReadOnlyProperty<T>>, IEnumerable
```

#### Type Parameters

`T`<br/>

Implements [IEnumerable\<ReadOnlyProperty\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Methods

### **TryGetValue(String, ReadOnlyProperty\<T\>)**

```csharp
bool TryGetValue(string key, out ReadOnlyProperty<T> value)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [ReadOnlyProperty\<T\>](../masstransit-internals/readonlyproperty-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
