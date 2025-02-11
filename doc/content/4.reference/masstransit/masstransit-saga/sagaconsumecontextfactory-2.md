---

title: SagaConsumeContextFactory<TContext, TSaga>

---

# SagaConsumeContextFactory\<TContext, TSaga\>

Namespace: MassTransit.Saga

```csharp
public class SagaConsumeContextFactory<TContext, TSaga> : ISagaConsumeContextFactory<TContext, TSaga>
```

#### Type Parameters

`TContext`<br/>

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SagaConsumeContextFactory\<TContext, TSaga\>](../masstransit-saga/sagaconsumecontextfactory-2)<br/>
Implements [ISagaConsumeContextFactory\<TContext, TSaga\>](../masstransit-saga/isagaconsumecontextfactory-2)

## Constructors

### **SagaConsumeContextFactory()**

```csharp
public SagaConsumeContextFactory()
```

## Methods

### **CreateSagaConsumeContext\<T\>(TContext, ConsumeContext\<T\>, TSaga, SagaConsumeContextMode)**

```csharp
public Task<SagaConsumeContext<TSaga, T>> CreateSagaConsumeContext<T>(TContext context, ConsumeContext<T> consumeContext, TSaga instance, SagaConsumeContextMode mode)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` TContext<br/>

`consumeContext` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`instance` TSaga<br/>

`mode` [SagaConsumeContextMode](../masstransit-saga/sagaconsumecontextmode)<br/>

#### Returns

[Task\<SagaConsumeContext\<TSaga, T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
