---

title: OutboxConsumeFilter<TContext, TMessage>

---

# OutboxConsumeFilter\<TContext, TMessage\>

Namespace: MassTransit.Middleware

Sends the message through the outbox

```csharp
public class OutboxConsumeFilter<TContext, TMessage> : IFilter<ConsumeContext<TMessage>>, IProbeSite
```

#### Type Parameters

`TContext`<br/>
The outbox context type

`TMessage`<br/>
The message type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [OutboxConsumeFilter\<TContext, TMessage\>](../masstransit-middleware/outboxconsumefilter-2)<br/>
Implements [IFilter\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **OutboxConsumeFilter(IConsumeScopeProvider, OutboxConsumeOptions)**

```csharp
public OutboxConsumeFilter(IConsumeScopeProvider scopeProvider, OutboxConsumeOptions options)
```

#### Parameters

`scopeProvider` [IConsumeScopeProvider](../masstransit-dependencyinjection/iconsumescopeprovider)<br/>

`options` [OutboxConsumeOptions](../masstransit-middleware/outboxconsumeoptions)<br/>

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
