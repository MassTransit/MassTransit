---

title: ReceiveTransportCompleted

---

# ReceiveTransportCompleted

Namespace: MassTransit

```csharp
public interface ReceiveTransportCompleted : ReceiveTransportEvent
```

Implements [ReceiveTransportEvent](../masstransit/receivetransportevent)

## Properties

### **DeliveryCount**

The number of messages delivered to the receive endpoint

```csharp
public abstract long DeliveryCount { get; }
```

#### Property Value

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

### **ConcurrentDeliveryCount**

The maximum concurrent messages delivery to the receive endpoint

```csharp
public abstract long ConcurrentDeliveryCount { get; }
```

#### Property Value

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>
