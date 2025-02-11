---

title: FactoryMethodSagaFactory<TSaga, TMessage>

---

# FactoryMethodSagaFactory\<TSaga, TMessage\>

Namespace: MassTransit.Saga

Creates a saga instance using the default factory method

```csharp
public class FactoryMethodSagaFactory<TSaga, TMessage> : ISagaFactory<TSaga, TMessage>
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FactoryMethodSagaFactory\<TSaga, TMessage\>](../masstransit-saga/factorymethodsagafactory-2)<br/>
Implements [ISagaFactory\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/isagafactory-2)

## Constructors

### **FactoryMethodSagaFactory(SagaFactoryMethod\<TSaga, TMessage\>)**

```csharp
public FactoryMethodSagaFactory(SagaFactoryMethod<TSaga, TMessage> factoryMethod)
```

#### Parameters

`factoryMethod` [SagaFactoryMethod\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/sagafactorymethod-2)<br/>

## Methods

### **Create(ConsumeContext\<TMessage\>)**

```csharp
public TSaga Create(ConsumeContext<TMessage> context)
```

#### Parameters

`context` [ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

TSaga<br/>

### **Send(ConsumeContext\<TMessage\>, IPipe\<SagaConsumeContext\<TSaga, TMessage\>\>)**

```csharp
public Task Send(ConsumeContext<TMessage> context, IPipe<SagaConsumeContext<TSaga, TMessage>> next)
```

#### Parameters

`context` [ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`next` [IPipe\<SagaConsumeContext\<TSaga, TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
