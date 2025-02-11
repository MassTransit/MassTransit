---

title: ISagaFactory<TSaga, TMessage>

---

# ISagaFactory\<TSaga, TMessage\>

Namespace: MassTransit

Creates a saga instance when an existing saga instance is missing

```csharp
public interface ISagaFactory<TSaga, TMessage>
```

#### Type Parameters

`TSaga`<br/>
The saga type

`TMessage`<br/>

## Methods

### **Create(ConsumeContext\<TMessage\>)**

Create a new saga instance using the supplied consume context

```csharp
TSaga Create(ConsumeContext<TMessage> context)
```

#### Parameters

`context` [ConsumeContext\<TMessage\>](../masstransit/consumecontext-1)<br/>

#### Returns

TSaga<br/>

### **Send(ConsumeContext\<TMessage\>, IPipe\<SagaConsumeContext\<TSaga, TMessage\>\>)**

Send the context through the factory, with the proper decorations

```csharp
Task Send(ConsumeContext<TMessage> context, IPipe<SagaConsumeContext<TSaga, TMessage>> next)
```

#### Parameters

`context` [ConsumeContext\<TMessage\>](../masstransit/consumecontext-1)<br/>

`next` [IPipe\<SagaConsumeContext\<TSaga, TMessage\>\>](../masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
