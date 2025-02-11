---

title: LoadSagaRepository<TSaga>

---

# LoadSagaRepository\<TSaga\>

Namespace: MassTransit.Saga

The modern query saga repository, which can be used with any storage engine. Leverages the new interfaces for query context.

```csharp
public class LoadSagaRepository<TSaga> : ILoadSagaRepository<TSaga>, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [LoadSagaRepository\<TSaga\>](../masstransit-saga/loadsagarepository-1)<br/>
Implements [ILoadSagaRepository\<TSaga\>](../../masstransit-abstractions/masstransit/iloadsagarepository-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **LoadSagaRepository(ILoadSagaRepositoryContextFactory\<TSaga\>)**

```csharp
public LoadSagaRepository(ILoadSagaRepositoryContextFactory<TSaga> repositoryContextFactory)
```

#### Parameters

`repositoryContextFactory` [ILoadSagaRepositoryContextFactory\<TSaga\>](../masstransit-saga/iloadsagarepositorycontextfactory-1)<br/>

## Methods

### **Load(Guid)**

```csharp
public Task<TSaga> Load(Guid correlationId)
```

#### Parameters

`correlationId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

#### Returns

[Task\<TSaga\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
