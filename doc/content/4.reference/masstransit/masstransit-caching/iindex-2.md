---

title: IIndex<TKey, TValue>

---

# IIndex\<TKey, TValue\>

Namespace: MassTransit.Caching

An index is used to access items in the cache quickly

```csharp
public interface IIndex<TKey, TValue>
```

#### Type Parameters

`TKey`<br/>

`TValue`<br/>

## Methods

### **Get(TKey, MissingValueFactory\<TKey, TValue\>)**

```csharp
Task<TValue> Get(TKey key, MissingValueFactory<TKey, TValue> missingValueFactory)
```

#### Parameters

`key` TKey<br/>

`missingValueFactory` [MissingValueFactory\<TKey, TValue\>](../masstransit-caching/missingvaluefactory-2)<br/>

#### Returns

[Task\<TValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Remove(TKey)**

Forcibly removes the item from the cache, but disposal may occur asynchronously.

```csharp
bool Remove(TKey key)
```

#### Parameters

`key` TKey<br/>
The value key

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
