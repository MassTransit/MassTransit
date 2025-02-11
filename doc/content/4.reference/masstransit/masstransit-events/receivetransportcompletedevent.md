---

title: ReceiveTransportCompletedEvent

---

# ReceiveTransportCompletedEvent

Namespace: MassTransit.Events

```csharp
public class ReceiveTransportCompletedEvent : ReceiveTransportCompleted, ReceiveTransportEvent
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReceiveTransportCompletedEvent](../masstransit-events/receivetransportcompletedevent)<br/>
Implements [ReceiveTransportCompleted](../../masstransit-abstractions/masstransit/receivetransportcompleted), [ReceiveTransportEvent](../../masstransit-abstractions/masstransit/receivetransportevent)

## Properties

### **InputAddress**

```csharp
public Uri InputAddress { get; }
```

#### Property Value

Uri<br/>

### **DeliveryCount**

```csharp
public long DeliveryCount { get; }
```

#### Property Value

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

### **ConcurrentDeliveryCount**

```csharp
public long ConcurrentDeliveryCount { get; }
```

#### Property Value

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

## Constructors

### **ReceiveTransportCompletedEvent(Uri, DeliveryMetrics)**

```csharp
public ReceiveTransportCompletedEvent(Uri inputAddress, DeliveryMetrics metrics)
```

#### Parameters

`inputAddress` Uri<br/>

`metrics` [DeliveryMetrics](../masstransit-transports/deliverymetrics)<br/>
