---

title: InMemorySendContext<T>

---

# InMemorySendContext\<T\>

Namespace: MassTransit.InMemoryTransport

```csharp
public class InMemorySendContext<T> : MessageSendContext<T>, PipeContext, TransportSendContext<T>, PublishContext<T>, SendContext<T>, SendContext, PublishContext, TransportSendContext, RoutingKeySendContext
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BasePipeContext](../../masstransit-abstractions/masstransit-middleware/basepipecontext) → [MessageSendContext\<T\>](../masstransit-context/messagesendcontext-1) → [InMemorySendContext\<T\>](../masstransit-inmemorytransport/inmemorysendcontext-1)<br/>
Implements [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [TransportSendContext\<T\>](../masstransit-context/transportsendcontext-1), [PublishContext\<T\>](../../masstransit-abstractions/masstransit/publishcontext-1), [SendContext\<T\>](../../masstransit-abstractions/masstransit/sendcontext-1), [SendContext](../../masstransit-abstractions/masstransit/sendcontext), [PublishContext](../../masstransit-abstractions/masstransit/publishcontext), [TransportSendContext](../masstransit-context/transportsendcontext), [RoutingKeySendContext](../../masstransit-abstractions/masstransit/routingkeysendcontext)

## Properties

### **RoutingKey**

```csharp
public string RoutingKey { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **IsPublish**

Set to true if the message is being published

```csharp
public bool IsPublish { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Body**

```csharp
public MessageBody Body { get; }
```

#### Property Value

[MessageBody](../../masstransit-abstractions/masstransit/messagebody)<br/>

### **Delay**

```csharp
public Nullable<TimeSpan> Delay { get; set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **MessageId**

```csharp
public Nullable<Guid> MessageId { get; set; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **RequestId**

```csharp
public Nullable<Guid> RequestId { get; set; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **CorrelationId**

```csharp
public Nullable<Guid> CorrelationId { get; set; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ConversationId**

```csharp
public Nullable<Guid> ConversationId { get; set; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **InitiatorId**

```csharp
public Nullable<Guid> InitiatorId { get; set; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ScheduledMessageId**

```csharp
public Nullable<Guid> ScheduledMessageId { get; set; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Headers**

```csharp
public SendHeaders Headers { get; }
```

#### Property Value

[SendHeaders](../../masstransit-abstractions/masstransit/sendheaders)<br/>

### **SourceAddress**

```csharp
public Uri SourceAddress { get; set; }
```

#### Property Value

Uri<br/>

### **DestinationAddress**

```csharp
public Uri DestinationAddress { get; set; }
```

#### Property Value

Uri<br/>

### **ResponseAddress**

```csharp
public Uri ResponseAddress { get; set; }
```

#### Property Value

Uri<br/>

### **FaultAddress**

```csharp
public Uri FaultAddress { get; set; }
```

#### Property Value

Uri<br/>

### **TimeToLive**

```csharp
public Nullable<TimeSpan> TimeToLive { get; set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **SentTime**

```csharp
public Nullable<DateTime> SentTime { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ContentType**

```csharp
public ContentType ContentType { get; set; }
```

#### Property Value

ContentType<br/>

### **Serializer**

```csharp
public IMessageSerializer Serializer { get; set; }
```

#### Property Value

[IMessageSerializer](../../masstransit-abstractions/masstransit/imessageserializer)<br/>

### **Serialization**

```csharp
public ISerialization Serialization { get; set; }
```

#### Property Value

[ISerialization](../../masstransit-abstractions/masstransit/iserialization)<br/>

### **SupportedMessageTypes**

```csharp
public String[] SupportedMessageTypes { get; set; }
```

#### Property Value

[String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **BodyLength**

```csharp
public Nullable<long> BodyLength { get; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Durable**

```csharp
public bool Durable { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Message**

```csharp
public T Message { get; }
```

#### Property Value

T<br/>

### **Mandatory**

```csharp
public bool Mandatory { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **CancellationToken**

```csharp
public CancellationToken CancellationToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Constructors

### **InMemorySendContext(T, CancellationToken)**

```csharp
public InMemorySendContext(T message, CancellationToken cancellationToken)
```

#### Parameters

`message` T<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Methods

### **ReadPropertiesFrom(IReadOnlyDictionary\<String, Object\>)**

```csharp
public void ReadPropertiesFrom(IReadOnlyDictionary<string, object> properties)
```

#### Parameters

`properties` [IReadOnlyDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2)<br/>

### **WritePropertiesTo(IDictionary\<String, Object\>)**

```csharp
public void WritePropertiesTo(IDictionary<string, object> properties)
```

#### Parameters

`properties` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>
