---

title: DefaultSagaRepositoryQueryContext<TSaga>

---

# DefaultSagaRepositoryQueryContext\<TSaga\>

Namespace: MassTransit.Saga

```csharp
public class DefaultSagaRepositoryQueryContext<TSaga> : ProxyPipeContext, SagaRepositoryQueryContext<TSaga>, QuerySagaRepositoryContext<TSaga>, PipeContext, IEnumerable<Guid>, IEnumerable
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ProxyPipeContext](../../masstransit-abstractions/masstransit-middleware/proxypipecontext) → [DefaultSagaRepositoryQueryContext\<TSaga\>](../masstransit-saga/defaultsagarepositoryquerycontext-1)<br/>
Implements [SagaRepositoryQueryContext\<TSaga\>](../masstransit-saga/sagarepositoryquerycontext-1), [QuerySagaRepositoryContext\<TSaga\>](../masstransit-saga/querysagarepositorycontext-1), [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [IEnumerable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

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

### **DefaultSagaRepositoryQueryContext(QuerySagaRepositoryContext\<TSaga\>, IList\<Guid\>)**

```csharp
public DefaultSagaRepositoryQueryContext(QuerySagaRepositoryContext<TSaga> queryContext, IList<Guid> results)
```

#### Parameters

`queryContext` [QuerySagaRepositoryContext\<TSaga\>](../masstransit-saga/querysagarepositorycontext-1)<br/>

`results` [IList\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1)<br/>

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
