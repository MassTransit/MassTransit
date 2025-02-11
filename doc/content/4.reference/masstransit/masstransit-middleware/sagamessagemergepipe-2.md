---

title: SagaMessageMergePipe<TSaga, TMessage>

---

# SagaMessageMergePipe\<TSaga, TMessage\>

Namespace: MassTransit.Middleware

Merges the out-of-band Saga back into the context

```csharp
public class SagaMessageMergePipe<TSaga, TMessage> : IPipe<ConsumeContext<TMessage>>, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SagaMessageMergePipe\<TSaga, TMessage\>](../masstransit-middleware/sagamessagemergepipe-2)<br/>
Implements [IPipe\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **SagaMessageMergePipe(IPipe\<SagaConsumeContext\<TSaga, TMessage\>\>, SagaConsumeContext\<TSaga, TMessage\>)**

```csharp
public SagaMessageMergePipe(IPipe<SagaConsumeContext<TSaga, TMessage>> output, SagaConsumeContext<TSaga, TMessage> context)
```

#### Parameters

`output` [IPipe\<SagaConsumeContext\<TSaga, TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`context` [SagaConsumeContext\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/sagaconsumecontext-2)<br/>

## Methods

### **Send(ConsumeContext\<TMessage\>)**

```csharp
public Task Send(ConsumeContext<TMessage> context)
```

#### Parameters

`context` [ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
