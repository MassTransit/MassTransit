---

title: ConsumeConcurrencyLimitFilter<TMessage>

---

# ConsumeConcurrencyLimitFilter\<TMessage\>

Namespace: MassTransit.Middleware

A concurrency limit filter that is shared by multiple message types, so that a consumer
 accepting those various types can be limited to a specific number of consumer instances.

```csharp
public class ConsumeConcurrencyLimitFilter<TMessage> : IFilter<ConsumeContext<TMessage>>, IProbeSite
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumeConcurrencyLimitFilter\<TMessage\>](../masstransit-middleware/consumeconcurrencylimitfilter-1)<br/>
Implements [IFilter\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **ConsumeConcurrencyLimitFilter(IConcurrencyLimiter)**

```csharp
public ConsumeConcurrencyLimitFilter(IConcurrencyLimiter limiter)
```

#### Parameters

`limiter` [IConcurrencyLimiter](../masstransit-middleware/iconcurrencylimiter)<br/>

## Methods

### **Send(ConsumeContext\<TMessage\>, IPipe\<ConsumeContext\<TMessage\>\>)**

```csharp
public Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
```

#### Parameters

`context` [ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`next` [IPipe\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
