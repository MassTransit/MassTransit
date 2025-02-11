---

title: ISagaList<T>

---

# ISagaList\<T\>

Namespace: MassTransit.Testing

```csharp
public interface ISagaList<T> : IAsyncElementList<ISagaInstance<T>>
```

#### Type Parameters

`T`<br/>

Implements [IAsyncElementList\<ISagaInstance\<T\>\>](../masstransit-testing/iasyncelementlist-1)

## Methods

### **Select(FilterDelegate\<T\>, CancellationToken)**

```csharp
IEnumerable<ISagaInstance<T>> Select(FilterDelegate<T> filter, CancellationToken cancellationToken)
```

#### Parameters

`filter` [FilterDelegate\<T\>](../masstransit-testing/filterdelegate-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IEnumerable\<ISagaInstance\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Contains(Guid)**

```csharp
T Contains(Guid sagaId)
```

#### Parameters

`sagaId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

#### Returns

T<br/>

### **SelectAsync(CancellationToken)**

```csharp
IAsyncEnumerable<ISagaInstance<T>> SelectAsync(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IAsyncEnumerable\<ISagaInstance\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br/>

### **SelectAsync(FilterDelegate\<T\>, CancellationToken)**

```csharp
IAsyncEnumerable<ISagaInstance<T>> SelectAsync(FilterDelegate<T> filter, CancellationToken cancellationToken)
```

#### Parameters

`filter` [FilterDelegate\<T\>](../masstransit-testing/filterdelegate-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IAsyncEnumerable\<ISagaInstance\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br/>

### **Any(CancellationToken)**

```csharp
Task<bool> Any(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Any(FilterDelegate\<T\>, CancellationToken)**

```csharp
Task<bool> Any(FilterDelegate<T> filter, CancellationToken cancellationToken)
```

#### Parameters

`filter` [FilterDelegate\<T\>](../masstransit-testing/filterdelegate-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
