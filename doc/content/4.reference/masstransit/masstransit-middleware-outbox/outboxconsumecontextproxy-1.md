---

title: OutboxConsumeContextProxy<TMessage>

---

# OutboxConsumeContextProxy\<TMessage\>

Namespace: MassTransit.Middleware.Outbox

```csharp
public abstract class OutboxConsumeContextProxy<TMessage> : ConsumeContextProxy<TMessage>, IPublishEndpoint, IPublishObserverConnector, ConsumeContext, PipeContext, MessageContext, ISendEndpointProvider, ISendObserverConnector, ConsumeContext<TMessage>, OutboxConsumeContext<TMessage>, OutboxConsumeContext, OutboxSendContext, IServiceProvider
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PublishEndpoint](../../masstransit-abstractions/masstransit-transports/publishendpoint) → [BaseConsumeContext](../masstransit-context/baseconsumecontext) → [ConsumeContextProxy](../masstransit-context/consumecontextproxy) → [ConsumeContextProxy\<TMessage\>](../masstransit-context/consumecontextproxy-1) → [OutboxConsumeContextProxy\<TMessage\>](../masstransit-middleware-outbox/outboxconsumecontextproxy-1)<br/>
Implements [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext), [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [MessageContext](../../masstransit-abstractions/masstransit/messagecontext), [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1), [OutboxConsumeContext\<TMessage\>](../masstransit-middleware/outboxconsumecontext-1), [OutboxConsumeContext](../masstransit-middleware/outboxconsumecontext), [OutboxSendContext](../masstransit-middleware/outboxsendcontext), IServiceProvider

## Properties

### **CapturedContext**

```csharp
public ConsumeContext CapturedContext { get; }
```

#### Property Value

[ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

### **ContinueProcessing**

```csharp
public abstract bool ContinueProcessing { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsMessageConsumed**

```csharp
public abstract bool IsMessageConsumed { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsOutboxDelivered**

```csharp
public abstract bool IsOutboxDelivered { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **ReceiveCount**

```csharp
public abstract int ReceiveCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **LastSequenceNumber**

```csharp
public abstract Nullable<long> LastSequenceNumber { get; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Message**

```csharp
public TMessage Message { get; }
```

#### Property Value

TMessage<br/>

### **CancellationToken**

Returns the CancellationToken for the context (implicit interface)

```csharp
public CancellationToken CancellationToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **MessageId**

```csharp
public Nullable<Guid> MessageId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **RequestId**

```csharp
public Nullable<Guid> RequestId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **CorrelationId**

```csharp
public Nullable<Guid> CorrelationId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ConversationId**

```csharp
public Nullable<Guid> ConversationId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **InitiatorId**

```csharp
public Nullable<Guid> InitiatorId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ExpirationTime**

```csharp
public Nullable<DateTime> ExpirationTime { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **SourceAddress**

```csharp
public Uri SourceAddress { get; }
```

#### Property Value

Uri<br/>

### **DestinationAddress**

```csharp
public Uri DestinationAddress { get; }
```

#### Property Value

Uri<br/>

### **ResponseAddress**

```csharp
public Uri ResponseAddress { get; }
```

#### Property Value

Uri<br/>

### **FaultAddress**

```csharp
public Uri FaultAddress { get; }
```

#### Property Value

Uri<br/>

### **SentTime**

```csharp
public Nullable<DateTime> SentTime { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Headers**

```csharp
public Headers Headers { get; }
```

#### Property Value

[Headers](../../masstransit-abstractions/masstransit/headers)<br/>

### **Host**

```csharp
public HostInfo Host { get; }
```

#### Property Value

[HostInfo](../../masstransit-abstractions/masstransit/hostinfo)<br/>

### **ConsumeCompleted**

```csharp
public Task ConsumeCompleted { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SupportedMessageTypes**

```csharp
public IEnumerable<string> SupportedMessageTypes { get; }
```

#### Property Value

[IEnumerable\<String\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **ReceiveContext**

```csharp
public ReceiveContext ReceiveContext { get; protected set; }
```

#### Property Value

[ReceiveContext](../../masstransit-abstractions/masstransit/receivecontext)<br/>

### **SerializerContext**

```csharp
public SerializerContext SerializerContext { get; }
```

#### Property Value

[SerializerContext](../../masstransit-abstractions/masstransit/serializercontext)<br/>

## Methods

### **SetConsumed()**

```csharp
public abstract Task SetConsumed()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SetDelivered()**

```csharp
public abstract Task SetDelivered()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **LoadOutboxMessages()**

```csharp
public abstract Task<List<OutboxMessageContext>> LoadOutboxMessages()
```

#### Returns

[Task\<List\<OutboxMessageContext\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **NotifyOutboxMessageDelivered(OutboxMessageContext)**

```csharp
public abstract Task NotifyOutboxMessageDelivered(OutboxMessageContext message)
```

#### Parameters

`message` [OutboxMessageContext](../masstransit-middleware/outboxmessagecontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **RemoveOutboxMessages()**

```csharp
public abstract Task RemoveOutboxMessages()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **AddSend\<T\>(SendContext\<T\>)**

```csharp
public abstract Task AddSend<T>(SendContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [SendContext\<T\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **GetService(Type)**

```csharp
public object GetService(Type serviceType)
```

#### Parameters

`serviceType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
