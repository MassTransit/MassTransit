---

title: CorrelationIdMessageFilter<TMessage>

---

# CorrelationIdMessageFilter\<TMessage\>

Namespace: MassTransit.Middleware

Extracts the CorrelationId from the message where there is a one-to-one correlation
 identifier in the message (such as CorrelationId) and sets it in the header for use
 by the saga repository.

```csharp
public class CorrelationIdMessageFilter<TMessage> : IFilter<ConsumeContext<TMessage>>, IProbeSite
```

#### Type Parameters

`TMessage`<br/>
The message type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CorrelationIdMessageFilter\<TMessage\>](../masstransit-middleware/correlationidmessagefilter-1)<br/>
Implements [IFilter\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **CorrelationIdMessageFilter(Func\<ConsumeContext\<TMessage\>, Guid\>)**

```csharp
public CorrelationIdMessageFilter(Func<ConsumeContext<TMessage>, Guid> getCorrelationId)
```

#### Parameters

`getCorrelationId` [Func\<ConsumeContext\<TMessage\>, Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

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
