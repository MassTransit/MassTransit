---

title: InMemoryMessageMoveTransport

---

# InMemoryMessageMoveTransport

Namespace: MassTransit.InMemoryTransport

```csharp
public class InMemoryMessageMoveTransport
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InMemoryMessageMoveTransport](../masstransit-inmemorytransport/inmemorymessagemovetransport)

## Methods

### **Move(ReceiveContext, Action\<InMemoryTransportMessage, SendHeaders\>)**

```csharp
protected Task Move(ReceiveContext context, Action<InMemoryTransportMessage, SendHeaders> preSend)
```

#### Parameters

`context` [ReceiveContext](../../masstransit-abstractions/masstransit/receivecontext)<br/>

`preSend` [Action\<InMemoryTransportMessage, SendHeaders\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
