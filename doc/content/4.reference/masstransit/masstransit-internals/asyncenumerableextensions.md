---

title: AsyncEnumerableExtensions

---

# AsyncEnumerableExtensions

Namespace: MassTransit.Internals

```csharp
public static class AsyncEnumerableExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AsyncEnumerableExtensions](../masstransit-internals/asyncenumerableextensions)

## Methods

### **ToListAsync\<TElement\>(IAsyncEnumerable\<TElement\>)**

```csharp
public static Task<IList<TElement>> ToListAsync<TElement>(IAsyncEnumerable<TElement> elements)
```

#### Type Parameters

`TElement`<br/>

#### Parameters

`elements` [IAsyncEnumerable\<TElement\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br/>

#### Returns

[Task\<IList\<TElement\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ToListAsync\<TElement\>(IAsyncEnumerable\<TElement\>, CancellationToken)**

```csharp
public static Task<IList<TElement>> ToListAsync<TElement>(IAsyncEnumerable<TElement> elements, CancellationToken cancellationToken)
```

#### Type Parameters

`TElement`<br/>

#### Parameters

`elements` [IAsyncEnumerable\<TElement\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<IList\<TElement\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
