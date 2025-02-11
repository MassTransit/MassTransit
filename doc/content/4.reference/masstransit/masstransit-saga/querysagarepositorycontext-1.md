---

title: QuerySagaRepositoryContext<TSaga>

---

# QuerySagaRepositoryContext\<TSaga\>

Namespace: MassTransit.Saga

```csharp
public interface QuerySagaRepositoryContext<TSaga> : PipeContext
```

#### Type Parameters

`TSaga`<br/>

Implements [PipeContext](../../masstransit-abstractions/masstransit/pipecontext)

## Methods

### **Query(ISagaQuery\<TSaga\>, CancellationToken)**

Query saga instances

```csharp
Task<SagaRepositoryQueryContext<TSaga>> Query(ISagaQuery<TSaga> query, CancellationToken cancellationToken)
```

#### Parameters

`query` [ISagaQuery\<TSaga\>](../../masstransit-abstractions/masstransit/isagaquery-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<SagaRepositoryQueryContext\<TSaga\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
