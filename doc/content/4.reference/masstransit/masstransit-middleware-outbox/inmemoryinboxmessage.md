---

title: InMemoryInboxMessage

---

# InMemoryInboxMessage

Namespace: MassTransit.Middleware.Outbox

```csharp
public class InMemoryInboxMessage
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InMemoryInboxMessage](../masstransit-middleware-outbox/inmemoryinboxmessage)

## Properties

### **MessageId**

The MessageId of the incoming message

```csharp
public Guid MessageId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **ConsumerId**

And MD5 hash of the endpoint name + consumer type

```csharp
public Guid ConsumerId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Received**

When the message was first received

```csharp
public DateTime Received { get; set; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **ReceiveCount**

How many times the message has been received

```csharp
public int ReceiveCount { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ExpirationTime**

If present, when the message expires (from the message header)

```csharp
public Nullable<DateTime> ExpirationTime { get; set; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Consumed**

When the message was consumed, successfully

```csharp
public Nullable<DateTime> Consumed { get; set; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Delivered**

When all messages in the outbox were delivered to the transport

```csharp
public Nullable<DateTime> Delivered { get; set; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **LastSequenceNumber**

The last sequence number that was successfully delivered to the transport

```csharp
public Nullable<long> LastSequenceNumber { get; set; }
```

#### Property Value

[Nullable\<Int64\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **InMemoryInboxMessage(Guid, Guid)**

```csharp
public InMemoryInboxMessage(Guid messageId, Guid consumerId)
```

#### Parameters

`messageId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`consumerId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

## Methods

### **MarkInUse(CancellationToken)**

```csharp
public Task MarkInUse(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Release()**

```csharp
public void Release()
```

### **GetOutboxMessages()**

```csharp
public List<InMemoryOutboxMessage> GetOutboxMessages()
```

#### Returns

[List\<InMemoryOutboxMessage\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br/>

### **RemoveOutboxMessages()**

```csharp
public void RemoveOutboxMessages()
```

### **AddOutboxMessage(InMemoryOutboxMessage)**

```csharp
public void AddOutboxMessage(InMemoryOutboxMessage outboxMessage)
```

#### Parameters

`outboxMessage` [InMemoryOutboxMessage](../masstransit-middleware-outbox/inmemoryoutboxmessage)<br/>
