---

title: InitiatedByOrOrchestratesSagaMessageFilter<TSaga, TMessage>

---

# InitiatedByOrOrchestratesSagaMessageFilter\<TSaga, TMessage\>

Namespace: MassTransit.Middleware

Dispatches the ConsumeContext to the consumer method for the specified message type

```csharp
public class InitiatedByOrOrchestratesSagaMessageFilter<TSaga, TMessage> : ISagaMessageFilter<TSaga, TMessage>, IFilter<SagaConsumeContext<TSaga, TMessage>>, IProbeSite
```

#### Type Parameters

`TSaga`<br/>
The consumer type

`TMessage`<br/>
The message type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InitiatedByOrOrchestratesSagaMessageFilter\<TSaga, TMessage\>](../masstransit-middleware/initiatedbyororchestratessagamessagefilter-2)<br/>
Implements [ISagaMessageFilter\<TSaga, TMessage\>](../masstransit-middleware/isagamessagefilter-2), [IFilter\<SagaConsumeContext\<TSaga, TMessage\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **InitiatedByOrOrchestratesSagaMessageFilter()**

```csharp
public InitiatedByOrOrchestratesSagaMessageFilter()
```

## Methods

### **Send(SagaConsumeContext\<TSaga, TMessage\>, IPipe\<SagaConsumeContext\<TSaga, TMessage\>\>)**

```csharp
public Task Send(SagaConsumeContext<TSaga, TMessage> context, IPipe<SagaConsumeContext<TSaga, TMessage>> next)
```

#### Parameters

`context` [SagaConsumeContext\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/sagaconsumecontext-2)<br/>

`next` [IPipe\<SagaConsumeContext\<TSaga, TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
