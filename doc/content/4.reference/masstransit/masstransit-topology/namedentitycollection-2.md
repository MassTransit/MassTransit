---

title: NamedEntityCollection<TEntity, THandle>

---

# NamedEntityCollection\<TEntity, THandle\>

Namespace: MassTransit.Topology

```csharp
public class NamedEntityCollection<TEntity, THandle> : EntityCollection<TEntity, THandle>, IEnumerable<TEntity>, IEnumerable
```

#### Type Parameters

`TEntity`<br/>

`THandle`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [EntityCollection\<TEntity, THandle\>](../masstransit-topology/entitycollection-2) → [NamedEntityCollection\<TEntity, THandle\>](../masstransit-topology/namedentitycollection-2)<br/>
Implements [IEnumerable\<TEntity\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Constructors

### **NamedEntityCollection(IEqualityComparer\<TEntity\>, IEqualityComparer\<TEntity\>)**

```csharp
public NamedEntityCollection(IEqualityComparer<TEntity> entityComparer, IEqualityComparer<TEntity> nameComparer)
```

#### Parameters

`entityComparer` [IEqualityComparer\<TEntity\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iequalitycomparer-1)<br/>

`nameComparer` [IEqualityComparer\<TEntity\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iequalitycomparer-1)<br/>

## Methods

### **GetOrAdd(TEntity)**

```csharp
public THandle GetOrAdd(TEntity entity)
```

#### Parameters

`entity` TEntity<br/>

#### Returns

THandle<br/>
