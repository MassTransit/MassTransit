---

title: InMemorySagaRepositoryContext<TSaga>

---

# InMemorySagaRepositoryContext\<TSaga\>

Namespace: MassTransit.Saga

```csharp
public class InMemorySagaRepositoryContext<TSaga> : BasePipeContext, PipeContext, QuerySagaRepositoryContext<TSaga>, LoadSagaRepositoryContext<TSaga>
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BasePipeContext](../../masstransit-abstractions/masstransit-middleware/basepipecontext) → [InMemorySagaRepositoryContext\<TSaga\>](../masstransit-saga/inmemorysagarepositorycontext-1)<br/>
Implements [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [QuerySagaRepositoryContext\<TSaga\>](../masstransit-saga/querysagarepositorycontext-1), [LoadSagaRepositoryContext\<TSaga\>](../masstransit-saga/loadsagarepositorycontext-1)

## Properties

### **CancellationToken**

```csharp
public CancellationToken CancellationToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Constructors

### **InMemorySagaRepositoryContext(IndexedSagaDictionary\<TSaga\>, CancellationToken)**

```csharp
public InMemorySagaRepositoryContext(IndexedSagaDictionary<TSaga> sagas, CancellationToken cancellationToken)
```

#### Parameters

`sagas` [IndexedSagaDictionary\<TSaga\>](../masstransit-saga/indexedsagadictionary-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Methods

### **Load(Guid)**

```csharp
public Task<TSaga> Load(Guid correlationId)
```

#### Parameters

`correlationId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

#### Returns

[Task\<TSaga\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Query(ISagaQuery\<TSaga\>, CancellationToken)**

```csharp
public Task<SagaRepositoryQueryContext<TSaga>> Query(ISagaQuery<TSaga> query, CancellationToken cancellationToken)
```

#### Parameters

`query` [ISagaQuery\<TSaga\>](../../masstransit-abstractions/masstransit/isagaquery-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<SagaRepositoryQueryContext\<TSaga\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
