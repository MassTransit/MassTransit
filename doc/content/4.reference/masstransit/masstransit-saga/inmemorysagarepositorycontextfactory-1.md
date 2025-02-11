---

title: InMemorySagaRepositoryContextFactory<TSaga>

---

# InMemorySagaRepositoryContextFactory\<TSaga\>

Namespace: MassTransit.Saga

Supports the InMemorySagaRepository

```csharp
public class InMemorySagaRepositoryContextFactory<TSaga> : ISagaRepositoryContextFactory<TSaga>, IProbeSite, IQuerySagaRepositoryContextFactory<TSaga>, ILoadSagaRepositoryContextFactory<TSaga>
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InMemorySagaRepositoryContextFactory\<TSaga\>](../masstransit-saga/inmemorysagarepositorycontextfactory-1)<br/>
Implements [ISagaRepositoryContextFactory\<TSaga\>](../masstransit-saga/isagarepositorycontextfactory-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IQuerySagaRepositoryContextFactory\<TSaga\>](../masstransit-saga/iquerysagarepositorycontextfactory-1), [ILoadSagaRepositoryContextFactory\<TSaga\>](../masstransit-saga/iloadsagarepositorycontextfactory-1)

## Constructors

### **InMemorySagaRepositoryContextFactory(IndexedSagaDictionary\<TSaga\>, ISagaConsumeContextFactory\<IndexedSagaDictionary\<TSaga\>, TSaga\>)**

```csharp
public InMemorySagaRepositoryContextFactory(IndexedSagaDictionary<TSaga> sagas, ISagaConsumeContextFactory<IndexedSagaDictionary<TSaga>, TSaga> factory)
```

#### Parameters

`sagas` [IndexedSagaDictionary\<TSaga\>](../masstransit-saga/indexedsagadictionary-1)<br/>

`factory` [ISagaConsumeContextFactory\<IndexedSagaDictionary\<TSaga\>, TSaga\>](../masstransit-saga/isagaconsumecontextfactory-2)<br/>

## Methods

### **Execute\<T\>(Func\<LoadSagaRepositoryContext\<TSaga\>, Task\<T\>\>, CancellationToken)**

```csharp
public Task<T> Execute<T>(Func<LoadSagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`asyncMethod` [Func\<LoadSagaRepositoryContext\<TSaga\>, Task\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Execute\<T\>(Func\<QuerySagaRepositoryContext\<TSaga\>, Task\<T\>\>, CancellationToken)**

```csharp
public Task<T> Execute<T>(Func<QuerySagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`asyncMethod` [Func\<QuerySagaRepositoryContext\<TSaga\>, Task\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **Send\<T\>(ConsumeContext\<T\>, IPipe\<SagaRepositoryContext\<TSaga, T\>\>)**

```csharp
public Task Send<T>(ConsumeContext<T> context, IPipe<SagaRepositoryContext<TSaga, T>> next)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`next` [IPipe\<SagaRepositoryContext\<TSaga, T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SendQuery\<T\>(ConsumeContext\<T\>, ISagaQuery\<TSaga\>, IPipe\<SagaRepositoryQueryContext\<TSaga, T\>\>)**

```csharp
public Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, IPipe<SagaRepositoryQueryContext<TSaga, T>> next)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`query` [ISagaQuery\<TSaga\>](../../masstransit-abstractions/masstransit/isagaquery-1)<br/>

`next` [IPipe\<SagaRepositoryQueryContext\<TSaga, T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
