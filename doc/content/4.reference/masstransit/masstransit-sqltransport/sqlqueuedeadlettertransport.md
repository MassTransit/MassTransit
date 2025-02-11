---

title: SqlQueueDeadLetterTransport

---

# SqlQueueDeadLetterTransport

Namespace: MassTransit.SqlTransport

```csharp
public class SqlQueueDeadLetterTransport : SqlQueueMoveTransport, IDeadLetterTransport
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SqlQueueMoveTransport](../masstransit-sqltransport/sqlqueuemovetransport) → [SqlQueueDeadLetterTransport](../masstransit-sqltransport/sqlqueuedeadlettertransport)<br/>
Implements [IDeadLetterTransport](../../masstransit-abstractions/masstransit-transports/ideadlettertransport)

## Constructors

### **SqlQueueDeadLetterTransport(String, SqlQueueType)**

```csharp
public SqlQueueDeadLetterTransport(string queueName, SqlQueueType queueType)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`queueType` [SqlQueueType](../masstransit/sqlqueuetype)<br/>

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
