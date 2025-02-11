---

title: MessageDelivery

---

# MessageDelivery

Namespace: MassTransit.SqlTransport

```csharp
public class MessageDelivery
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageDelivery](../masstransit-sqltransport/messagedelivery)

## Properties

### **QueueId**

```csharp
public int QueueId { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Priority**

```csharp
public int Priority { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **EnqueueTime**

```csharp
public DateTimeOffset EnqueueTime { get; set; }
```

#### Property Value

[DateTimeOffset](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset)<br/>

### **ConsumerId**

```csharp
public Nullable<Guid> ConsumerId { get; set; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **TransportMessageId**

```csharp
public Guid TransportMessageId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **ExpirationTime**

```csharp
public Nullable<DateTimeOffset> ExpirationTime { get; set; }
```

#### Property Value

[Nullable\<DateTimeOffset\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **DeliveryCount**

```csharp
public int DeliveryCount { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **MaxDeliveryCount**

```csharp
public int MaxDeliveryCount { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **LastDelivered**

```csharp
public Nullable<DateTimeOffset> LastDelivered { get; set; }
```

#### Property Value

[Nullable\<DateTimeOffset\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **SessionNumber**

```csharp
public long SessionNumber { get; set; }
```

#### Property Value

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

### **ReplyToSessionId**

```csharp
public string ReplyToSessionId { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **GroupId**

```csharp
public string GroupId { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **GroupSequenceNumber**

```csharp
public Nullable<int> GroupSequenceNumber { get; set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **TransportHeaders**

```csharp
public string TransportHeaders { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **MessageDelivery()**

```csharp
public MessageDelivery()
```
