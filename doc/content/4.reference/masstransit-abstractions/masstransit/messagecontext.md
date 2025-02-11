---

title: MessageContext

---

# MessageContext

Namespace: MassTransit

The message context includes the headers that are transferred with the message

```csharp
public interface MessageContext
```

## Properties

### **MessageId**

The messageId assigned to the message when it was initially Sent. This is different
 than the transport MessageId, which is only for the Transport.

```csharp
public abstract Nullable<Guid> MessageId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **RequestId**

If the message is a request, or related to a request, such as a response or a fault,
 this contains the requestId.

```csharp
public abstract Nullable<Guid> RequestId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **CorrelationId**

If the message implements the CorrelatedBy(Guid) interface, this field should be
 populated by default to match that value. It can, of course, be overwritten with
 something else.

```csharp
public abstract Nullable<Guid> CorrelationId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ConversationId**

The conversationId of the message, which is copied and carried throughout the message
 flow by the infrastructure.

```csharp
public abstract Nullable<Guid> ConversationId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **InitiatorId**

If this message was produced within the context of a previous message, the CorrelationId
 of the message is contained in this property. If the message was produced from a saga
 instance, the CorrelationId of the saga is used.

```csharp
public abstract Nullable<Guid> InitiatorId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ExpirationTime**

The expiration time of the message if it is not intended to last forever.

```csharp
public abstract Nullable<DateTime> ExpirationTime { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **SourceAddress**

The address of the message producer that sent the message

```csharp
public abstract Uri SourceAddress { get; }
```

#### Property Value

Uri<br/>

### **DestinationAddress**

The destination address of the message

```csharp
public abstract Uri DestinationAddress { get; }
```

#### Property Value

Uri<br/>

### **ResponseAddress**

The response address to which responses to the request should be sent

```csharp
public abstract Uri ResponseAddress { get; }
```

#### Property Value

Uri<br/>

### **FaultAddress**

The fault address to which fault events should be sent if the message consumer faults

```csharp
public abstract Uri FaultAddress { get; }
```

#### Property Value

Uri<br/>

### **SentTime**

When the message was originally sent

```csharp
public abstract Nullable<DateTime> SentTime { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Headers**

Additional application-specific headers that are added to the message by the application
 or by features within MassTransit, such as when a message is moved to an error queue.

```csharp
public abstract Headers Headers { get; }
```

#### Property Value

[Headers](../masstransit/headers)<br/>

### **Host**

The host information of the message producer. This may not be present if the message was sent
 from an earlier version of MassTransit.

```csharp
public abstract HostInfo Host { get; }
```

#### Property Value

[HostInfo](../masstransit/hostinfo)<br/>
