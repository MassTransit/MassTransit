---

title: InMemorySagaConsumeContextFactory<TSaga>

---

# InMemorySagaConsumeContextFactory\<TSaga\>

Namespace: MassTransit.Saga

```csharp
public class InMemorySagaConsumeContextFactory<TSaga> : ISagaConsumeContextFactory<IndexedSagaDictionary<TSaga>, TSaga>
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InMemorySagaConsumeContextFactory\<TSaga\>](../masstransit-saga/inmemorysagaconsumecontextfactory-1)<br/>
Implements [ISagaConsumeContextFactory\<IndexedSagaDictionary\<TSaga\>, TSaga\>](../masstransit-saga/isagaconsumecontextfactory-2)

## Constructors

### **InMemorySagaConsumeContextFactory()**

```csharp
public InMemorySagaConsumeContextFactory()
```

## Methods

### **CreateSagaConsumeContext\<T\>(IndexedSagaDictionary\<TSaga\>, ConsumeContext\<T\>, TSaga, SagaConsumeContextMode)**

```csharp
public Task<SagaConsumeContext<TSaga, T>> CreateSagaConsumeContext<T>(IndexedSagaDictionary<TSaga> context, ConsumeContext<T> consumeContext, TSaga instance, SagaConsumeContextMode mode)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [IndexedSagaDictionary\<TSaga\>](../masstransit-saga/indexedsagadictionary-1)<br/>

`consumeContext` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`instance` TSaga<br/>

`mode` [SagaConsumeContextMode](../masstransit-saga/sagaconsumecontextmode)<br/>

#### Returns

[Task\<SagaConsumeContext\<TSaga, T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
