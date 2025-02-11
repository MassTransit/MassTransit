---

title: ILoadSagaRepositoryContextFactory<TSaga>

---

# ILoadSagaRepositoryContextFactory\<TSaga\>

Namespace: MassTransit.Saga



```csharp
public interface ILoadSagaRepositoryContextFactory<TSaga> : IProbeSite
```

#### Type Parameters

`TSaga`<br/>

Implements [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Methods

### **Execute\<T\>(Func\<LoadSagaRepositoryContext\<TSaga\>, Task\<T\>\>, CancellationToken)**

Create a [LoadSagaRepositoryContext\<TSaga\>](../masstransit-saga/loadsagarepositorycontext-1) and send it to the next pipe.

```csharp
Task<T> Execute<T>(Func<LoadSagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`asyncMethod` [Func\<LoadSagaRepositoryContext\<TSaga\>, Task\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
