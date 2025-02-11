---

title: InMemoryMessageDeadLetterTransport

---

# InMemoryMessageDeadLetterTransport

Namespace: MassTransit.InMemoryTransport

```csharp
public class InMemoryMessageDeadLetterTransport : InMemoryMessageMoveTransport, IDeadLetterTransport
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InMemoryMessageMoveTransport](../masstransit-inmemorytransport/inmemorymessagemovetransport) → [InMemoryMessageDeadLetterTransport](../masstransit-inmemorytransport/inmemorymessagedeadlettertransport)<br/>
Implements [IDeadLetterTransport](../../masstransit-abstractions/masstransit-transports/ideadlettertransport)

## Constructors

### **InMemoryMessageDeadLetterTransport(IMessageExchange\<InMemoryTransportMessage\>)**

```csharp
public InMemoryMessageDeadLetterTransport(IMessageExchange<InMemoryTransportMessage> exchange)
```

#### Parameters

`exchange` [IMessageExchange\<InMemoryTransportMessage\>](../masstransit-transports-fabric/imessageexchange-1)<br/>

## Methods

### **Send(ReceiveContext, String)**

```csharp
public Task Send(ReceiveContext context, string reason)
```

#### Parameters

`context` [ReceiveContext](../../masstransit-abstractions/masstransit/receivecontext)<br/>

`reason` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
