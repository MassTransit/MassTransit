---

title: IItineraryBuilder

---

# IItineraryBuilder

Namespace: MassTransit

```csharp
public interface IItineraryBuilder
```

## Properties

### **TrackingNumber**

The tracking number of the routing slip

```csharp
public abstract Guid TrackingNumber { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

## Methods

### **AddActivity(String, Uri)**

Adds an activity to the routing slip without specifying any arguments

```csharp
void AddActivity(string name, Uri executeAddress)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The activity name

`executeAddress` Uri<br/>
The execution address of the activity

### **AddActivity(String, Uri, Object)**

Adds an activity to the routing slip specifying activity arguments as an anonymous object

```csharp
void AddActivity(string name, Uri executeAddress, object arguments)
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
void AddActivity(string name, Uri executeAddress, IDictionary<string, object> arguments)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The activity name

`executeAddress` Uri<br/>
The execution address of the activity

`arguments` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>
A dictionary of name/values matching the activity argument properties

### **AddVariable(String, String)**

Add a variable to the routing slip

```csharp
void AddVariable(string key, string value)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **AddVariable(String, Object)**

Add a variable to the routing slip

```csharp
void AddVariable(string key, object value)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

### **SetVariables(Object)**

Sets the value of any existing variables to the value in the anonymous object,
 as well as adding any additional variables that did not exist previously.
 For example, SetVariables(new { IntValue = 27, StringValue = "Hello, World." });

```csharp
void SetVariables(object values)
```

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

### **SetVariables(IEnumerable\<KeyValuePair\<String, Object\>\>)**

Set multiple variables (from a dictionary, for example) on the routing slip

```csharp
void SetVariables(IEnumerable<KeyValuePair<string, object>> values)
```

#### Parameters

`values` [IEnumerable\<KeyValuePair\<String, Object\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **AddActivitiesFromSourceItinerary()**

Add the original itinerary to the routing slip (if present)

```csharp
int AddActivitiesFromSourceItinerary()
```

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of activities added to the itinerary

### **AddSubscription(Uri, RoutingSlipEvents)**

Add an explicit subscription to the routing slip events

```csharp
void AddSubscription(Uri address, RoutingSlipEvents events)
```

#### Parameters

`address` Uri<br/>
The destination address where the events are sent

`events` [RoutingSlipEvents](../masstransit-courier-contracts/routingslipevents)<br/>
The events to include in the subscription

### **AddSubscription(Uri, RoutingSlipEvents, RoutingSlipEventContents)**

Add an explicit subscription to the routing slip events

```csharp
void AddSubscription(Uri address, RoutingSlipEvents events, RoutingSlipEventContents contents)
```

#### Parameters

`address` Uri<br/>
The destination address where the events are sent

`events` [RoutingSlipEvents](../masstransit-courier-contracts/routingslipevents)<br/>
The events to include in the subscription

`contents` [RoutingSlipEventContents](../masstransit-courier-contracts/routingslipeventcontents)<br/>
The contents of the routing slip event

### **AddSubscription(Uri, RoutingSlipEvents, Func\<ISendEndpoint, Task\>)**

Adds a message subscription to the routing slip that will be sent at the specified event points

```csharp
Task AddSubscription(Uri address, RoutingSlipEvents events, Func<ISendEndpoint, Task> callback)
```

#### Parameters

`address` Uri<br/>

`events` [RoutingSlipEvents](../masstransit-courier-contracts/routingslipevents)<br/>

`callback` [Func\<ISendEndpoint, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **AddSubscription(Uri, RoutingSlipEvents, RoutingSlipEventContents, Func\<ISendEndpoint, Task\>)**

Adds a message subscription to the routing slip that will be sent at the specified event points

```csharp
Task AddSubscription(Uri address, RoutingSlipEvents events, RoutingSlipEventContents contents, Func<ISendEndpoint, Task> callback)
```

#### Parameters

`address` Uri<br/>

`events` [RoutingSlipEvents](../masstransit-courier-contracts/routingslipevents)<br/>

`contents` [RoutingSlipEventContents](../masstransit-courier-contracts/routingslipeventcontents)<br/>

`callback` [Func\<ISendEndpoint, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **AddSubscription(Uri, RoutingSlipEvents, RoutingSlipEventContents, String)**

Add an explicit subscription to the routing slip events

```csharp
void AddSubscription(Uri address, RoutingSlipEvents events, RoutingSlipEventContents contents, string activityName)
```

#### Parameters

`address` Uri<br/>
The destination address where the events are sent

`events` [RoutingSlipEvents](../masstransit-courier-contracts/routingslipevents)<br/>
The events to include in the subscription

`contents` [RoutingSlipEventContents](../masstransit-courier-contracts/routingslipeventcontents)<br/>
The contents of the routing slip event

`activityName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
Only send events for the specified activity

### **AddSubscription(Uri, RoutingSlipEvents, RoutingSlipEventContents, String, Func\<ISendEndpoint, Task\>)**

Adds a message subscription to the routing slip that will be sent at the specified event points

```csharp
Task AddSubscription(Uri address, RoutingSlipEvents events, RoutingSlipEventContents contents, string activityName, Func<ISendEndpoint, Task> callback)
```

#### Parameters

`address` Uri<br/>

`events` [RoutingSlipEvents](../masstransit-courier-contracts/routingslipevents)<br/>

`contents` [RoutingSlipEventContents](../masstransit-courier-contracts/routingslipeventcontents)<br/>

`activityName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
Only send events for the specified activity

`callback` [Func\<ISendEndpoint, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
