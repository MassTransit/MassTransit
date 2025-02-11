---

title: RoutingSlipEventPublisher

---

# RoutingSlipEventPublisher

Namespace: MassTransit.Courier

```csharp
public class RoutingSlipEventPublisher : IRoutingSlipEventPublisher
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RoutingSlipEventPublisher](../masstransit-courier/routingslipeventpublisher)<br/>
Implements [IRoutingSlipEventPublisher](../masstransit-courier/iroutingslipeventpublisher)

## Constructors

### **RoutingSlipEventPublisher(CourierContext, RoutingSlip, CancellationToken)**

```csharp
public RoutingSlipEventPublisher(CourierContext context, RoutingSlip routingSlip, CancellationToken cancellationToken)
```

#### Parameters

`context` [CourierContext](../masstransit/couriercontext)<br/>

`routingSlip` [RoutingSlip](../masstransit-courier-contracts/routingslip)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **RoutingSlipEventPublisher(ISendEndpointProvider, IPublishEndpoint, RoutingSlip, CancellationToken)**

```csharp
public RoutingSlipEventPublisher(ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint, RoutingSlip routingSlip, CancellationToken cancellationToken)
```

#### Parameters

`sendEndpointProvider` [ISendEndpointProvider](../masstransit/isendendpointprovider)<br/>

`publishEndpoint` [IPublishEndpoint](../masstransit/ipublishendpoint)<br/>

`routingSlip` [RoutingSlip](../masstransit-courier-contracts/routingslip)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Methods

### **PublishRoutingSlipCompleted(DateTime, TimeSpan, IDictionary\<String, Object\>)**

```csharp
public Task PublishRoutingSlipCompleted(DateTime timestamp, TimeSpan duration, IDictionary<string, object> variables)
```

#### Parameters

`timestamp` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`variables` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PublishRoutingSlipFaulted(DateTime, TimeSpan, IDictionary\<String, Object\>, ActivityException[])**

```csharp
public Task PublishRoutingSlipFaulted(DateTime timestamp, TimeSpan duration, IDictionary<string, object> variables, ActivityException[] exceptions)
```

#### Parameters

`timestamp` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`variables` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`exceptions` [ActivityException[]](../masstransit-courier-contracts/activityexception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PublishRoutingSlipActivityCompleted(String, Guid, DateTime, TimeSpan, IDictionary\<String, Object\>, IDictionary\<String, Object\>, IDictionary\<String, Object\>)**

```csharp
public Task PublishRoutingSlipActivityCompleted(string activityName, Guid executionId, DateTime timestamp, TimeSpan duration, IDictionary<string, object> variables, IDictionary<string, object> arguments, IDictionary<string, object> data)
```

#### Parameters

`activityName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`executionId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`timestamp` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`variables` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`arguments` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`data` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PublishRoutingSlipActivityFaulted(String, Guid, DateTime, TimeSpan, ExceptionInfo, IDictionary\<String, Object\>, IDictionary\<String, Object\>)**

```csharp
public Task PublishRoutingSlipActivityFaulted(string activityName, Guid executionId, DateTime timestamp, TimeSpan duration, ExceptionInfo exceptionInfo, IDictionary<string, object> variables, IDictionary<string, object> arguments)
```

#### Parameters

`activityName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`executionId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`timestamp` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`exceptionInfo` [ExceptionInfo](../masstransit/exceptioninfo)<br/>

`variables` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`arguments` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PublishRoutingSlipActivityCompensated(String, Guid, DateTime, TimeSpan, IDictionary\<String, Object\>, IDictionary\<String, Object\>)**

```csharp
public Task PublishRoutingSlipActivityCompensated(string activityName, Guid executionId, DateTime timestamp, TimeSpan duration, IDictionary<string, object> variables, IDictionary<string, object> data)
```

#### Parameters

`activityName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`executionId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`timestamp` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`variables` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`data` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PublishRoutingSlipRevised(String, Guid, DateTime, TimeSpan, IDictionary\<String, Object\>, IList\<Activity\>, IList\<Activity\>)**

```csharp
public Task PublishRoutingSlipRevised(string activityName, Guid executionId, DateTime timestamp, TimeSpan duration, IDictionary<string, object> variables, IList<Activity> itinerary, IList<Activity> previousItinerary)
```

#### Parameters

`activityName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`executionId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`timestamp` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`variables` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`itinerary` [IList\<Activity\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1)<br/>

`previousItinerary` [IList\<Activity\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PublishRoutingSlipTerminated(String, Guid, DateTime, TimeSpan, IDictionary\<String, Object\>, IList\<Activity\>)**

```csharp
public Task PublishRoutingSlipTerminated(string activityName, Guid executionId, DateTime timestamp, TimeSpan duration, IDictionary<string, object> variables, IList<Activity> previousItinerary)
```

#### Parameters

`activityName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`executionId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`timestamp` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`variables` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`previousItinerary` [IList\<Activity\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PublishRoutingSlipActivityCompensationFailed(String, Guid, DateTime, TimeSpan, DateTime, TimeSpan, ExceptionInfo, IDictionary\<String, Object\>, IDictionary\<String, Object\>)**

```csharp
public Task PublishRoutingSlipActivityCompensationFailed(string activityName, Guid executionId, DateTime timestamp, TimeSpan duration, DateTime failureTimestamp, TimeSpan routingSlipDuration, ExceptionInfo exceptionInfo, IDictionary<string, object> variables, IDictionary<string, object> data)
```

#### Parameters

`activityName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`executionId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`timestamp` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`failureTimestamp` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`routingSlipDuration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`exceptionInfo` [ExceptionInfo](../masstransit/exceptioninfo)<br/>

`variables` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

`data` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
