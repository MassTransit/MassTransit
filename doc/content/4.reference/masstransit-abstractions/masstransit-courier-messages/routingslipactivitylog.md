---

title: RoutingSlipActivityLog

---

# RoutingSlipActivityLog

Namespace: MassTransit.Courier.Messages

```csharp
public class RoutingSlipActivityLog : ActivityLog
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RoutingSlipActivityLog](../masstransit-courier-messages/routingslipactivitylog)<br/>
Implements [ActivityLog](../masstransit-courier-contracts/activitylog)

## Properties

### **ExecutionId**

```csharp
public Guid ExecutionId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Name**

```csharp
public string Name { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

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

### **Host**

```csharp
public HostInfo Host { get; set; }
```

#### Property Value

[HostInfo](../masstransit/hostinfo)<br/>

## Constructors

### **RoutingSlipActivityLog()**

```csharp
public RoutingSlipActivityLog()
```

### **RoutingSlipActivityLog(HostInfo, Guid, String, DateTime, TimeSpan)**

```csharp
public RoutingSlipActivityLog(HostInfo host, Guid executionId, string name, DateTime timestamp, TimeSpan duration)
```

#### Parameters

`host` [HostInfo](../masstransit/hostinfo)<br/>

`executionId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`timestamp` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **RoutingSlipActivityLog(ActivityLog)**

```csharp
public RoutingSlipActivityLog(ActivityLog activityLog)
```

#### Parameters

`activityLog` [ActivityLog](../masstransit-courier-contracts/activitylog)<br/>
