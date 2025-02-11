---

title: Subscription

---

# Subscription

Namespace: MassTransit.Courier.Contracts

A routing slip subscription defines a specific endpoint where routing
 slip events should be sent (not published). If specified, events are not published.

```csharp
public interface Subscription
```

## Properties

### **Address**

The address where events should be sent

```csharp
public abstract Uri Address { get; }
```

#### Property Value

Uri<br/>

### **Events**

The events that are subscribed

```csharp
public abstract RoutingSlipEvents Events { get; }
```

#### Property Value

[RoutingSlipEvents](../masstransit-courier-contracts/routingslipevents)<br/>

### **Include**

The event contents to include when published

```csharp
public abstract RoutingSlipEventContents Include { get; }
```

#### Property Value

[RoutingSlipEventContents](../masstransit-courier-contracts/routingslipeventcontents)<br/>

### **ActivityName**

If specified, events are only used in this subscription if the activity name matches

```csharp
public abstract string ActivityName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Message**

The message sent as part of the subscription

```csharp
public abstract MessageEnvelope Message { get; }
```

#### Property Value

[MessageEnvelope](../masstransit-serialization/messageenvelope)<br/>
