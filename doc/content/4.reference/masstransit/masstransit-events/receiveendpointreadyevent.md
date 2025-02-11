---

title: ReceiveEndpointReadyEvent

---

# ReceiveEndpointReadyEvent

Namespace: MassTransit.Events

```csharp
public class ReceiveEndpointReadyEvent : ReceiveEndpointReady, ReceiveEndpointEvent
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReceiveEndpointReadyEvent](../masstransit-events/receiveendpointreadyevent)<br/>
Implements [ReceiveEndpointReady](../../masstransit-abstractions/masstransit/receiveendpointready), [ReceiveEndpointEvent](../../masstransit-abstractions/masstransit/receiveendpointevent)

## Properties

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

### **IsStarted**

```csharp
public bool IsStarted { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **ReceiveEndpointReadyEvent(Uri, IReceiveEndpoint, Boolean)**

```csharp
public ReceiveEndpointReadyEvent(Uri inputAddress, IReceiveEndpoint receiveEndpoint, bool isStarted)
```

#### Parameters

`inputAddress` Uri<br/>

`receiveEndpoint` [IReceiveEndpoint](../../masstransit-abstractions/masstransit/ireceiveendpoint)<br/>

`isStarted` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
