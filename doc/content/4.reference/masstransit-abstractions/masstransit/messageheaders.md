---

title: MessageHeaders

---

# MessageHeaders

Namespace: MassTransit

```csharp
public static class MessageHeaders
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageHeaders](../masstransit/messageheaders)

## Fields

### **Reason**

The reason for a message action being taken

```csharp
public static string Reason;
```

### **FaultExceptionType**

The type of exception from a Fault

```csharp
public static string FaultExceptionType;
```

### **FaultInputAddress**

The input address of the endpoint on which the fault occurred

```csharp
public static string FaultInputAddress;
```

### **FaultMessage**

The exception message from a Fault

```csharp
public static string FaultMessage;
```

### **FaultMessageType**

The message type from a Fault

```csharp
public static string FaultMessageType;
```

### **FaultConsumerType**

The consumer type which faulted

```csharp
public static string FaultConsumerType;
```

### **FaultTimestamp**

The timestamp when the fault occurred

```csharp
public static string FaultTimestamp;
```

### **FaultStackTrace**

The stack trace from a Fault

```csharp
public static string FaultStackTrace;
```

### **FaultRetryCount**

The number of times the message was retried

```csharp
public static string FaultRetryCount;
```

### **FaultRedeliveryCount**

The number of times the message was redelivered

```csharp
public static string FaultRedeliveryCount;
```

### **ForwarderAddress**

The endpoint that forwarded the message to the new destination

```csharp
public static string ForwarderAddress;
```

### **SchedulingTokenId**

The tokenId for the message that was registered with the scheduler

```csharp
public static string SchedulingTokenId;
```

### **RedeliveryCount**

The number of times the message has been redelivered (zero if never)

```csharp
public static string RedeliveryCount;
```

### **QuartzTriggerKey**

The trigger key that was used when the scheduled message was trigger

```csharp
public static string QuartzTriggerKey;
```

### **ClientId**

Identifies the client from which the request is being sent

```csharp
public static string ClientId;
```

### **EndpointId**

Identifies the endpoint that handled the request

```csharp
public static string EndpointId;
```

### **InitiatingConversationId**

The initiating conversation id if a new conversation was started by this message

```csharp
public static string InitiatingConversationId;
```

### **MessageId**

MessageId - [MessageEnvelope](../masstransit-serialization/messageenvelope)

```csharp
public static string MessageId;
```

### **CorrelationId**

CorrelationId - [MessageEnvelope](../masstransit-serialization/messageenvelope)

```csharp
public static string CorrelationId;
```

### **ConversationId**

ConversationId - [MessageEnvelope](../masstransit-serialization/messageenvelope)

```csharp
public static string ConversationId;
```

### **RequestId**

RequestId - [MessageEnvelope](../masstransit-serialization/messageenvelope)

```csharp
public static string RequestId;
```

### **InitiatorId**

InitiatorId - [MessageEnvelope](../masstransit-serialization/messageenvelope)

```csharp
public static string InitiatorId;
```

### **SourceAddress**

SourceAddress - [MessageEnvelope](../masstransit-serialization/messageenvelope)

```csharp
public static string SourceAddress;
```

### **ResponseAddress**

ResponseAddress - [MessageEnvelope](../masstransit-serialization/messageenvelope)

```csharp
public static string ResponseAddress;
```

### **FaultAddress**

FaultAddress - [MessageEnvelope](../masstransit-serialization/messageenvelope)

```csharp
public static string FaultAddress;
```

### **MessageType**

MessageType - [MessageEnvelope](../masstransit-serialization/messageenvelope)

```csharp
public static string MessageType;
```

### **TransportMessageId**

The Transport message ID, which is a string, because we can't assume anything

```csharp
public static string TransportMessageId;
```

### **TransportSentTime**

The Transport sent time (not supported by all, but hopefully enough)

```csharp
public static string TransportSentTime;
```

### **OriginalMessageId**

When the message is redelivered or scheduled, and a new MessageId was generated, the original messageId

```csharp
public static string OriginalMessageId;
```

### **ContentType**

When a transport header is used, this is the name

```csharp
public static string ContentType;
```

### **FutureId**

Used in routing slip variables to store the correlationId of a future

```csharp
public static string FutureId;
```
