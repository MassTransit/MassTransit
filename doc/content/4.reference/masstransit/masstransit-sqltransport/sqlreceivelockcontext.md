---

title: SqlReceiveLockContext

---

# SqlReceiveLockContext

Namespace: MassTransit.SqlTransport

```csharp
public class SqlReceiveLockContext : MessageRedeliveryContext, ReceiveLockContext
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SqlReceiveLockContext](../masstransit-sqltransport/sqlreceivelockcontext)<br/>
Implements [MessageRedeliveryContext](../../masstransit-abstractions/masstransit/messageredeliverycontext), [ReceiveLockContext](../masstransit-transports/receivelockcontext)

## Constructors

### **SqlReceiveLockContext(Uri, SqlTransportMessage, ReceiveSettings, ClientContext)**

```csharp
public SqlReceiveLockContext(Uri inputAddress, SqlTransportMessage message, ReceiveSettings settings, ClientContext clientContext)
```

#### Parameters

`inputAddress` Uri<br/>

`message` [SqlTransportMessage](../masstransit-sqltransport/sqltransportmessage)<br/>

`settings` [ReceiveSettings](../masstransit-sqltransport/receivesettings)<br/>

`clientContext` [ClientContext](../masstransit-sqltransport/clientcontext)<br/>

## Methods

### **ScheduleRedelivery(TimeSpan, Action\<ConsumeContext, SendContext\>)**

```csharp
public Task ScheduleRedelivery(TimeSpan delay, Action<ConsumeContext, SendContext> callback)
```

#### Parameters

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`callback` [Action\<ConsumeContext, SendContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Complete()**

```csharp
public Task Complete()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Expired()**

```csharp
public Task Expired()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Faulted(Exception)**

```csharp
public Task Faulted(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ValidateLockStatus()**

```csharp
public Task ValidateLockStatus()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
