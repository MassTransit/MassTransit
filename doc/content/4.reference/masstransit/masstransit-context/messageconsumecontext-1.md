---

title: MessageConsumeContext<TMessage>

---

# MessageConsumeContext\<TMessage\>

Namespace: MassTransit.Context

```csharp
public class MessageConsumeContext<TMessage> : ConsumeContext<TMessage>, ConsumeContext, PipeContext, MessageContext, IPublishEndpoint, IPublishObserverConnector, ISendEndpointProvider, ISendObserverConnector
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageConsumeContext\<TMessage\>](../masstransit-context/messageconsumecontext-1)<br/>
Implements [ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1), [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext), [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [MessageContext](../../masstransit-abstractions/masstransit/messagecontext), [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector)

## Properties

### **Message**

```csharp
public TMessage Message { get; }
```

#### Property Value

TMessage<br/>

### **CancellationToken**

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

### **ReceiveContext**

```csharp
public ReceiveContext ReceiveContext { get; }
```

#### Property Value

[ReceiveContext](../../masstransit-abstractions/masstransit/receivecontext)<br/>

### **SerializerContext**

```csharp
public SerializerContext SerializerContext { get; }
```

#### Property Value

[SerializerContext](../../masstransit-abstractions/masstransit/serializercontext)<br/>

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

## Constructors

### **MessageConsumeContext(ConsumeContext, TMessage)**

```csharp
public MessageConsumeContext(ConsumeContext context, TMessage message)
```

#### Parameters

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`message` TMessage<br/>

## Methods

### **NotifyConsumed(TimeSpan, String)**

```csharp
public Task NotifyConsumed(TimeSpan duration, string consumerType)
```

#### Parameters

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`consumerType` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **NotifyFaulted(TimeSpan, String, Exception)**

```csharp
public Task NotifyFaulted(TimeSpan duration, string consumerType, Exception exception)
```

#### Parameters

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`consumerType` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **HasPayloadType(Type)**

```csharp
public bool HasPayloadType(Type payloadType)
```

#### Parameters

`payloadType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetPayload\<T\>(T)**

```csharp
public bool TryGetPayload<T>(out T payload)
```

#### Type Parameters

`T`<br/>

#### Parameters

`payload` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetOrAddPayload\<T\>(PayloadFactory\<T\>)**

```csharp
public T GetOrAddPayload<T>(PayloadFactory<T> payloadFactory)
```

#### Type Parameters

`T`<br/>

#### Parameters

`payloadFactory` [PayloadFactory\<T\>](../../masstransit-abstractions/masstransit/payloadfactory-1)<br/>

#### Returns

T<br/>

### **AddOrUpdatePayload\<T\>(PayloadFactory\<T\>, UpdatePayloadFactory\<T\>)**

```csharp
public T AddOrUpdatePayload<T>(PayloadFactory<T> addFactory, UpdatePayloadFactory<T> updateFactory)
```

#### Type Parameters

`T`<br/>

#### Parameters

`addFactory` [PayloadFactory\<T\>](../../masstransit-abstractions/masstransit/payloadfactory-1)<br/>

`updateFactory` [UpdatePayloadFactory\<T\>](../../masstransit-abstractions/masstransit/updatepayloadfactory-1)<br/>

#### Returns

T<br/>

### **ConnectPublishObserver(IPublishObserver)**

```csharp
public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
```

#### Parameters

