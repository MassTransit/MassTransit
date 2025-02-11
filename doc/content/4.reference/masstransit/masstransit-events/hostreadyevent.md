---

title: HostReadyEvent

---

# HostReadyEvent

Namespace: MassTransit.Events

```csharp
public class HostReadyEvent : HostReady
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [HostReadyEvent](../masstransit-events/hostreadyevent)<br/>
Implements [HostReady](../../masstransit-abstractions/masstransit/hostready)

## Properties

### **HostAddress**

```csharp
public Uri HostAddress { get; }
```

#### Property Value

Uri<br/>

### **ReceiveEndpoints**

```csharp
public ReceiveEndpointReady[] ReceiveEndpoints { get; }
```

#### Property Value

[ReceiveEndpointReady[]](../../masstransit-abstractions/masstransit/receiveendpointready)<br/>

### **Riders**

```csharp
public RiderReady[] Riders { get; }
```

#### Property Value

[RiderReady[]](../../masstransit-abstractions/masstransit/riderready)<br/>

## Constructors

### **HostReadyEvent(Uri, ReceiveEndpointReady[], RiderReady[])**

```csharp
public HostReadyEvent(Uri hostAddress, ReceiveEndpointReady[] receiveEndpoints, RiderReady[] riders)
```

#### Parameters

`hostAddress` Uri<br/>

`receiveEndpoints` [ReceiveEndpointReady[]](../../masstransit-abstractions/masstransit/receiveendpointready)<br/>

`riders` [RiderReady[]](../../masstransit-abstractions/masstransit/riderready)<br/>
