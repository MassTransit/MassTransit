---

title: RoutingSlipBuilder

---

# RoutingSlipBuilder

Namespace: MassTransit

A RoutingSlipBuilder is used to create a routing slip with proper validation that the resulting RoutingSlip
 is valid.

```csharp
public class RoutingSlipBuilder : IRoutingSlipBuilder, IItineraryBuilder, IRoutingSlipSendEndpointTarget
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RoutingSlipBuilder](../masstransit/routingslipbuilder)<br/>
Implements [IRoutingSlipBuilder](../../masstransit-abstractions/masstransit/iroutingslipbuilder), [IItineraryBuilder](../../masstransit-abstractions/masstransit/iitinerarybuilder), [IRoutingSlipSendEndpointTarget](../masstransit-courier/iroutingslipsendendpointtarget)

## Fields

### **NoArguments**

```csharp
public static IDictionary<string, object> NoArguments;
```

## Properties

### **SourceItinerary**

```csharp
public IList<Activity> SourceItinerary { get; }
```

#### Property Value

[IList\<Activity\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1)<br/>

### **TrackingNumber**

The tracking number of the routing slip

```csharp
public Guid TrackingNumber { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

## Constructors

### **RoutingSlipBuilder(Guid)**

```csharp
public RoutingSlipBuilder(Guid trackingNumber)
```

#### Parameters

`trackingNumber` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **RoutingSlipBuilder(RoutingSlip, Func\<IEnumerable\<Activity\>, IEnumerable\<Activity\>\>)**

```csharp
public RoutingSlipBuilder(RoutingSlip routingSlip, Func<IEnumerable<Activity>, IEnumerable<Activity>> activitySelector)
```

#### Parameters

`routingSlip` [RoutingSlip](../../masstransit-abstractions/masstransit-courier-contracts/routingslip)<br/>

`activitySelector` [Func\<IEnumerable\<Activity\>, IEnumerable\<Activity\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **RoutingSlipBuilder(RoutingSlip, IEnumerable\<Activity\>, IEnumerable\<Activity\>)**

```csharp
public RoutingSlipBuilder(RoutingSlip routingSlip, IEnumerable<Activity> itinerary, IEnumerable<Activity> sourceItinerary)
```

#### Parameters

`routingSlip` [RoutingSlip](../../masstransit-abstractions/masstransit-courier-contracts/routingslip)<br/>

`itinerary` [IEnumerable\<Activity\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`sourceItinerary` [IEnumerable\<Activity\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **RoutingSlipBuilder(RoutingSlip, IEnumerable\<CompensateLog\>)**

```csharp
public RoutingSlipBuilder(RoutingSlip routingSlip, IEnumerable<CompensateLog> compensateLogs)
```

#### Parameters

`routingSlip` [RoutingSlip](../../masstransit-abstractions/masstransit-courier-contracts/routingslip)<br/>

`compensateLogs` [IEnumerable\<CompensateLog\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

## Methods

### **AddActivity(String, Uri)**

Adds an activity to the routing slip without specifying any arguments

```csharp
public void AddActivity(string name, Uri executeAddress)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The activity name

`executeAddress` Uri<br/>
The execution address of the activity

### **AddActivity(String, Uri, Object)**

Adds an activity to the routing slip specifying activity arguments as an anonymous object

```csharp
public void AddActivity(string name, Uri executeAddress, object arguments)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The activity name

`executeAddress` Uri<br/>
The execution address of the activity

`arguments` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
An anonymous object of properties matching the argument names of the activity

### **AddActivity(String, Uri, IDictionary\<String, Object\>)**

Adds an activity to the routing slip specifying activity arguments a dictionary

```csharp
public void AddActivity(string name, Uri executeAddress, IDictionary<string, object> arguments)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The activity name

`executeAddress` Uri<br/>
The execution address of the activity

`arguments` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>
A dictionary of name/values matching the activity argument properties

### **AddVariable(String, String)**

Add a string value to the routing slip

```csharp
public void AddVariable(string key, string value)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **AddVariable(String, Object)**

Add an object variable to the routing slip

```csharp
public void AddVariable(string key, object value)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

### **SetVariables(Object)**

Sets the value of any existing variables to the value in the anonymous object,
 as well as adding any additional variables that did not exist previously.
 For example, SetVariables(new { IntValue = 27, StringValue = "Hello, World." });

```csharp
public void SetVariables(object values)
```

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

### **SetVariables(IEnumerable\<KeyValuePair\<String, Object\>\>)**

```csharp
public void SetVariables(IEnumerable<KeyValuePair<string, object>> values)
```

#### Parameters

`values` [IEnumerable\<KeyValuePair\<String, Object\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **AddActivitiesFromSourceItinerary()**

Adds the activities from the source itinerary to the new routing slip and removes them from the
 source itinerary.

```csharp
public int AddActivitiesFromSourceItinerary()
```

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **AddSubscription(Uri, RoutingSlipEvents)**

Add an explicit subscription to the routing slip events

```csharp
public void AddSubscription(Uri address, RoutingSlipEvents events)
```

#### Parameters

`address` Uri<br/>
The destination address where the events are sent

`events` [RoutingSlipEvents](../../masstransit-abstractions/masstransit-courier-contracts/routingslipevents)<br/>
The events to include in the subscription

### **AddSubscription(Uri, RoutingSlipEvents, RoutingSlipEventContents)**

Add an explicit subscription to the routing slip events

```csharp
public void AddSubscription(Uri address, RoutingSlipEvents events, RoutingSlipEventContents contents)
```

#### Parameters

`address` Uri<br/>
The destination address where the events are sent

`events` [RoutingSlipEvents](../../masstransit-abstractions/masstransit-courier-contracts/routingslipevents)<br/>
The events to include in the subscription

`contents` [RoutingSlipEventContents](../../masstransit-abstractions/masstransit-courier-contracts/routingslipeventcontents)<br/>
The contents of the routing slip event

### **AddSubscription(Uri, RoutingSlipEvents, RoutingSlipEventContents, String)**

Add an explicit subscription to the routing slip events

```csharp
public void AddSubscription(Uri address, RoutingSlipEvents events, RoutingSlipEventContents contents, string activityName)
```

#### Parameters

`address` Uri<br/>
The destination address where the events are sent

`events` [RoutingSlipEvents](../../masstransit-abstractions/masstransit-courier-contracts/routingslipevents)<br/>
The events to include in the subscription

`contents` [RoutingSlipEventContents](../../masstransit-abstractions/masstransit-courier-contracts/routingslipeventcontents)<br/>
The contents of the routing slip event

`activityName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
Only send events for the specified activity

### **AddSubscription(Uri, RoutingSlipEvents, Func\<ISendEndpoint, Task\>)**

Adds a message subscription to the routing slip that will be sent at the specified event points

```csharp
public Task AddSubscription(Uri address, RoutingSlipEvents events, Func<ISendEndpoint, Task> callback)
```

#### Parameters

`address` Uri<br/>

`events` [RoutingSlipEvents](../../masstransit-abstractions/masstransit-courier-contracts/routingslipevents)<br/>

`callback` [Func\<ISendEndpoint, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **AddSubscription(Uri, RoutingSlipEvents, RoutingSlipEventContents, Func\<ISendEndpoint, Task\>)**

Adds a message subscription to the routing slip that will be sent at the specified event points

```csharp
public Task AddSubscription(Uri address, RoutingSlipEvents events, RoutingSlipEventContents contents, Func<ISendEndpoint, Task> callback)
```

#### Parameters

`address` Uri<br/>

`events` [RoutingSlipEvents](../../masstransit-abstractions/masstransit-courier-contracts/routingslipevents)<br/>

`contents` [RoutingSlipEventContents](../../masstransit-abstractions/masstransit-courier-contracts/routingslipeventcontents)<br/>

`callback` [Func\<ISendEndpoint, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **AddSubscription(Uri, RoutingSlipEvents, RoutingSlipEventContents, String, Func\<ISendEndpoint, Task\>)**

Adds a message subscription to the routing slip that will be sent at the specified event points

```csharp
public Task AddSubscription(Uri address, RoutingSlipEvents events, RoutingSlipEventContents contents, string activityName, Func<ISendEndpoint, Task> callback)
```

#### Parameters

`address` Uri<br/>

`events` [RoutingSlipEvents](../../masstransit-abstractions/masstransit-courier-contracts/routingslipevents)<br/>

`contents` [RoutingSlipEventContents](../../masstransit-abstractions/masstransit-courier-contracts/routingslipeventcontents)<br/>

`activityName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
Only send events for the specified activity

`callback` [Func\<ISendEndpoint, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Build()**

Builds the routing slip using the current state of the builder

```csharp
public RoutingSlip Build()
```

#### Returns

[RoutingSlip](../../masstransit-abstractions/masstransit-courier-contracts/routingslip)<br/>
The RoutingSlip

### **AddActivityLog(HostInfo, String, Guid, DateTime, TimeSpan)**

```csharp
public void AddActivityLog(HostInfo host, string name, Guid activityTrackingNumber, DateTime timestamp, TimeSpan duration)
```

#### Parameters

`host` [HostInfo](../../masstransit-abstractions/masstransit/hostinfo)<br/>

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`activityTrackingNumber` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`timestamp` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **AddCompensateLog(Guid, Uri, Object)**

```csharp
public void AddCompensateLog(Guid activityTrackingNumber, Uri compensateAddress, object logObject)
```

#### Parameters

`activityTrackingNumber` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`compensateAddress` Uri<br/>

`logObject` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

### **AddCompensateLog(Guid, Uri, IDictionary\<String, Object\>)**

```csharp
public void AddCompensateLog(Guid activityTrackingNumber, Uri compensateAddress, IDictionary<string, object> data)
```

#### Parameters

`activityTrackingNumber` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`compensateAddress` Uri<br/>

`data` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

### **AddActivityException(HostInfo, String, Guid, DateTime, TimeSpan, Exception)**

Adds an activity exception to the routing slip

```csharp
public void AddActivityException(HostInfo host, string name, Guid activityTrackingNumber, DateTime timestamp, TimeSpan elapsed, Exception exception)
```

#### Parameters

`host` [HostInfo](../../masstransit-abstractions/masstransit/hostinfo)<br/>

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The name of the faulted activity

`activityTrackingNumber` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>
The activity tracking number

`timestamp` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>
The timestamp of the exception

`elapsed` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time elapsed from the start of the activity to the exception

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>
The exception thrown by the activity

### **AddActivityException(HostInfo, String, Guid, DateTime, TimeSpan, ExceptionInfo)**

Adds an activity exception to the routing slip

```csharp
public void AddActivityException(HostInfo host, string name, Guid activityTrackingNumber, DateTime timestamp, TimeSpan elapsed, ExceptionInfo exceptionInfo)
```

#### Parameters

`host` [HostInfo](../../masstransit-abstractions/masstransit/hostinfo)<br/>

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The name of the faulted activity

`activityTrackingNumber` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>
The activity tracking number

`timestamp` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>
The timestamp of the exception

`elapsed` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The time elapsed from the start of the activity to the exception

`exceptionInfo` [ExceptionInfo](../../masstransit-abstractions/masstransit/exceptioninfo)<br/>

### **AddActivityException(ActivityException)**

```csharp
public void AddActivityException(ActivityException activityException)
```

#### Parameters

`activityException` [ActivityException](../../masstransit-abstractions/masstransit-courier-contracts/activityexception)<br/>

### **GetObjectAsDictionary(Object)**

```csharp
public static IDictionary<string, object> GetObjectAsDictionary(object values)
```

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>
