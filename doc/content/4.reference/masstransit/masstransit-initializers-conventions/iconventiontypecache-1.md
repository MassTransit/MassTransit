---

title: IConventionTypeCache<T>

---

# IConventionTypeCache\<T\>

Namespace: MassTransit.Initializers.Conventions

A convention cache for type specified, which converts to the generic type requested

```csharp
public interface IConventionTypeCache<T>
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
