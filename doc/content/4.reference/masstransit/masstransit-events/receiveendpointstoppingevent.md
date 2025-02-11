---

title: ReceiveEndpointStoppingEvent

---

# ReceiveEndpointStoppingEvent

Namespace: MassTransit.Events

```csharp
public class ReceiveEndpointStoppingEvent : ReceiveEndpointStopping, ReceiveEndpointEvent
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReceiveEndpointStoppingEvent](../masstransit-events/receiveendpointstoppingevent)<br/>
Implements [ReceiveEndpointStopping](../../masstransit-abstractions/masstransit/receiveendpointstopping), [ReceiveEndpointEvent](../../masstransit-abstractions/masstransit/receiveendpointevent)

## Properties

### **Removed**

```csharp
public bool Removed { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **InputAddress**

```csharp
public Uri InputAddress { get; }
```

#### Property Value

Uri<br/>

### **ReceiveEndpoint**

```csharp
public IReceiveEndpoint ReceiveEndpoint { get; }
```

#### Property Value

[IReceiveEndpoint](../../masstransit-abstractions/masstransit/ireceiveendpoint)<br/>

## Constructors

### **ReceiveEndpointStoppingEvent(Uri, IReceiveEndpoint, Boolean)**

```csharp
public ReceiveEndpointStoppingEvent(Uri inputAddress, IReceiveEndpoint receiveEndpoint, bool removed)
```

#### Parameters

`inputAddress` Uri<br/>

`receiveEndpoint` [IReceiveEndpoint](../../masstransit-abstractions/masstransit/ireceiveendpoint)<br/>

`removed` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
