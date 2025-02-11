---

title: RoutingSlipCompletedMessage

---

# RoutingSlipCompletedMessage

Namespace: MassTransit.Courier.Messages

```csharp
public class RoutingSlipCompletedMessage : RoutingSlipCompleted
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RoutingSlipCompletedMessage](../masstransit-courier-messages/routingslipcompletedmessage)<br/>
Implements [RoutingSlipCompleted](../masstransit-courier-contracts/routingslipcompleted)

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

### **Variables**

```csharp
public IDictionary<string, object> Variables { get; set; }
```

#### Property Value

[IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

## Constructors

### **RoutingSlipCompletedMessage()**

```csharp
public RoutingSlipCompletedMessage()
```

### **RoutingSlipCompletedMessage(Guid, DateTime, TimeSpan, IDictionary\<String, Object\>)**

```csharp
public RoutingSlipCompletedMessage(Guid trackingNumber, DateTime timestamp, TimeSpan duration, IDictionary<string, object> variables)
```

#### Parameters

`trackingNumber` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`timestamp` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`variables` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>
