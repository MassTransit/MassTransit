---

title: SqlTransportMessage

---

# SqlTransportMessage

Namespace: MassTransit.SqlTransport

```csharp
public class SqlTransportMessage
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SqlTransportMessage](../masstransit-sqltransport/sqltransportmessage)

## Properties

### **TransportMessageId**

```csharp
public Guid TransportMessageId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **QueueName**

```csharp
public string QueueName { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Priority**

```csharp
public short Priority { get; set; }
```

#### Property Value

[Int16](https://learn.microsoft.com/en-us/dotnet/api/system.int16)<br/>

### **MessageDeliveryId**

```csharp
public long MessageDeliveryId { get; set; }
```

#### Property Value

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

### **ConsumerId**

```csharp
public Nullable<Guid> ConsumerId { get; set; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **LockId**

```csharp
public Nullable<Guid> LockId { get; set; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **EnqueueTime**

```csharp
public DateTime EnqueueTime { get; set; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **DeliveryCount**

```csharp
public int DeliveryCount { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **PartitionKey**

```csharp
public string PartitionKey { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **RoutingKey**

```csharp
public string RoutingKey { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **TransportHeaders**

```csharp
public string TransportHeaders { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

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

### **BinaryBody**

```csharp
public Byte[] BinaryBody { get; set; }
```

#### Property Value

[Byte[]](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

### **Headers**

```csharp
public string Headers { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Host**

```csharp
public string Host { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

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

### **ExpirationTime**

```csharp
public Nullable<DateTime> ExpirationTime { get; set; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

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

### **SentTime**

```csharp
public Nullable<DateTime> SentTime { get; set; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Constructors

### **SqlTransportMessage()**

```csharp
public SqlTransportMessage()
```

## Methods

### **GetHeaders()**

```csharp
public SendHeaders GetHeaders()
```

#### Returns

[SendHeaders](../../masstransit-abstractions/masstransit/sendheaders)<br/>

### **GetTransportHeaders()**

```csharp
public SendHeaders GetTransportHeaders()
```

#### Returns

[SendHeaders](../../masstransit-abstractions/masstransit/sendheaders)<br/>

### **DeserializeHeaders(String)**

```csharp
public static SendHeaders DeserializeHeaders(string jsonHeaders)
```

#### Parameters

`jsonHeaders` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[SendHeaders](../../masstransit-abstractions/masstransit/sendheaders)<br/>
