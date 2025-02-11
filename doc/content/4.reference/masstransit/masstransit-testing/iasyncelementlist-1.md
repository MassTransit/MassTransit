---

title: IAsyncElementList<TElement>

---

# IAsyncElementList\<TElement\>

Namespace: MassTransit.Testing

```csharp
public interface IAsyncElementList<TElement>
```

#### Type Parameters

`TElement`<br/>

## Methods

### **Select(FilterDelegate\<TElement\>, CancellationToken)**

```csharp
IEnumerable<TElement> Select(FilterDelegate<TElement> filter, CancellationToken cancellationToken)
```

#### Parameters

`filter` [FilterDelegate\<TElement\>](../masstransit-testing/filterdelegate-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IEnumerable\<TElement\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **SelectAsync(FilterDelegate\<TElement\>, CancellationToken)**

```csharp
IAsyncEnumerable<TElement> SelectAsync(FilterDelegate<TElement> filter, CancellationToken cancellationToken)
```

#### Parameters

`filter` [FilterDelegate\<TElement\>](../masstransit-testing/filterdelegate-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IAsyncEnumerable\<TElement\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br/>

### **Any(FilterDelegate\<TElement\>, CancellationToken)**

```csharp
Task<bool> Any(FilterDelegate<TElement> filter, CancellationToken cancellationToken)
```

#### Parameters

`filter` [FilterDelegate\<TElement\>](../masstransit-testing/filterdelegate-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
