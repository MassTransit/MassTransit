---

title: RoutingSlipActivityFaultedMessage

---

# RoutingSlipActivityFaultedMessage

Namespace: MassTransit.Courier.Messages

```csharp
public class RoutingSlipActivityFaultedMessage : RoutingSlipActivityFaulted
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RoutingSlipActivityFaultedMessage](../masstransit-courier-messages/routingslipactivityfaultedmessage)<br/>
Implements [RoutingSlipActivityFaulted](../masstransit-courier-contracts/routingslipactivityfaulted)

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

### **Host**

```csharp
public HostInfo Host { get; set; }
```

#### Property Value

[HostInfo](../masstransit/hostinfo)<br/>

### **ExecutionId**

```csharp
public Guid ExecutionId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **ExceptionInfo**

```csharp
public ExceptionInfo ExceptionInfo { get; set; }
```

#### Property Value

[ExceptionInfo](../masstransit/exceptioninfo)<br/>

### **Arguments**

```csharp
public IDictionary<string, object> Arguments { get; set; }
```

#### Property Value

[IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

### **Variables**

```csharp
public IDictionary<string, object> Variables { get; set; }
```

#### Property Value

[IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

## Constructors

### **RoutingSlipActivityFaultedMessage()**

```csharp
public RoutingSlipActivityFaultedMessage()
```

### **RoutingSlipActivityFaultedMessage(HostInfo, Guid, String, Guid, DateTime, TimeSpan, ExceptionInfo, IDictionary\<String, Object\>, IDictionary\<String, Object\>)**

```csharp
public RoutingSlipActivityFaultedMessage(HostInfo host, Guid trackingNumber, string activityName, Guid executionId, DateTime timestamp, TimeSpan duration, ExceptionInfo exceptionInfo, IDictionary<string, object> variables, IDictionary<string, object> arguments)
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

`arguments` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>
