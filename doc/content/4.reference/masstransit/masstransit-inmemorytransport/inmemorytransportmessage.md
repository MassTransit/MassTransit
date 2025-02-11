---

title: InMemoryTransportMessage

---

# InMemoryTransportMessage

Namespace: MassTransit.InMemoryTransport

```csharp
public class InMemoryTransportMessage
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InMemoryTransportMessage](../masstransit-inmemorytransport/inmemorytransportmessage)

## Properties

### **SequenceNumber**

```csharp
public long SequenceNumber { get; }
```

#### Property Value

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

### **MessageId**

```csharp
public Guid MessageId { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Body**

```csharp
public Byte[] Body { get; }
```

#### Property Value

[Byte[]](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

### **DeliveryCount**

```csharp
public int DeliveryCount { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Headers**

```csharp
public SendHeaders Headers { get; }
```

#### Property Value

[SendHeaders](../../masstransit-abstractions/masstransit/sendheaders)<br/>

### **Delay**

```csharp
public Nullable<TimeSpan> Delay { get; set; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **RoutingKey**

```csharp
public string RoutingKey { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **InMemoryTransportMessage(Guid, Byte[], String)**

```csharp
public InMemoryTransportMessage(Guid messageId, Byte[] body, string contentType)
```

#### Parameters

`messageId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`body` [Byte[]](https://learn.microsoft.com/en-us/dotnet/api/system.byte)<br/>

`contentType` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
