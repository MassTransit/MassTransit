---

title: ReceiveEndpointCompletedEvent

---

# ReceiveEndpointCompletedEvent

Namespace: MassTransit.Events

```csharp
public class ReceiveEndpointCompletedEvent : ReceiveEndpointCompleted, ReceiveEndpointEvent
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReceiveEndpointCompletedEvent](../masstransit-events/receiveendpointcompletedevent)<br/>
Implements [ReceiveEndpointCompleted](../../masstransit-abstractions/masstransit/receiveendpointcompleted), [ReceiveEndpointEvent](../../masstransit-abstractions/masstransit/receiveendpointevent)

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

### **ReceiveEndpoint**

```csharp
public IReceiveEndpoint ReceiveEndpoint { get; }
```

#### Property Value

[IReceiveEndpoint](../../masstransit-abstractions/masstransit/ireceiveendpoint)<br/>

## Constructors

### **ReceiveEndpointCompletedEvent(ReceiveTransportCompleted, IReceiveEndpoint)**

```csharp
public ReceiveEndpointCompletedEvent(ReceiveTransportCompleted completed, IReceiveEndpoint receiveEndpoint)
```

#### Parameters

`completed` [ReceiveTransportCompleted](../../masstransit-abstractions/masstransit/receivetransportcompleted)<br/>

`receiveEndpoint` [IReceiveEndpoint](../../masstransit-abstractions/masstransit/ireceiveendpoint)<br/>
