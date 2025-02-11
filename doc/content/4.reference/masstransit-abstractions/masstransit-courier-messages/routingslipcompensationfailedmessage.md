---

title: RoutingSlipCompensationFailedMessage

---

# RoutingSlipCompensationFailedMessage

Namespace: MassTransit.Courier.Messages

```csharp
public class RoutingSlipCompensationFailedMessage : RoutingSlipCompensationFailed
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RoutingSlipCompensationFailedMessage](../masstransit-courier-messages/routingslipcompensationfailedmessage)<br/>
Implements [RoutingSlipCompensationFailed](../masstransit-courier-contracts/routingslipcompensationfailed)

## Properties

### **TrackingNumber**

```csharp
public Guid TrackingNumber { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **ExceptionInfo**

```csharp
public ExceptionInfo ExceptionInfo { get; set; }
```

#### Property Value

[ExceptionInfo](../masstransit/exceptioninfo)<br/>

### **Variables**

```csharp
public IDictionary<string, object> Variables { get; set; }
```

#### Property Value

[IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

### **Host**

```csharp
public HostInfo Host { get; set; }
```

#### Property Value

[HostInfo](../masstransit/hostinfo)<br/>

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

## Constructors

### **RoutingSlipCompensationFailedMessage()**

```csharp
public RoutingSlipCompensationFailedMessage()
```

### **RoutingSlipCompensationFailedMessage(HostInfo, Guid, DateTime, TimeSpan, ExceptionInfo, IDictionary\<String, Object\>)**

```csharp
public RoutingSlipCompensationFailedMessage(HostInfo host, Guid trackingNumber, DateTime failureTimestamp, TimeSpan routingSlipDuration, ExceptionInfo exceptionInfo, IDictionary<string, object> variables)
```

#### Parameters

`host` [HostInfo](../masstransit/hostinfo)<br/>

`trackingNumber` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`failureTimestamp` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`routingSlipDuration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`exceptionInfo` [ExceptionInfo](../masstransit/exceptioninfo)<br/>

`variables` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>
