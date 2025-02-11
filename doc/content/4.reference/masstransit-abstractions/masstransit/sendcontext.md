---

title: SendContext

---

# SendContext

Namespace: MassTransit

Unlike the old world, the send context is returned from the endpoint and used to configure the message sending.
 That way the message is captured by the endpoint and then any configuration is done at the higher level.

```csharp
public interface SendContext : PipeContext
```

Implements [PipeContext](../masstransit/pipecontext)

## Properties

### **SourceAddress**

```csharp
public abstract Uri SourceAddress { get; set; }
```

#### Property Value

Uri<br/>

### **DestinationAddress**

```csharp
public abstract Uri DestinationAddress { get; set; }
```

#### Property Value

Uri<br/>

### **ResponseAddress**

```csharp
public abstract Uri ResponseAddress { get; set; }
```

#### Property Value

Uri<br/>

### **FaultAddress**

```csharp
public abstract Uri FaultAddress { get; set; }
```

#### Property Value

Uri<br/>

### **RequestId**

```csharp
public abstract Nullable<Guid> RequestId { get; set; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **MessageId**

```csharp
public abstract Nullable<Guid> MessageId { get; set; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **CorrelationId**

```csharp
public abstract Nullable<Guid> CorrelationId { get; set; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ConversationId**

```csharp
public abstract Nullable<Guid> ConversationId { get; set; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **InitiatorId**

```csharp
public abstract Nullable<Guid> InitiatorId { get; set; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ScheduledMessageId**

```csharp
public abstract Nullable<Guid> ScheduledMessageId { get; set; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Headers**

```csharp
public abstract SendHeaders Headers { get; }
```

#### Property Value

[SendHeaders](../masstransit/sendheaders)<br/>

### **TimeToLive**

```csharp
public abstract Nullable<TimeSpan> TimeToLive { get; set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **SentTime**

```csharp
public abstract Nullable<DateTime> SentTime { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ContentType**

```csharp
public abstract ContentType ContentType { get; set; }
```

#### Property Value

ContentType<br/>

### **Durable**

True if the message should be persisted to disk to survive a broker restart

```csharp
public abstract bool Durable { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Delay**

If specified, the message delivery will be delayed by the transport (if supported)

```csharp
public abstract Nullable<TimeSpan> Delay { get; set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Serializer**

The serializer to use when serializing the message to the transport

```csharp
public abstract IMessageSerializer Serializer { get; set; }
```

#### Property Value

[IMessageSerializer](../masstransit/imessageserializer)<br/>

### **Serialization**

The endpoint configured serialization collection

```csharp
public abstract ISerialization Serialization { get; set; }
```

#### Property Value

[ISerialization](../masstransit/iserialization)<br/>

### **SupportedMessageTypes**

The supported message types for the message being sent/published. For internal use only.

```csharp
public abstract String[] SupportedMessageTypes { get; set; }
```

#### Property Value

[String[]](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **BodyLength**

After serialization, should return the length of the message body

```csharp
public abstract Nullable<long> BodyLength { get; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Methods

### **CreateProxy\<T\>(T)**

Create a send context proxy with the new message type

```csharp
SendContext<T> CreateProxy<T>(T message)
```

#### Type Parameters

`T`<br/>

#### Parameters

`message` T<br/>

#### Returns

[SendContext\<T\>](../masstransit/sendcontext-1)<br/>
