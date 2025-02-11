---

title: SagaList<T>

---

# SagaList\<T\>

Namespace: MassTransit.Testing.Implementations

```csharp
public class SagaList<T> : AsyncElementList<ISagaInstance<T>>, IAsyncElementList<ISagaInstance<T>>, ISagaList<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AsyncElementList\<ISagaInstance\<T\>\>](../masstransit-testing-implementations/asyncelementlist-1) → [SagaList\<T\>](../masstransit-testing-implementations/sagalist-1)<br/>
Implements [IAsyncElementList\<ISagaInstance\<T\>\>](../masstransit-testing/iasyncelementlist-1), [ISagaList\<T\>](../masstransit-testing/isagalist-1)

## Constructors

### **SagaList(TimeSpan, CancellationToken)**

```csharp
public SagaList(TimeSpan timeout, CancellationToken testCompleted)
```

#### Parameters

`timeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`testCompleted` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Methods

### **Select(FilterDelegate\<T\>, CancellationToken)**

```csharp
public IEnumerable<ISagaInstance<T>> Select(FilterDelegate<T> filter, CancellationToken cancellationToken)
```

#### Parameters

`filter` [FilterDelegate\<T\>](../masstransit-testing/filterdelegate-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IEnumerable\<ISagaInstance\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Contains(Guid)**

```csharp
public T Contains(Guid sagaId)
```

#### Parameters

`sagaId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

#### Returns

T<br/>

### **SelectAsync(CancellationToken)**

```csharp
public IAsyncEnumerable<ISagaInstance<T>> SelectAsync(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IAsyncEnumerable\<ISagaInstance\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br/>

### **SelectAsync(FilterDelegate\<T\>, CancellationToken)**

```csharp
public IAsyncEnumerable<ISagaInstance<T>> SelectAsync(FilterDelegate<T> filter, CancellationToken cancellationToken)
```

#### Parameters

`filter` [FilterDelegate\<T\>](../masstransit-testing/filterdelegate-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[IAsyncEnumerable\<ISagaInstance\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br/>

### **Any(CancellationToken)**

```csharp
public Task<bool> Any(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Any(FilterDelegate\<T\>, CancellationToken)**

```csharp
public Task<bool> Any(FilterDelegate<T> filter, CancellationToken cancellationToken)
```

#### Parameters

`filter` [FilterDelegate\<T\>](../masstransit-testing/filterdelegate-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Add(SagaConsumeContext\<T\>)**

```csharp
public void Add(SagaConsumeContext<T> context)
```

#### Parameters

`context` [SagaConsumeContext\<T\>](../../masstransit-abstractions/masstransit/sagaconsumecontext-1)<br/>
