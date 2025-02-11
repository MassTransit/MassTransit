---

title: ScopeClientContext

---

# ScopeClientContext

Namespace: MassTransit.SqlTransport

```csharp
public class ScopeClientContext : ScopePipeContext, ClientContext, PipeContext
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScopePipeContext](../../masstransit-abstractions/masstransit-middleware/scopepipecontext) → [ScopeClientContext](../masstransit-sqltransport/scopeclientcontext)<br/>
Implements [ClientContext](../masstransit-sqltransport/clientcontext), [PipeContext](../../masstransit-abstractions/masstransit/pipecontext)

## Properties

### **CancellationToken**

```csharp
public CancellationToken CancellationToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **ConnectionContext**

```csharp
public ConnectionContext ConnectionContext { get; }
```

#### Property Value

[ConnectionContext](../masstransit-sqltransport/connectioncontext)<br/>

## Constructors

### **ScopeClientContext(ClientContext, CancellationToken)**

```csharp
public ScopeClientContext(ClientContext context, CancellationToken cancellationToken)
```

#### Parameters

`context` [ClientContext](../masstransit-sqltransport/clientcontext)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Methods

### **CreateQueue(Queue)**

```csharp
public Task<long> CreateQueue(Queue queue)
```

#### Parameters

`queue` [Queue](../masstransit-sqltransport-topology/queue)<br/>

#### Returns

[Task\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **CreateTopic(Topic)**

```csharp
public Task<long> CreateTopic(Topic topic)
```

#### Parameters

`topic` [Topic](../masstransit-sqltransport-topology/topic)<br/>

#### Returns

[Task\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **CreateTopicSubscription(TopicToTopicSubscription)**

```csharp
public Task<long> CreateTopicSubscription(TopicToTopicSubscription subscription)
```

#### Parameters

`subscription` [TopicToTopicSubscription](../masstransit-sqltransport-topology/topictotopicsubscription)<br/>

#### Returns

[Task\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **CreateQueueSubscription(TopicToQueueSubscription)**

```csharp
public Task<long> CreateQueueSubscription(TopicToQueueSubscription subscription)
```

#### Parameters

`subscription` [TopicToQueueSubscription](../masstransit-sqltransport-topology/topictoqueuesubscription)<br/>

#### Returns

[Task\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **PurgeQueue(String, CancellationToken)**

```csharp
public Task<long> PurgeQueue(string queueName, CancellationToken cancellationToken)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Send\<T\>(String, SqlMessageSendContext\<T\>)**

```csharp
public Task Send<T>(string queueName, SqlMessageSendContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`context` [SqlMessageSendContext\<T\>](../masstransit-sqltransport/sqlmessagesendcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Publish\<T\>(String, SqlMessageSendContext\<T\>)**

```csharp
public Task Publish<T>(string topicName, SqlMessageSendContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`topicName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`context` [SqlMessageSendContext\<T\>](../masstransit-sqltransport/sqlmessagesendcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **RenewLock(Guid, Int64, TimeSpan)**

```csharp
public Task<bool> RenewLock(Guid lockId, long messageDeliveryId, TimeSpan duration)
```

#### Parameters

`lockId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`messageDeliveryId` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Unlock(Guid, Int64, TimeSpan, SendHeaders)**

```csharp
public Task<bool> Unlock(Guid lockId, long messageDeliveryId, TimeSpan delay, SendHeaders sendHeaders)
```

#### Parameters

`lockId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`messageDeliveryId` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`sendHeaders` [SendHeaders](../../masstransit-abstractions/masstransit/sendheaders)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ReceiveMessages(String, SqlReceiveMode, Int32, Int32, TimeSpan)**

```csharp
public Task<IEnumerable<SqlTransportMessage>> ReceiveMessages(string queueName, SqlReceiveMode mode, int messageLimit, int concurrentLimit, TimeSpan lockDuration)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`mode` [SqlReceiveMode](../masstransit/sqlreceivemode)<br/>

`messageLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`concurrentLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`lockDuration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[Task\<IEnumerable\<SqlTransportMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **TouchQueue(String)**

```csharp
public Task TouchQueue(string queueName)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **DeadLetterQueue(String, Int32)**

```csharp
public Task<Nullable<int>> DeadLetterQueue(string queueName, int messageCount)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`messageCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[Task\<Nullable\<Int32\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **DeleteMessage(Guid, Int64)**

```csharp
public Task<bool> DeleteMessage(Guid lockId, long messageDeliveryId)
```

#### Parameters

`lockId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`messageDeliveryId` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **DeleteScheduledMessage(Guid, CancellationToken)**

```csharp
public Task<bool> DeleteScheduledMessage(Guid tokenId, CancellationToken cancellationToken)
```

#### Parameters

`tokenId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **MoveMessage(Guid, Int64, String, SqlQueueType, SendHeaders)**

```csharp
public Task<bool> MoveMessage(Guid lockId, long messageDeliveryId, string queueName, SqlQueueType queueType, SendHeaders sendHeaders)
```

#### Parameters

`lockId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`messageDeliveryId` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`queueType` [SqlQueueType](../masstransit/sqlqueuetype)<br/>

`sendHeaders` [SendHeaders](../../masstransit-abstractions/masstransit/sendheaders)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
