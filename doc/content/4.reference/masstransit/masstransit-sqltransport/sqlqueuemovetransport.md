---

title: SqlQueueMoveTransport

---

# SqlQueueMoveTransport

Namespace: MassTransit.SqlTransport

```csharp
public class SqlQueueMoveTransport
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SqlQueueMoveTransport](../masstransit-sqltransport/sqlqueuemovetransport)

## Methods

### **Move(ReceiveContext, Action\<SqlTransportMessage, SendHeaders\>)**

```csharp
protected Task Move(ReceiveContext context, Action<SqlTransportMessage, SendHeaders> preSend)
```

#### Parameters

`context` [ReceiveContext](../../masstransit-abstractions/masstransit/receivecontext)<br/>

`preSend` [Action\<SqlTransportMessage, SendHeaders\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
