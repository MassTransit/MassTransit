---

title: RoutingSlipActivityCompensationFailedMessage

---

# RoutingSlipActivityCompensationFailedMessage

Namespace: MassTransit.Courier.Messages

```csharp
public class RoutingSlipActivityCompensationFailedMessage : RoutingSlipActivityCompensationFailed
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RoutingSlipActivityCompensationFailedMessage](../masstransit-courier-messages/routingslipactivitycompensationfailedmessage)<br/>
Implements [RoutingSlipActivityCompensationFailed](../masstransit-courier-contracts/routingslipactivitycompensationfailed)

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

### **ExecutionId**

```csharp
public Guid ExecutionId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **ActivityName**

```csharp
public string ActivityName { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Data**

```csharp
public IDictionary<string, object> Data { get; set; }
```

#### Property Value

[IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

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

### **Duration**

```csharp
public TimeSpan Duration { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **Host**

```csharp
public HostInfo Host { get; set; }
```

#### Property Value

[HostInfo](../masstransit/hostinfo)<br/>

## Constructors

### **RoutingSlipActivityCompensationFailedMessage()**

```csharp
public RoutingSlipActivityCompensationFailedMessage()
```

### **RoutingSlipActivityCompensationFailedMessage(HostInfo, Guid, String, Guid, DateTime, TimeSpan, ExceptionInfo, IDictionary\<String, Object\>, IDictionary\<String, Object\>)**

```csharp
public RoutingSlipActivityCompensationFailedMessage(HostInfo host, Guid trackingNumber, string activityName, Guid executionId, DateTime timestamp, TimeSpan duration, ExceptionInfo exceptionInfo, IDictionary<string, object> variables, IDictionary<string, object> data)
```

#### Parameters

`host` [HostInfo](../masstransit/hostinfo)<br/>

`trackingNumber` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`activityName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`executionId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`timestamp` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`exceptionInfo` [ExceptionInfo](../masstransit/exceptioninfo)<br/>

`variables` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`data` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>
