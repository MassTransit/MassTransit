---

title: OutboxConsumeOptions

---

# OutboxConsumeOptions

Namespace: MassTransit.Middleware

```csharp
public class OutboxConsumeOptions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [OutboxConsumeOptions](../masstransit-middleware/outboxconsumeoptions)

## Properties

### **ConsumerId**

The generated identifier for the consumer based upon endpoint name

```csharp
public Guid ConsumerId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **ConsumerType**

The display name of the consumer type

```csharp
public string ConsumerType { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **MessageDeliveryLimit**

The number of message to deliver at a time from the outbox

```csharp
public int MessageDeliveryLimit { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **MessageDeliveryTimeout**

The time to wait when delivering a message to the broker

```csharp
public TimeSpan MessageDeliveryTimeout { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

## Constructors

### **OutboxConsumeOptions()**

```csharp
public OutboxConsumeOptions()
```
