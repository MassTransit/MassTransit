---

title: LoadedSagaRepositoryQueryContext<TSaga>

---

# LoadedSagaRepositoryQueryContext\<TSaga\>

Namespace: MassTransit.Saga

For queries that load the actual saga instances

```csharp
public class LoadedSagaRepositoryQueryContext<TSaga> : BasePipeContext, PipeContext, SagaRepositoryQueryContext<TSaga>, QuerySagaRepositoryContext<TSaga>, IEnumerable<Guid>, IEnumerable
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BasePipeContext](../../masstransit-abstractions/masstransit-middleware/basepipecontext) → [LoadedSagaRepositoryQueryContext\<TSaga\>](../masstransit-saga/loadedsagarepositoryquerycontext-1)<br/>
Implements [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [SagaRepositoryQueryContext\<TSaga\>](../masstransit-saga/sagarepositoryquerycontext-1), [QuerySagaRepositoryContext\<TSaga\>](../masstransit-saga/querysagarepositorycontext-1), [IEnumerable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Properties

### **Count**

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **CancellationToken**

```csharp
public CancellationToken CancellationToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Constructors

### **LoadedSagaRepositoryQueryContext(QuerySagaRepositoryContext\<TSaga\>, IEnumerable\<TSaga\>)**

```csharp
public LoadedSagaRepositoryQueryContext(QuerySagaRepositoryContext<TSaga> querySagaRepositoryContext, IEnumerable<TSaga> instances)
```

#### Parameters

`querySagaRepositoryContext` [QuerySagaRepositoryContext\<TSaga\>](../masstransit-saga/querysagarepositorycontext-1)<br/>

`instances` [IEnumerable\<TSaga\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

## Methods

### **Query(ISagaQuery\<TSaga\>, CancellationToken)**

```csharp
public Task<SagaRepositoryQueryContext<TSaga>> Query(ISagaQuery<TSaga> query, CancellationToken cancellationToken)
```

#### Parameters

`query` [ISagaQuery\<TSaga\>](../../masstransit-abstractions/masstransit/isagaquery-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<SagaRepositoryQueryContext\<TSaga\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **GetEnumerator()**

```csharp
public IEnumerator<Guid> GetEnumerator()
```

#### Returns

[IEnumerator\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerator-1)<br/>
