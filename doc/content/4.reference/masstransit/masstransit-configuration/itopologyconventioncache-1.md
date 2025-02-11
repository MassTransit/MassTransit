---

title: ITopologyConventionCache<T>

---

# ITopologyConventionCache\<T\>

Namespace: MassTransit.Configuration

A convention cache for type specified, which converts to the generic type requested

```csharp
public interface ITopologyConventionCache<T>
```

#### Type Parameters

`T`<br/>

## Methods

### **GetOrAdd\<TKey, TResult\>()**

Returns the cached item for the specified type key, creating a new value
 if one has not yet been created.

```csharp
TResult GetOrAdd<TKey, TResult>()
```

#### Type Parameters

`TKey`<br/>

`TResult`<br/>

#### Returns

TResult<br/>
