---

title: RoutingSlip

---

# RoutingSlip

Namespace: MassTransit.Courier.Contracts

A RoutingSlip is the transport-level interface that is used to carry the details
 of a message routing slip over the network.

```csharp
public interface RoutingSlip
```

## Properties

### **TrackingNumber**

The unique tracking number for this routing slip, used to correlate events
 and activities

```csharp
public abstract Guid TrackingNumber { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **CreateTimestamp**

The time when the routing slip was created

```csharp
public abstract DateTime CreateTimestamp { get; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **Itinerary**

The list of activities that are remaining

```csharp
public abstract IList<Activity> Itinerary { get; }
```

#### Property Value

[IList\<Activity\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1)<br/>

### **ActivityLogs**

The logs of activities that have already been executed

```csharp
public abstract IList<ActivityLog> ActivityLogs { get; }
```

#### Property Value

[IList\<ActivityLog\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1)<br/>

### **CompensateLogs**

The logs of activities that can be compensated

```csharp
public abstract IList<CompensateLog> CompensateLogs { get; }
```

#### Property Value

[IList\<CompensateLog\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1)<br/>

### **Variables**

Variables that are carried with the routing slip for use by any activity

```csharp
public abstract IDictionary<string, object> Variables { get; }
```

#### Property Value

[IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

### **ActivityExceptions**

A list of exceptions that have occurred during routing slip execution

```csharp
public abstract IList<ActivityException> ActivityExceptions { get; }
```

#### Property Value

[IList\<ActivityException\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1)<br/>

### **Subscriptions**

Subscriptions to routing slip events

```csharp
public abstract IList<Subscription> Subscriptions { get; }
```

#### Property Value

[IList\<Subscription\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1)<br/>
