---

title: OutboxMessageSendPipe

---

# OutboxMessageSendPipe

Namespace: MassTransit.Middleware

```csharp
public class OutboxMessageSendPipe : IPipe<SendContext>, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [OutboxMessageSendPipe](../masstransit-middleware/outboxmessagesendpipe)<br/>
Implements [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **OutboxMessageSendPipe(OutboxMessageContext, Uri)**

```csharp
public OutboxMessageSendPipe(OutboxMessageContext message, Uri destinationAddress)
```

#### Parameters

`message` [OutboxMessageContext](../masstransit-middleware/outboxmessagecontext)<br/>

`destinationAddress` Uri<br/>

## Methods

### **Send(SendContext)**

```csharp
public Task Send(SendContext context)
```

#### Parameters

`context` [SendContext](../../masstransit-abstractions/masstransit/sendcontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
