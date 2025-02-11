---

title: SagaRepositoryQueryContext<TSaga>

---

# SagaRepositoryQueryContext\<TSaga\>

Namespace: MassTransit.Saga

```csharp
public interface SagaRepositoryQueryContext<TSaga> : QuerySagaRepositoryContext<TSaga>, PipeContext, IEnumerable<Guid>, IEnumerable
```

#### Type Parameters

`TSaga`<br/>

Implements [QuerySagaRepositoryContext\<TSaga\>](../masstransit-saga/querysagarepositorycontext-1), [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [IEnumerable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Properties

### **Count**

The number of matching saga instances

```csharp
public abstract int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
