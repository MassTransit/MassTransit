---

title: ISagaRepositoryContextFactory<TSaga>

---

# ISagaRepositoryContextFactory\<TSaga\>

Namespace: MassTransit.Saga



```csharp
public interface ISagaRepositoryContextFactory<TSaga> : IProbeSite
```

#### Type Parameters

`TSaga`<br/>

Implements [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Methods

### **Send\<T\>(ConsumeContext\<T\>, IPipe\<SagaRepositoryContext\<TSaga, T\>\>)**

Create a [SagaRepositoryContext\<TSaga, TMessage\>](../masstransit-saga/sagarepositorycontext-2) and send it to the next pipe.

```csharp
Task Send<T>(ConsumeContext<T> context, IPipe<SagaRepositoryContext<TSaga, T>> next)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`next` [IPipe\<SagaRepositoryContext\<TSaga, T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SendQuery\<T\>(ConsumeContext\<T\>, ISagaQuery\<TSaga\>, IPipe\<SagaRepositoryQueryContext\<TSaga, T\>\>)**

Create a [SagaRepositoryQueryContext\<TSaga, T\>](../masstransit-saga/sagarepositoryquerycontext-2) and send it to the next pipe.

```csharp
Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, IPipe<SagaRepositoryQueryContext<TSaga, T>> next)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`query` [ISagaQuery\<TSaga\>](../../masstransit-abstractions/masstransit/isagaquery-1)<br/>

`next` [IPipe\<SagaRepositoryQueryContext\<TSaga, T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
