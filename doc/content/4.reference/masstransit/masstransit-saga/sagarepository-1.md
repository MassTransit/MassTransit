---

title: SagaRepository<TSaga>

---

# SagaRepository\<TSaga\>

Namespace: MassTransit.Saga

The modern saga repository, which can be used with any storage engine. Leverages the new interfaces for consume and query context.

```csharp
public class SagaRepository<TSaga> : ISagaRepository<TSaga>, IProbeSite, IQuerySagaRepository<TSaga>, ILoadSagaRepository<TSaga>
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SagaRepository\<TSaga\>](../masstransit-saga/sagarepository-1)<br/>
Implements [ISagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/isagarepository-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IQuerySagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/iquerysagarepository-1), [ILoadSagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/iloadsagarepository-1)

## Constructors

### **SagaRepository(ISagaRepositoryContextFactory\<TSaga\>, IQuerySagaRepositoryContextFactory\<TSaga\>, ILoadSagaRepositoryContextFactory\<TSaga\>)**

```csharp
public SagaRepository(ISagaRepositoryContextFactory<TSaga> repositoryContextFactory, IQuerySagaRepositoryContextFactory<TSaga> queryRepositoryContextFactory, ILoadSagaRepositoryContextFactory<TSaga> loadSagaRepositoryContextFactory)
```

#### Parameters

`repositoryContextFactory` [ISagaRepositoryContextFactory\<TSaga\>](../masstransit-saga/isagarepositorycontextfactory-1)<br/>

`queryRepositoryContextFactory` [IQuerySagaRepositoryContextFactory\<TSaga\>](../masstransit-saga/iquerysagarepositorycontextfactory-1)<br/>

`loadSagaRepositoryContextFactory` [ILoadSagaRepositoryContextFactory\<TSaga\>](../masstransit-saga/iloadsagarepositorycontextfactory-1)<br/>

## Methods

### **Load(Guid)**

```csharp
public Task<TSaga> Load(Guid correlationId)
```

#### Parameters

`correlationId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

#### Returns

[Task\<TSaga\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

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

### **Send\<T\>(ConsumeContext\<T\>, ISagaPolicy\<TSaga, T\>, IPipe\<SagaConsumeContext\<TSaga, T\>\>)**

```csharp
public Task Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`policy` [ISagaPolicy\<TSaga, T\>](../../masstransit-abstractions/masstransit/isagapolicy-2)<br/>

`next` [IPipe\<SagaConsumeContext\<TSaga, T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SendQuery\<T\>(ConsumeContext\<T\>, ISagaQuery\<TSaga\>, ISagaPolicy\<TSaga, T\>, IPipe\<SagaConsumeContext\<TSaga, T\>\>)**

```csharp
public Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`query` [ISagaQuery\<TSaga\>](../../masstransit-abstractions/masstransit/isagaquery-1)<br/>

`policy` [ISagaPolicy\<TSaga, T\>](../../masstransit-abstractions/masstransit/isagapolicy-2)<br/>

`next` [IPipe\<SagaConsumeContext\<TSaga, T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
