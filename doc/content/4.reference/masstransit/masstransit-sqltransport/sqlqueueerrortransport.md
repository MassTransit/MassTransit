---

title: SqlQueueErrorTransport

---

# SqlQueueErrorTransport

Namespace: MassTransit.SqlTransport

```csharp
public class SqlQueueErrorTransport : SqlQueueMoveTransport, IErrorTransport
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SqlQueueMoveTransport](../masstransit-sqltransport/sqlqueuemovetransport) → [SqlQueueErrorTransport](../masstransit-sqltransport/sqlqueueerrortransport)<br/>
Implements [IErrorTransport](../../masstransit-abstractions/masstransit-transports/ierrortransport)

## Constructors

### **SqlQueueErrorTransport(String, SqlQueueType)**

```csharp
public SqlQueueErrorTransport(string queueName, SqlQueueType queueType)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`queueType` [SqlQueueType](../masstransit/sqlqueuetype)<br/>

## Methods

### **Send(ExceptionReceiveContext)**

```csharp
public Task Send(ExceptionReceiveContext context)
```

#### Parameters

`context` [ExceptionReceiveContext](../../masstransit-abstractions/masstransit/exceptionreceivecontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
