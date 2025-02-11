---

title: RoutingSlipSubscription

---

# RoutingSlipSubscription

Namespace: MassTransit.Courier.Messages

```csharp
public class RoutingSlipSubscription : Subscription
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RoutingSlipSubscription](../masstransit-courier-messages/routingslipsubscription)<br/>
Implements [Subscription](../masstransit-courier-contracts/subscription)

## Properties

### **Address**

```csharp
public Uri Address { get; set; }
```

#### Property Value

Uri<br/>

### **Events**

```csharp
public RoutingSlipEvents Events { get; set; }
```

#### Property Value

[RoutingSlipEvents](../masstransit-courier-contracts/routingslipevents)<br/>

### **Include**

```csharp
public RoutingSlipEventContents Include { get; set; }
```

#### Property Value

[RoutingSlipEventContents](../masstransit-courier-contracts/routingslipeventcontents)<br/>

### **Message**

```csharp
public MessageEnvelope Message { get; set; }
```

#### Property Value

[MessageEnvelope](../masstransit-serialization/messageenvelope)<br/>

### **ActivityName**

```csharp
public string ActivityName { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **RoutingSlipSubscription()**

```csharp
public RoutingSlipSubscription()
```

### **RoutingSlipSubscription(Uri, RoutingSlipEvents, RoutingSlipEventContents, String, MessageEnvelope)**

```csharp
public RoutingSlipSubscription(Uri address, RoutingSlipEvents events, RoutingSlipEventContents include, string activityName, MessageEnvelope message)
```

#### Parameters

`address` Uri<br/>

`events` [RoutingSlipEvents](../masstransit-courier-contracts/routingslipevents)<br/>

`include` [RoutingSlipEventContents](../masstransit-courier-contracts/routingslipeventcontents)<br/>

`activityName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`message` [MessageEnvelope](../masstransit-serialization/messageenvelope)<br/>

### **RoutingSlipSubscription(Subscription)**

```csharp
public RoutingSlipSubscription(Subscription subscription)
```

#### Parameters

`subscription` [Subscription](../masstransit-courier-contracts/subscription)<br/>
