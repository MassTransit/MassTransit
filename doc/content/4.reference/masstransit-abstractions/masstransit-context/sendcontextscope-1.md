---

title: SendContextScope<TMessage>

---

# SendContextScope\<TMessage\>

Namespace: MassTransit.Context

```csharp
public class SendContextScope<TMessage> : SendContextScope, SendContext<TMessage>, SendContext, PipeContext
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ProxyPipeContext](../masstransit-middleware/proxypipecontext) → [SendContextProxy](../masstransit-context/sendcontextproxy) → [SendContextScope](../masstransit-context/sendcontextscope) → [SendContextScope\<TMessage\>](../masstransit-context/sendcontextscope-1)<br/>
Implements [SendContext\<TMessage\>](../masstransit/sendcontext-1), [SendContext](../masstransit/sendcontext), [PipeContext](../masstransit/pipecontext)

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

### **RequestId**

```csharp
public Nullable<Guid> RequestId { get; set; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **MessageId**

```csharp
public Nullable<Guid> MessageId { get; set; }
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

[SendHeaders](../masstransit/sendheaders)<br/>

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

### **Durable**

```csharp
public bool Durable { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Delay**

```csharp
public Nullable<TimeSpan> Delay { get; set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Serializer**

```csharp
public IMessageSerializer Serializer { get; set; }
```

#### Property Value

[IMessageSerializer](../masstransit/imessageserializer)<br/>

### **Serialization**

```csharp
public ISerialization Serialization { get; set; }
```

#### Property Value

[ISerialization](../masstransit/iserialization)<br/>

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

## Constructors

### **SendContextScope(SendContext\<TMessage\>)**

```csharp
public SendContextScope(SendContext<TMessage> context)
```

#### Parameters

`context` [SendContext\<TMessage\>](../masstransit/sendcontext-1)<br/>

### **SendContextScope(SendContext\<TMessage\>, Object[])**

```csharp
public SendContextScope(SendContext<TMessage> context, Object[] payloads)
```

#### Parameters

`context` [SendContext\<TMessage\>](../masstransit/sendcontext-1)<br/>

`payloads` [Object[]](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
