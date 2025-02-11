---

title: QuerySagaRepository<TSaga>

---

# QuerySagaRepository\<TSaga\>

Namespace: MassTransit.Saga

The modern query saga repository, which can be used with any storage engine. Leverages the new interfaces for query context.

```csharp
public class QuerySagaRepository<TSaga> : IQuerySagaRepository<TSaga>, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [QuerySagaRepository\<TSaga\>](../masstransit-saga/querysagarepository-1)<br/>
Implements [IQuerySagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/iquerysagarepository-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **QuerySagaRepository(IQuerySagaRepositoryContextFactory\<TSaga\>)**

```csharp
public QuerySagaRepository(IQuerySagaRepositoryContextFactory<TSaga> repositoryContextFactory)
```

#### Parameters

`repositoryContextFactory` [IQuerySagaRepositoryContextFactory\<TSaga\>](../masstransit-saga/iquerysagarepositorycontextfactory-1)<br/>

## Methods

### **Find(ISagaQuery\<TSaga\>)**

```csharp
public Task<IEnumerable<Guid>> Find(ISagaQuery<TSaga> query)
```

#### Parameters

`query` [ISagaQuery\<TSaga\>](../../masstransit-abstractions/masstransit/isagaquery-1)<br/>

#### Returns

[Task\<IEnumerable\<Guid\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
