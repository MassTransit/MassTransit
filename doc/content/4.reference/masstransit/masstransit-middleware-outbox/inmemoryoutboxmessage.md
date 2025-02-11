---

title: InMemoryOutboxMessage

---

# InMemoryOutboxMessage

Namespace: MassTransit.Middleware.Outbox

```csharp
public class InMemoryOutboxMessage : OutboxMessageContext, MessageContext
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InMemoryOutboxMessage](../masstransit-middleware-outbox/inmemoryoutboxmessage)<br/>
Implements [OutboxMessageContext](../masstransit-middleware/outboxmessagecontext), [MessageContext](../../masstransit-abstractions/masstransit/messagecontext)

## Properties

### **EnqueueTime**

When the message should be visible / ready to be delivered

```csharp
public Nullable<DateTime> EnqueueTime { get; set; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **SentTime**

```csharp
public DateTime SentTime { get; set; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **Headers**

```csharp
public string Headers { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Properties**

Transport-specific message properties (routing key, partition key, sessionId, etc.)

```csharp
public string Properties { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **SequenceNumber**

```csharp
public long SequenceNumber { get; set; }
```

#### Property Value

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

### **MessageId**

```csharp
public Guid MessageId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **ContentType**

```csharp
public string ContentType { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **MessageType**

```csharp
public string MessageType { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Body**

```csharp
public string Body { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ConversationId**

```csharp
public Nullable<Guid> ConversationId { get; set; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **CorrelationId**

```csharp
public Nullable<Guid> CorrelationId { get; set; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **InitiatorId**

```csharp
public Nullable<Guid> InitiatorId { get; set; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **RequestId**

```csharp
public Nullable<Guid> RequestId { get; set; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

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

### **ExpirationTime**

```csharp
public Nullable<DateTime> ExpirationTime { get; set; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **InMemoryOutboxMessage()**

```csharp
public InMemoryOutboxMessage()
```

## Methods

### **Deserialize(IObjectDeserializer)**

```csharp
public void Deserialize(IObjectDeserializer deserializer)
```

#### Parameters

`deserializer` [IObjectDeserializer](../../masstransit-abstractions/masstransit/iobjectdeserializer)<br/>
