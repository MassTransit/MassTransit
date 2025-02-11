---

title: ISagaConsumeContextFactory<TContext, TSaga>

---

# ISagaConsumeContextFactory\<TContext, TSaga\>

Namespace: MassTransit.Saga

Creates the  as needed by the [QuerySagaRepositoryContext\<TSaga\>](../masstransit-saga/querysagarepositorycontext-1).

```csharp
public interface ISagaConsumeContextFactory<TContext, TSaga>
```

#### Type Parameters

`TContext`<br/>

`TSaga`<br/>

## Methods

### **CreateSagaConsumeContext\<T\>(TContext, ConsumeContext\<T\>, TSaga, SagaConsumeContextMode)**

Create a new .

```csharp
Task<SagaConsumeContext<TSaga, T>> CreateSagaConsumeContext<T>(TContext context, ConsumeContext<T> consumeContext, TSaga instance, SagaConsumeContextMode mode)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`context` TContext<br/>
The [QuerySagaRepositoryContext\<TSaga\>](../masstransit-saga/querysagarepositorycontext-1)

`consumeContext` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>
The message consume context being delivered to the saga

`instance` TSaga<br/>
The saga instance

`mode` [SagaConsumeContextMode](../masstransit-saga/sagaconsumecontextmode)<br/>
The creation mode of the saga instance

#### Returns

[Task\<SagaConsumeContext\<TSaga, T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
