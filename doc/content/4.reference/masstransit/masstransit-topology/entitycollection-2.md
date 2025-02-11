---

title: EntityCollection<TEntity, THandle>

---

# EntityCollection\<TEntity, THandle\>

Namespace: MassTransit.Topology

```csharp
public class EntityCollection<TEntity, THandle> : IEnumerable<TEntity>, IEnumerable
```

#### Type Parameters

`TEntity`<br/>

`THandle`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [EntityCollection\<TEntity, THandle\>](../masstransit-topology/entitycollection-2)<br/>
Implements [IEnumerable\<TEntity\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Constructors

### **EntityCollection(IEqualityComparer\<TEntity\>)**

```csharp
public EntityCollection(IEqualityComparer<TEntity> entityComparer)
```

#### Parameters

`entityComparer` [IEqualityComparer\<TEntity\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iequalitycomparer-1)<br/>

## Methods

### **GetEnumerator()**

```csharp
public IEnumerator<TEntity> GetEnumerator()
```

#### Returns

[IEnumerator\<TEntity\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerator-1)<br/>

### **GetOrAdd(TEntity)**

```csharp
public THandle GetOrAdd(TEntity entity)
```

#### Parameters

`entity` TEntity<br/>

#### Returns

THandle<br/>

### **Get(THandle)**

```csharp
public TEntity Get(THandle entityHandle)
```

#### Parameters

`entityHandle` THandle<br/>

#### Returns

TEntity<br/>
