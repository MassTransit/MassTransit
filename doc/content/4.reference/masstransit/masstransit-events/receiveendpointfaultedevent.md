---

title: ReceiveEndpointFaultedEvent

---

# ReceiveEndpointFaultedEvent

Namespace: MassTransit.Events

```csharp
public class ReceiveEndpointFaultedEvent : ReceiveEndpointFaulted, ReceiveEndpointEvent
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReceiveEndpointFaultedEvent](../masstransit-events/receiveendpointfaultedevent)<br/>
Implements [ReceiveEndpointFaulted](../../masstransit-abstractions/masstransit/receiveendpointfaulted), [ReceiveEndpointEvent](../../masstransit-abstractions/masstransit/receiveendpointevent)

## Properties

### **InputAddress**

```csharp
public Uri InputAddress { get; }
```

#### Property Value

Uri<br/>

### **Exception**

```csharp
public Exception Exception { get; }
```

#### Property Value

[Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

### **ReceiveEndpoint**

```csharp
public IReceiveEndpoint ReceiveEndpoint { get; }
```

#### Property Value

[IReceiveEndpoint](../../masstransit-abstractions/masstransit/ireceiveendpoint)<br/>

## Constructors

### **ReceiveEndpointFaultedEvent(ReceiveTransportFaulted, IReceiveEndpoint)**

```csharp
public ReceiveEndpointFaultedEvent(ReceiveTransportFaulted faulted, IReceiveEndpoint receiveEndpoint)
```

#### Parameters

`faulted` [ReceiveTransportFaulted](../../masstransit-abstractions/masstransit/receivetransportfaulted)<br/>

`receiveEndpoint` [IReceiveEndpoint](../../masstransit-abstractions/masstransit/ireceiveendpoint)<br/>
