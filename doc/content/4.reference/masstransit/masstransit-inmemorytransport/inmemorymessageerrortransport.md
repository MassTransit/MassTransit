---

title: InMemoryMessageErrorTransport

---

# InMemoryMessageErrorTransport

Namespace: MassTransit.InMemoryTransport

```csharp
public class InMemoryMessageErrorTransport : InMemoryMessageMoveTransport, IErrorTransport
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InMemoryMessageMoveTransport](../masstransit-inmemorytransport/inmemorymessagemovetransport) → [InMemoryMessageErrorTransport](../masstransit-inmemorytransport/inmemorymessageerrortransport)<br/>
Implements [IErrorTransport](../../masstransit-abstractions/masstransit-transports/ierrortransport)

## Constructors

### **InMemoryMessageErrorTransport(IMessageExchange\<InMemoryTransportMessage\>)**

```csharp
public InMemoryMessageErrorTransport(IMessageExchange<InMemoryTransportMessage> exchange)
```

#### Parameters

`exchange` [IMessageExchange\<InMemoryTransportMessage\>](../masstransit-transports-fabric/imessageexchange-1)<br/>

## Methods

### **Send(ExceptionReceiveContext)**

```csharp
public Task Send(ExceptionReceiveContext context)
```

#### Parameters

`context` [ExceptionReceiveContext](../../masstransit-abstractions/masstransit/exceptionreceivecontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
