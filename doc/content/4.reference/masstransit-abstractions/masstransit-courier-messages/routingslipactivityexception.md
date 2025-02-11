---

title: RoutingSlipActivityException

---

# RoutingSlipActivityException

Namespace: MassTransit.Courier.Messages

```csharp
public class RoutingSlipActivityException : ActivityException
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RoutingSlipActivityException](../masstransit-courier-messages/routingslipactivityexception)<br/>
Implements [ActivityException](../masstransit-courier-contracts/activityexception)

## Properties

### **ExecutionId**

```csharp
public Guid ExecutionId { get; set; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Timestamp**

```csharp
public DateTime Timestamp { get; set; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **Elapsed**

```csharp
public TimeSpan Elapsed { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **Name**

```csharp
public string Name { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Host**

```csharp
public HostInfo Host { get; set; }
```

#### Property Value

[HostInfo](../masstransit/hostinfo)<br/>

### **ExceptionInfo**

```csharp
public ExceptionInfo ExceptionInfo { get; set; }
```

#### Property Value

[ExceptionInfo](../masstransit/exceptioninfo)<br/>

## Constructors

### **RoutingSlipActivityException()**

```csharp
public RoutingSlipActivityException()
```

### **RoutingSlipActivityException(String, HostInfo, Guid, DateTime, TimeSpan, ExceptionInfo)**

```csharp
public RoutingSlipActivityException(string activityName, HostInfo host, Guid executionId, DateTime timestamp, TimeSpan elapsed, ExceptionInfo exceptionInfo)
```

#### Parameters

`activityName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`host` [HostInfo](../masstransit/hostinfo)<br/>

`executionId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`timestamp` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`elapsed` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`exceptionInfo` [ExceptionInfo](../masstransit/exceptioninfo)<br/>

### **RoutingSlipActivityException(ActivityException)**

```csharp
public RoutingSlipActivityException(ActivityException activityException)
```

#### Parameters

`activityException` [ActivityException](../masstransit-courier-contracts/activityexception)<br/>
