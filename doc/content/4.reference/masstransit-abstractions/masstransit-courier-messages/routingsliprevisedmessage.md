---

title: RoutingSlipRevisedMessage

---

# RoutingSlipRevisedMessage

Namespace: MassTransit.Courier.Messages

```csharp
public class RoutingSlipRevisedMessage : RoutingSlipRevised
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RoutingSlipRevisedMessage](../masstransit-courier-messages/routingsliprevisedmessage)<br/>
Implements [RoutingSlipRevised](../masstransit-courier-contracts/routingsliprevised)

## Properties

### **TrackingNumber**

```csharp
public Guid TrackingNumber { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Timestamp**

```csharp
public DateTime Timestamp { get; set; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **Duration**

```csharp
public TimeSpan Duration { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **ActivityName**

```csharp
public string ActivityName { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ExecutionId**

```csharp
public Guid ExecutionId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Host**

```csharp
public HostInfo Host { get; set; }
```

#### Property Value

[HostInfo](../masstransit/hostinfo)<br/>

### **Variables**

```csharp
public IDictionary<string, object> Variables { get; set; }
```

#### Property Value

[IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

### **Itinerary**

```csharp
public Activity[] Itinerary { get; set; }
```

#### Property Value

[Activity[]](../masstransit-courier-contracts/activity)<br/>

### **DiscardedItinerary**

```csharp
public Activity[] DiscardedItinerary { get; set; }
```

#### Property Value

[Activity[]](../masstransit-courier-contracts/activity)<br/>

## Constructors

### **RoutingSlipRevisedMessage()**

```csharp
public RoutingSlipRevisedMessage()
```

### **RoutingSlipRevisedMessage(HostInfo, Guid, String, Guid, DateTime, TimeSpan, IDictionary\<String, Object\>, IEnumerable\<Activity\>, IEnumerable\<Activity\>)**

```csharp
public RoutingSlipRevisedMessage(HostInfo host, Guid trackingNumber, string activityName, Guid executionId, DateTime timestamp, TimeSpan duration, IDictionary<string, object> variables, IEnumerable<Activity> itinerary, IEnumerable<Activity> discardedItinerary)
```

#### Parameters

`host` [HostInfo](../masstransit/hostinfo)<br/>

`trackingNumber` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`activityName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`executionId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`timestamp` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`variables` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`itinerary` [IEnumerable\<Activity\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`discardedItinerary` [IEnumerable\<Activity\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
