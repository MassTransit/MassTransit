---

title: OutboxMessagePipe<TMessage>

---

# OutboxMessagePipe\<TMessage\>

Namespace: MassTransit.Middleware

```csharp
public class OutboxMessagePipe<TMessage> : IPipe<OutboxConsumeContext<TMessage>>, IProbeSite
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [OutboxMessagePipe\<TMessage\>](../masstransit-middleware/outboxmessagepipe-1)<br/>
Implements [IPipe\<OutboxConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **OutboxMessagePipe(OutboxConsumeOptions, IConsumeScopeContext\<TMessage\>, IPipe\<ConsumeContext\<TMessage\>\>)**

```csharp
public OutboxMessagePipe(OutboxConsumeOptions options, IConsumeScopeContext<TMessage> scopeContext, IPipe<ConsumeContext<TMessage>> next)
```

#### Parameters

`options` [OutboxConsumeOptions](../masstransit-middleware/outboxconsumeoptions)<br/>

`scopeContext` [IConsumeScopeContext\<TMessage\>](../masstransit-dependencyinjection/iconsumescopecontext-1)<br/>

`next` [IPipe\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

## Methods

### **Send(OutboxConsumeContext\<TMessage\>)**

```csharp
public Task Send(OutboxConsumeContext<TMessage> context)
```

#### Parameters

`context` [OutboxConsumeContext\<TMessage\>](../masstransit-middleware/outboxconsumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
