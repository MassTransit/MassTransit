---

title: RoutingSlipActivityCompensatedMessage

---

# RoutingSlipActivityCompensatedMessage

Namespace: MassTransit.Courier.Messages

```csharp
public class RoutingSlipActivityCompensatedMessage : RoutingSlipActivityCompensated
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RoutingSlipActivityCompensatedMessage](../masstransit-courier-messages/routingslipactivitycompensatedmessage)<br/>
Implements [RoutingSlipActivityCompensated](../masstransit-courier-contracts/routingslipactivitycompensated)

## Properties

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

### **Data**

```csharp
public IDictionary<string, object> Data { get; set; }
```

#### Property Value

[IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

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

### **ActivityName**

```csharp
public string ActivityName { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

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

## Constructors

### **RoutingSlipActivityCompensatedMessage()**

```csharp
public RoutingSlipActivityCompensatedMessage()
```

### **RoutingSlipActivityCompensatedMessage(HostInfo, Guid, String, Guid, DateTime, TimeSpan, IDictionary\<String, Object\>, IDictionary\<String, Object\>)**

```csharp
public RoutingSlipActivityCompensatedMessage(HostInfo host, Guid trackingNumber, string activityName, Guid executionId, DateTime timestamp, TimeSpan duration, IDictionary<string, object> variables, IDictionary<string, object> data)
```

#### Parameters

`host` [HostInfo](../masstransit/hostinfo)<br/>

`trackingNumber` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`activityName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`executionId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`timestamp` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`variables` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`data` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>