`observer` [IPublishObserver](../../masstransit-abstractions/masstransit/ipublishobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **Publish\<T\>(T, CancellationToken)**

```csharp
public Task Publish<T>(T message, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`message` T<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Publish\<T\>(T, IPipe\<PublishContext\<T\>\>, CancellationToken)**

```csharp
public Task Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`message` T<br/>

`publishPipe` [IPipe\<PublishContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Publish\<T\>(T, IPipe\<PublishContext\>, CancellationToken)**

```csharp
public Task Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`message` T<br/>

`publishPipe` [IPipe\<PublishContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Publish(Object, CancellationToken)**

```csharp
public Task Publish(object message, CancellationToken cancellationToken)
```

#### Parameters

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Publish(Object, IPipe\<PublishContext\>, CancellationToken)**

```csharp
public Task Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
```

#### Parameters

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`publishPipe` [IPipe\<PublishContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Publish(Object, Type, CancellationToken)**

```csharp
public Task Publish(object message, Type messageType, CancellationToken cancellationToken)
```

#### Parameters

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Publish(Object, Type, IPipe\<PublishContext\>, CancellationToken)**

```csharp
public Task Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
```

#### Parameters

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`publishPipe` [IPipe\<PublishContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Publish\<T\>(Object, CancellationToken)**

```csharp
public Task Publish<T>(object values, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Publish\<T\>(Object, IPipe\<PublishContext\<T\>\>, CancellationToken)**

```csharp
public Task Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`publishPipe` [IPipe\<PublishContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Publish\<T\>(Object, IPipe\<PublishContext\>, CancellationToken)**

```csharp
public Task Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`publishPipe` [IPipe\<PublishContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ConnectSendObserver(ISendObserver)**

```csharp
public ConnectHandle ConnectSendObserver(ISendObserver observer)
```

#### Parameters

`observer` [ISendObserver](../../masstransit-abstractions/masstransit/isendobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **GetSendEndpoint(Uri)**

```csharp
public Task<ISendEndpoint> GetSendEndpoint(Uri address)
```

#### Parameters

`address` Uri<br/>

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **HasMessageType(Type)**

```csharp
public bool HasMessageType(Type messageType)
```

#### Parameters

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetMessage\<T\>(ConsumeContext\<T\>)**

```csharp
public bool TryGetMessage<T>(out ConsumeContext<T> consumeContext)
```

#### Type Parameters

`T`<br/>

#### Parameters

`consumeContext` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **AddConsumeTask(Task)**

```csharp
public void AddConsumeTask(Task task)
```

#### Parameters

`task` [Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **RespondAsync\<T\>(T)**

```csharp
public Task RespondAsync<T>(T message)
```

#### Type Parameters

`T`<br/>

#### Parameters

`message` T<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **RespondAsync\<T\>(T, IPipe\<SendContext\<T\>\>)**

```csharp
public Task RespondAsync<T>(T message, IPipe<SendContext<T>> sendPipe)
```

#### Type Parameters

`T`<br/>

#### Parameters

`message` T<br/>

`sendPipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **RespondAsync\<T\>(T, IPipe\<SendContext\>)**

```csharp
public Task RespondAsync<T>(T message, IPipe<SendContext> sendPipe)
```

#### Type Parameters

`T`<br/>

#### Parameters

`message` T<br/>

`sendPipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **RespondAsync(Object)**

```csharp
public Task RespondAsync(object message)
```

#### Parameters

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **RespondAsync(Object, Type)**

```csharp
public Task RespondAsync(object message, Type messageType)
```

#### Parameters

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **RespondAsync(Object, IPipe\<SendContext\>)**

```csharp
public Task RespondAsync(object message, IPipe<SendContext> sendPipe)
```

#### Parameters

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`sendPipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **RespondAsync(Object, Type, IPipe\<SendContext\>)**

```csharp
public Task RespondAsync(object message, Type messageType, IPipe<SendContext> sendPipe)
```

#### Parameters

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`sendPipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **RespondAsync\<T\>(Object)**

```csharp
public Task RespondAsync<T>(object values)
```

#### Type Parameters

`T`<br/>

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **RespondAsync\<T\>(Object, IPipe\<SendContext\<T\>\>)**

```csharp
public Task RespondAsync<T>(object values, IPipe<SendContext<T>> sendPipe)
```

#### Type Parameters

`T`<br/>

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`sendPipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **RespondAsync\<T\>(Object, IPipe\<SendContext\>)**

```csharp
public Task RespondAsync<T>(object values, IPipe<SendContext> sendPipe)
```

#### Type Parameters

`T`<br/>

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`sendPipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Respond\<T\>(T)**

```csharp
public void Respond<T>(T message)
```

#### Type Parameters

`T`<br/>

#### Parameters

`message` T<br/>

### **NotifyConsumed\<T\>(ConsumeContext\<T\>, TimeSpan, String)**

```csharp
public Task NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`consumerType` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **NotifyFaulted\<T\>(ConsumeContext\<T\>, TimeSpan, String, Exception)**

```csharp
public Task NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`consumerType` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
