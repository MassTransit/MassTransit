---

title: SqlReceiveContext

---

# SqlReceiveContext

Namespace: MassTransit.SqlTransport

```csharp
public sealed class SqlReceiveContext : BaseReceiveContext, ReceiveContext, PipeContext, IDisposable, SqlMessageContext, RoutingKeyConsumeContext, PartitionKeyConsumeContext, TransportReceiveContext, ITransportSequenceNumber
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScopePipeContext](../../masstransit-abstractions/masstransit-middleware/scopepipecontext) → [BaseReceiveContext](../masstransit-transports/basereceivecontext) → [SqlReceiveContext](../masstransit-sqltransport/sqlreceivecontext)<br/>
Implements [ReceiveContext](../../masstransit-abstractions/masstransit/receivecontext), [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable), [SqlMessageContext](../masstransit/sqlmessagecontext), [RoutingKeyConsumeContext](../../masstransit-abstractions/masstransit/routingkeyconsumecontext), [PartitionKeyConsumeContext](../../masstransit-abstractions/masstransit/partitionkeyconsumecontext), [TransportReceiveContext](../masstransit-context/transportreceivecontext), [ITransportSequenceNumber](../../masstransit-abstractions/masstransit/itransportsequencenumber)

## Properties

### **Body**

```csharp
public MessageBody Body { get; }
```

#### Property Value

[MessageBody](../../masstransit-abstractions/masstransit/messagebody)<br/>

### **SequenceNumber**

```csharp
public Nullable<ulong> SequenceNumber { get; }
```

#### Property Value

[Nullable\<UInt64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **TransportMessage**

```csharp
public SqlTransportMessage TransportMessage { get; }
```

#### Property Value

[SqlTransportMessage](../masstransit-sqltransport/sqltransportmessage)<br/>

### **RoutingKey**

```csharp
public string RoutingKey { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **TransportMessageId**

```csharp
public Guid TransportMessageId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **ConsumerId**

```csharp
public Nullable<Guid> ConsumerId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **LockId**

```csharp
public Nullable<Guid> LockId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **QueueName**

```csharp
public string QueueName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Priority**

```csharp
public short Priority { get; }
```

#### Property Value

[Int16](https://learn.microsoft.com/en-us/dotnet/api/system.int16)<br/>

### **DeliveryMessageId**

```csharp
public long DeliveryMessageId { get; }
```

#### Property Value

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

### **EnqueueTime**

```csharp
public DateTime EnqueueTime { get; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **DeliveryCount**

```csharp
public int DeliveryCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **PartitionKey**

```csharp
public string PartitionKey { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **CancellationToken**

```csharp
public CancellationToken CancellationToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **IsDelivered**

```csharp
public bool IsDelivered { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsFaulted**

```csharp
public bool IsFaulted { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **PublishFaults**

```csharp
public bool PublishFaults { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **SendEndpointProvider**

```csharp
public ISendEndpointProvider SendEndpointProvider { get; }
```

#### Property Value

[ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider)<br/>

### **PublishEndpointProvider**

```csharp
public IPublishEndpointProvider PublishEndpointProvider { get; }
```

#### Property Value

[IPublishEndpointProvider](../../masstransit-abstractions/masstransit/ipublishendpointprovider)<br/>

### **ReceiveCompleted**

```csharp
public Task ReceiveCompleted { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Redelivered**

```csharp
public bool Redelivered { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TransportHeaders**

```csharp
public Headers TransportHeaders { get; }
```

#### Property Value

[Headers](../../masstransit-abstractions/masstransit/headers)<br/>

### **ElapsedTime**

```csharp
public TimeSpan ElapsedTime { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **InputAddress**

```csharp
public Uri InputAddress { get; protected set; }
```

#### Property Value

Uri<br/>

### **ContentType**

```csharp
public ContentType ContentType { get; }
```

#### Property Value

ContentType<br/>

## Constructors

### **SqlReceiveContext(SqlTransportMessage, SqlReceiveEndpointContext, ReceiveSettings, ClientContext, ConnectionContext, SqlReceiveLockContext)**

```csharp
public SqlReceiveContext(SqlTransportMessage message, SqlReceiveEndpointContext context, ReceiveSettings settings, ClientContext clientContext, ConnectionContext connectionContext, SqlReceiveLockContext lockContext)
```

#### Parameters

`message` [SqlTransportMessage](../masstransit-sqltransport/sqltransportmessage)<br/>

`context` [SqlReceiveEndpointContext](../masstransit-sqltransport/sqlreceiveendpointcontext)<br/>

`settings` [ReceiveSettings](../masstransit-sqltransport/receivesettings)<br/>

`clientContext` [ClientContext](../masstransit-sqltransport/clientcontext)<br/>

`connectionContext` [ConnectionContext](../masstransit-sqltransport/connectioncontext)<br/>

`lockContext` [SqlReceiveLockContext](../masstransit-sqltransport/sqlreceivelockcontext)<br/>

## Methods

### **GetTransportProperties()**

```csharp
public IDictionary<string, object> GetTransportProperties()
```

#### Returns

[IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>
