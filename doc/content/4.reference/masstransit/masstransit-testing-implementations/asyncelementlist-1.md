---

title: AsyncElementList<TElement>

---

# AsyncElementList\<TElement\>

Namespace: MassTransit.Testing.Implementations

```csharp
public abstract class AsyncElementList<TElement> : IAsyncElementList<TElement>
```

#### Type Parameters

`TElement`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AsyncElementList\<TElement\>](../masstransit-testing-implementations/asyncelementlist-1)<br/>
Implements [IAsyncElementList\<TElement\>](../masstransit-testing/iasyncelementlist-1)

## Methods

### **SelectAsync(FilterDelegate\<TElement\>, CancellationToken)**

```csharp
public IAsyncEnumerable<TElement> SelectAsync(FilterDelegate<TElement> filter, CancellationToken cancellationToken)
```

#### Parameters

`filter` [FilterDelegate\<TElement\>](../masstransit-testing/filterdelegate-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IAsyncEnumerable\<TElement\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br/>

### **Any(FilterDelegate\<TElement\>, CancellationToken)**

```csharp
public Task<bool> Any(FilterDelegate<TElement> filter, CancellationToken cancellationToken)
```

#### Parameters

`filter` [FilterDelegate\<TElement\>](../masstransit-testing/filterdelegate-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Select(FilterDelegate\<TElement\>, CancellationToken)**

```csharp
public IEnumerable<TElement> Select(FilterDelegate<TElement> filter, CancellationToken cancellationToken)
```

#### Parameters

`filter` [FilterDelegate\<TElement\>](../masstransit-testing/filterdelegate-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IEnumerable\<TElement\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Add(TElement)**

```csharp
protected void Add(TElement context)
```

#### Parameters

`context` TElement<br/>
