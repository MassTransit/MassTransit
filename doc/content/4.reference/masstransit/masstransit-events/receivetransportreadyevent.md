---

title: ReceiveTransportReadyEvent

---

# ReceiveTransportReadyEvent

Namespace: MassTransit.Events

```csharp
public class ReceiveTransportReadyEvent : ReceiveTransportReady, ReceiveTransportEvent
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReceiveTransportReadyEvent](../masstransit-events/receivetransportreadyevent)<br/>
Implements [ReceiveTransportReady](../../masstransit-abstractions/masstransit/receivetransportready), [ReceiveTransportEvent](../../masstransit-abstractions/masstransit/receivetransportevent)

## Properties

### **InputAddress**

```csharp
public Uri InputAddress { get; }
```

#### Property Value

Uri<br/>

### **IsStarted**

```csharp
public bool IsStarted { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **ReceiveTransportReadyEvent(Uri, Boolean)**

```csharp
public ReceiveTransportReadyEvent(Uri inputAddress, bool isStarted)
```

#### Parameters

`inputAddress` Uri<br/>

`isStarted` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
