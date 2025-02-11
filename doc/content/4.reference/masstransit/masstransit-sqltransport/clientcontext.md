---

title: ClientContext

---

# ClientContext

Namespace: MassTransit.SqlTransport

```csharp
public interface ClientContext : PipeContext
```

Implements [PipeContext](../../masstransit-abstractions/masstransit/pipecontext)

## Properties

### **ConnectionContext**

```csharp
public abstract ConnectionContext ConnectionContext { get; }
```

#### Property Value

[ConnectionContext](../masstransit-sqltransport/connectioncontext)<br/>

## Methods

### **CreateQueue(Queue)**

Create a queue

```csharp
Task<long> CreateQueue(Queue queue)
```

#### Parameters

`queue` [Queue](../masstransit-sqltransport-topology/queue)<br/>

#### Returns

[Task\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **CreateTopic(Topic)**

Create a topic

```csharp
Task<long> CreateTopic(Topic topic)
```

#### Parameters

`topic` [Topic](../masstransit-sqltransport-topology/topic)<br/>

#### Returns

[Task\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **CreateTopicSubscription(TopicToTopicSubscription)**

Create a topic subscription

```csharp
Task<long> CreateTopicSubscription(TopicToTopicSubscription subscription)
```

#### Parameters

`subscription` [TopicToTopicSubscription](../masstransit-sqltransport-topology/topictotopicsubscription)<br/>

#### Returns

[Task\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **CreateQueueSubscription(TopicToQueueSubscription)**

Create a topic subscription to a queue

```csharp
Task<long> CreateQueueSubscription(TopicToQueueSubscription subscription)
```

#### Parameters

`subscription` [TopicToQueueSubscription](../masstransit-sqltransport-topology/topictoqueuesubscription)<br/>

#### Returns

[Task\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **PurgeQueue(String, CancellationToken)**

Purge the specified queue (including all queue types), returning the number of messages removed

```csharp
Task<long> PurgeQueue(string queueName, CancellationToken cancellationToken)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Send\<T\>(String, SqlMessageSendContext\<T\>)**

```csharp
Task Send<T>(string queueName, SqlMessageSendContext<T> context)
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
Task Publish<T>(string topicName, SqlMessageSendContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`topicName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`context` [SqlMessageSendContext\<T\>](../masstransit-sqltransport/sqlmessagesendcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ReceiveMessages(String, SqlReceiveMode, Int32, Int32, TimeSpan)**

```csharp
Task<IEnumerable<SqlTransportMessage>> ReceiveMessages(string queueName, SqlReceiveMode mode, int messageLimit, int concurrentCount, TimeSpan lockDuration)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`mode` [SqlReceiveMode](../masstransit/sqlreceivemode)<br/>

`messageLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`concurrentCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`lockDuration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[Task\<IEnumerable\<SqlTransportMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **TouchQueue(String)**

```csharp
Task TouchQueue(string queueName)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **DeadLetterQueue(String, Int32)**

Move any messages that have either expired or exceeded their delivery count to the dead-letter queue

```csharp
Task<Nullable<int>> DeadLetterQueue(string queueName, int messageCount)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`messageCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[Task\<Nullable\<Int32\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **DeleteMessage(Guid, Int64)**

```csharp
Task<bool> DeleteMessage(Guid lockId, long messageDeliveryId)
```

#### Parameters

`lockId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`messageDeliveryId` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **DeleteScheduledMessage(Guid, CancellationToken)**

```csharp
Task<bool> DeleteScheduledMessage(Guid tokenId, CancellationToken cancellationToken)
```

#### Parameters

`tokenId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **MoveMessage(Guid, Int64, String, SqlQueueType, SendHeaders)**

```csharp
Task<bool> MoveMessage(Guid lockId, long messageDeliveryId, string queueName, SqlQueueType queueType, SendHeaders sendHeaders)
```

#### Parameters

`lockId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`messageDeliveryId` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`queueType` [SqlQueueType](../masstransit/sqlqueuetype)<br/>

`sendHeaders` [SendHeaders](../../masstransit-abstractions/masstransit/sendheaders)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **RenewLock(Guid, Int64, TimeSpan)**

```csharp
Task<bool> RenewLock(Guid lockId, long messageDeliveryId, TimeSpan duration)
```

#### Parameters

`lockId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`messageDeliveryId` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Unlock(Guid, Int64, TimeSpan, SendHeaders)**

```csharp
Task<bool> Unlock(Guid lockId, long messageDeliveryId, TimeSpan delay, SendHeaders sendHeaders)
```

#### Parameters

`lockId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`messageDeliveryId` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

`delay` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`sendHeaders` [SendHeaders](../../masstransit-abstractions/masstransit/sendheaders)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
