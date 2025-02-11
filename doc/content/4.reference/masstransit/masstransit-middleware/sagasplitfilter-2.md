---

title: SagaSplitFilter<TSaga, TMessage>

---

# SagaSplitFilter\<TSaga, TMessage\>

Namespace: MassTransit.Middleware

Splits a context item off the pipe and carries it out-of-band to be merged
 once the next filter has completed

```csharp
public class SagaSplitFilter<TSaga, TMessage> : IFilter<SagaConsumeContext<TSaga, TMessage>>, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SagaSplitFilter\<TSaga, TMessage\>](../masstransit-middleware/sagasplitfilter-2)<br/>
Implements [IFilter\<SagaConsumeContext\<TSaga, TMessage\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **SagaSplitFilter(IFilter\<SagaConsumeContext\<TSaga\>\>)**

```csharp
public SagaSplitFilter(IFilter<SagaConsumeContext<TSaga>> next)
```

#### Parameters

`next` [IFilter\<SagaConsumeContext\<TSaga\>\>](../../masstransit-abstractions/masstransit/ifilter-1)<br/>

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
