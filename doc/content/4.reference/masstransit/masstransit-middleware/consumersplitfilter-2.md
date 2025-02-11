---

title: ConsumerSplitFilter<TConsumer, TMessage>

---

# ConsumerSplitFilter\<TConsumer, TMessage\>

Namespace: MassTransit.Middleware

Splits a context item off the pipe and carries it out-of-band to be merged
 once the next filter has completed

```csharp
public class ConsumerSplitFilter<TConsumer, TMessage> : IFilter<ConsumerConsumeContext<TConsumer, TMessage>>, IProbeSite
```

#### Type Parameters

`TConsumer`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumerSplitFilter\<TConsumer, TMessage\>](../masstransit-middleware/consumersplitfilter-2)<br/>
Implements [IFilter\<ConsumerConsumeContext\<TConsumer, TMessage\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ConsumerSplitFilter(IFilter\<ConsumerConsumeContext\<TConsumer\>\>)**

```csharp
public ConsumerSplitFilter(IFilter<ConsumerConsumeContext<TConsumer>> next)
```

#### Parameters

`next` [IFilter\<ConsumerConsumeContext\<TConsumer\>\>](../../masstransit-abstractions/masstransit/ifilter-1)<br/>

## Methods

### **Send(ConsumerConsumeContext\<TConsumer, TMessage\>, IPipe\<ConsumerConsumeContext\<TConsumer, TMessage\>\>)**

```csharp
public Task Send(ConsumerConsumeContext<TConsumer, TMessage> context, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
```

#### Parameters

`context` [ConsumerConsumeContext\<TConsumer, TMessage\>](../../masstransit-abstractions/masstransit/consumerconsumecontext-2)<br/>

`next` [IPipe\<ConsumerConsumeContext\<TConsumer, TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
