---

title: RoutingSlipFaultedMessage

---

# RoutingSlipFaultedMessage

Namespace: MassTransit.Courier.Messages

```csharp
public class RoutingSlipFaultedMessage : RoutingSlipFaulted
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RoutingSlipFaultedMessage](../masstransit-courier-messages/routingslipfaultedmessage)<br/>
Implements [RoutingSlipFaulted](../masstransit-courier-contracts/routingslipfaulted)

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

### **ActivityExceptions**

```csharp
public ActivityException[] ActivityExceptions { get; set; }
```

#### Property Value

[ActivityException[]](../masstransit-courier-contracts/activityexception)<br/>

### **Variables**

```csharp
public IDictionary<string, object> Variables { get; set; }
```

#### Property Value

[IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

## Constructors

### **RoutingSlipFaultedMessage()**

```csharp
public RoutingSlipFaultedMessage()
```

### **RoutingSlipFaultedMessage(Guid, DateTime, TimeSpan, IEnumerable\<ActivityException\>, IDictionary\<String, Object\>)**

```csharp
public RoutingSlipFaultedMessage(Guid trackingNumber, DateTime timestamp, TimeSpan duration, IEnumerable<ActivityException> activityExceptions, IDictionary<string, object> variables)
```

#### Parameters

`trackingNumber` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`timestamp` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`activityExceptions` [IEnumerable\<ActivityException\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`variables` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

### **RoutingSlipFaultedMessage(Guid, DateTime, TimeSpan, ActivityException)**

```csharp
public RoutingSlipFaultedMessage(Guid trackingNumber, DateTime timestamp, TimeSpan duration, ActivityException activityException)
```

#### Parameters

`trackingNumber` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`timestamp` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`activityException` [ActivityException](../masstransit-courier-contracts/activityexception)<br/>
