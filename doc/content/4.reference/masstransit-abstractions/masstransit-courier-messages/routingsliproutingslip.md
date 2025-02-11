---

title: RoutingSlipRoutingSlip

---

# RoutingSlipRoutingSlip

Namespace: MassTransit.Courier.Messages

```csharp
public class RoutingSlipRoutingSlip : RoutingSlip
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RoutingSlipRoutingSlip](../masstransit-courier-messages/routingsliproutingslip)<br/>
Implements [RoutingSlip](../masstransit-courier-contracts/routingslip)

## Properties

### **TrackingNumber**

```csharp
public Guid TrackingNumber { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **CreateTimestamp**

```csharp
public DateTime CreateTimestamp { get; set; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **Itinerary**

```csharp
public IList<Activity> Itinerary { get; set; }
```

#### Property Value

[IList\<Activity\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1)<br/>

### **ActivityLogs**

```csharp
public IList<ActivityLog> ActivityLogs { get; set; }
```

#### Property Value

[IList\<ActivityLog\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1)<br/>

### **CompensateLogs**

```csharp
public IList<CompensateLog> CompensateLogs { get; set; }
```

#### Property Value

[IList\<CompensateLog\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1)<br/>

### **Variables**

```csharp
public IDictionary<string, object> Variables { get; set; }
```

#### Property Value

[IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

### **ActivityExceptions**

```csharp
public IList<ActivityException> ActivityExceptions { get; set; }
```

#### Property Value

[IList\<ActivityException\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1)<br/>

### **Subscriptions**

```csharp
public IList<Subscription> Subscriptions { get; set; }
```

#### Property Value

[IList\<Subscription\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1)<br/>

## Constructors

### **RoutingSlipRoutingSlip()**

```csharp
public RoutingSlipRoutingSlip()
```

### **RoutingSlipRoutingSlip(Guid, DateTime, IEnumerable\<Activity\>, IEnumerable\<ActivityLog\>, IEnumerable\<CompensateLog\>, IEnumerable\<ActivityException\>, IDictionary\<String, Object\>, IEnumerable\<Subscription\>)**

```csharp
public RoutingSlipRoutingSlip(Guid trackingNumber, DateTime createTimestamp, IEnumerable<Activity> activities, IEnumerable<ActivityLog> activityLogs, IEnumerable<CompensateLog> compensateLogs, IEnumerable<ActivityException> exceptions, IDictionary<string, object> variables, IEnumerable<Subscription> subscriptions)
```

#### Parameters

`trackingNumber` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`createTimestamp` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`activities` [IEnumerable\<Activity\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`activityLogs` [IEnumerable\<ActivityLog\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`compensateLogs` [IEnumerable\<CompensateLog\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`exceptions` [IEnumerable\<ActivityException\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`variables` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`subscriptions` [IEnumerable\<Subscription\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
