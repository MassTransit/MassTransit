---

title: ReceiveTransportFaultedEvent

---

# ReceiveTransportFaultedEvent

Namespace: MassTransit.Events

```csharp
public class ReceiveTransportFaultedEvent : ReceiveTransportFaulted, ReceiveTransportEvent
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReceiveTransportFaultedEvent](../masstransit-events/receivetransportfaultedevent)<br/>
Implements [ReceiveTransportFaulted](../../masstransit-abstractions/masstransit/receivetransportfaulted), [ReceiveTransportEvent](../../masstransit-abstractions/masstransit/receivetransportevent)

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

## Constructors

### **ReceiveTransportFaultedEvent(Uri, Exception)**

```csharp
public ReceiveTransportFaultedEvent(Uri inputAddress, Exception exception)
```

#### Parameters

`inputAddress` Uri<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>
