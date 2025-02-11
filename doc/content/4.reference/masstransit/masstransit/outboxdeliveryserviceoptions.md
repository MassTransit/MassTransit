---

title: OutboxDeliveryServiceOptions

---

# OutboxDeliveryServiceOptions

Namespace: MassTransit

```csharp
public class OutboxDeliveryServiceOptions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [OutboxDeliveryServiceOptions](../masstransit/outboxdeliveryserviceoptions)

## Properties

### **MessageDeliveryLimit**

The number of message to deliver at a time from an individual outbox

```csharp
public int MessageDeliveryLimit { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **MessageDeliveryTimeout**

Transport Send timeout when delivering messages to the transport

```csharp
public TimeSpan MessageDeliveryTimeout { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **QueryDelay**

Delay between each database sweep to load outbox messages

```csharp
public TimeSpan QueryDelay { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **QueryMessageLimit**

The number of outboxes to deliver concurrently (set to 1 if you need in-order delivery across multiple transactions)

```csharp
public int QueryMessageLimit { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **QueryTimeout**

Database query timeout for loading outbox messages

```csharp
public TimeSpan QueryTimeout { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

## Constructors

### **OutboxDeliveryServiceOptions()**

```csharp
public OutboxDeliveryServiceOptions()
```
