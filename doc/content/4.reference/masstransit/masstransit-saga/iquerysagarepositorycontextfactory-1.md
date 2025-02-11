---

title: IQuerySagaRepositoryContextFactory<TSaga>

---

# IQuerySagaRepositoryContextFactory\<TSaga\>

Namespace: MassTransit.Saga



```csharp
public interface IQuerySagaRepositoryContextFactory<TSaga> : IProbeSite
```

#### Type Parameters

`TSaga`<br/>

Implements [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Methods

### **Execute\<T\>(Func\<QuerySagaRepositoryContext\<TSaga\>, Task\<T\>\>, CancellationToken)**

Create a [QuerySagaRepositoryContext\<TSaga\>](../masstransit-saga/querysagarepositorycontext-1) and send it to the next pipe.

```csharp
Task<T> Execute<T>(Func<QuerySagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`asyncMethod` [Func\<QuerySagaRepositoryContext\<TSaga\>, Task\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
