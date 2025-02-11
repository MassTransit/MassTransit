---

title: DefaultSagaFactory<TSaga, TMessage>

---

# DefaultSagaFactory\<TSaga, TMessage\>

Namespace: MassTransit.Configuration

Creates a saga instance using the default factory method

```csharp
public class DefaultSagaFactory<TSaga, TMessage> : ISagaFactory<TSaga, TMessage>
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DefaultSagaFactory\<TSaga, TMessage\>](../masstransit-configuration/defaultsagafactory-2)<br/>
Implements [ISagaFactory\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/isagafactory-2)

## Constructors

### **DefaultSagaFactory()**

```csharp
public DefaultSagaFactory()
```

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
