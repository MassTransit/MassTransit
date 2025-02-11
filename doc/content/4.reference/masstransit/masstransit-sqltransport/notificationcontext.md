---

title: NotificationContext

---

# NotificationContext

Namespace: MassTransit.SqlTransport

```csharp
public interface NotificationContext : PipeContext
```

Implements [PipeContext](../../masstransit-abstractions/masstransit/pipecontext)

## Methods

### **ConnectNotificationSink(String, IQueueNotificationListener)**

```csharp
ConnectHandle ConnectNotificationSink(string queueName, IQueueNotificationListener listener)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`listener` [IQueueNotificationListener](../masstransit-sqltransport/iqueuenotificationlistener)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
