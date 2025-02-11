---

title: DelayedMessageRedeliveryFilter<TMessage>

---

# DelayedMessageRedeliveryFilter\<TMessage\>

Namespace: MassTransit.Middleware

Uses a delayed exchange in ActiveMQ to delay a message retry

```csharp
public class DelayedMessageRedeliveryFilter<TMessage> : IFilter<ConsumeContext<TMessage>>, IProbeSite
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DelayedMessageRedeliveryFilter\<TMessage\>](../masstransit-middleware/delayedmessageredeliveryfilter-1)<br/>
Implements [IFilter\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **DelayedMessageRedeliveryFilter(RedeliveryOptions)**

```csharp
public DelayedMessageRedeliveryFilter(RedeliveryOptions options)
```

#### Parameters

`options` [RedeliveryOptions](../../masstransit-abstractions/masstransit/redeliveryoptions)<br/>

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **Send(ConsumeContext\<TMessage\>, IPipe\<ConsumeContext\<TMessage\>\>)**

```csharp
public Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
```

#### Parameters

`context` [ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`next` [IPipe\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
