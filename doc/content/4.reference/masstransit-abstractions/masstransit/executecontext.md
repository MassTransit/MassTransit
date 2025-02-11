---

title: ExecuteContext

---

# ExecuteContext

Namespace: MassTransit

```csharp
public interface ExecuteContext : CourierContext, ConsumeContext<RoutingSlip>, ConsumeContext, PipeContext, MessageContext, IPublishEndpoint, IPublishObserverConnector, ISendEndpointProvider, ISendObserverConnector
```

Implements [CourierContext](../masstransit/couriercontext), [ConsumeContext\<RoutingSlip\>](../masstransit/consumecontext-1), [ConsumeContext](../masstransit/consumecontext), [PipeContext](../masstransit/pipecontext), [MessageContext](../masstransit/messagecontext), [IPublishEndpoint](../masstransit/ipublishendpoint), [IPublishObserverConnector](../masstransit/ipublishobserverconnector), [ISendEndpointProvider](../masstransit/isendendpointprovider), [ISendObserverConnector](../masstransit/isendobserverconnector)

## Properties

### **Result**

Set the execution result, which completes the activity

```csharp
public abstract ExecutionResult Result { get; set; }
```

#### Property Value

[ExecutionResult](../masstransit/executionresult)<br/>

## Methods

### **Completed()**

Completes the execution, without passing a compensating log entry

```csharp
ExecutionResult Completed()
```

#### Returns

[ExecutionResult](../masstransit/executionresult)<br/>

### **CompletedWithVariables(IEnumerable\<KeyValuePair\<String, Object\>\>)**

Completes the execution, passing updated variables to the routing slip

```csharp
ExecutionResult CompletedWithVariables(IEnumerable<KeyValuePair<string, object>> variables)
```

#### Parameters

`variables` [IEnumerable\<KeyValuePair\<String, Object\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

#### Returns

[ExecutionResult](../masstransit/executionresult)<br/>

### **CompletedWithVariables(Object)**

Completes the execution, passing updated variables to the routing slip

```csharp
ExecutionResult CompletedWithVariables(object variables)
```

#### Parameters

`variables` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[ExecutionResult](../masstransit/executionresult)<br/>

### **Completed\<TLog\>(TLog)**

Completes the activity, passing a compensation log entry

```csharp
ExecutionResult Completed<TLog>(TLog log)
```

#### Type Parameters

`TLog`<br/>

#### Parameters

`log` TLog<br/>

#### Returns

[ExecutionResult](../masstransit/executionresult)<br/>

### **Completed\<TLog\>(Object)**

Completes the activity, passing a compensation log entry

```csharp
ExecutionResult Completed<TLog>(object logValues)
```

#### Type Parameters

`TLog`<br/>

#### Parameters

`logValues` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
An object to initialize the log properties

#### Returns

[ExecutionResult](../masstransit/executionresult)<br/>

### **CompletedWithVariables\<TLog\>(TLog, Object)**

Completes the activity, passing a compensation log entry and additional variables to set on
 the routing slip

```csharp
ExecutionResult CompletedWithVariables<TLog>(TLog log, object variables)
```

#### Type Parameters

`TLog`<br/>

#### Parameters

`log` TLog<br/>

`variables` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
An anonymous object of values to add/set as variables on the routing slip

#### Returns

[ExecutionResult](../masstransit/executionresult)<br/>

### **CompletedWithVariables\<TLog\>(Object, Object)**

Completes the activity, passing a compensation log entry and additional variables to set on
 the routing slip

```csharp
ExecutionResult CompletedWithVariables<TLog>(object logValues, object variables)
```

#### Type Parameters

`TLog`<br/>

#### Parameters

`logValues` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`variables` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
An anonymous object of values to add/set as variables on the routing slip

#### Returns

[ExecutionResult](../masstransit/executionresult)<br/>

### **CompletedWithVariables\<TLog\>(TLog, IEnumerable\<KeyValuePair\<String, Object\>\>)**

Completes the activity, passing a compensation log entry and additional variables to set on
 the routing slip

```csharp
ExecutionResult CompletedWithVariables<TLog>(TLog log, IEnumerable<KeyValuePair<string, object>> variables)
```

#### Type Parameters

`TLog`<br/>

#### Parameters

`log` TLog<br/>

`variables` [IEnumerable\<KeyValuePair\<String, Object\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
An dictionary of values to add/set as variables on the routing slip

#### Returns

[ExecutionResult](../masstransit/executionresult)<br/>

### **ReviseItinerary(Action\<IItineraryBuilder\>)**

```csharp
ExecutionResult ReviseItinerary(Action<IItineraryBuilder> buildItinerary)
```

#### Parameters

`buildItinerary` [Action\<IItineraryBuilder\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExecutionResult](../masstransit/executionresult)<br/>

### **ReviseItinerary\<TLog\>(TLog, Action\<IItineraryBuilder\>)**

```csharp
ExecutionResult ReviseItinerary<TLog>(TLog log, Action<IItineraryBuilder> buildItinerary)
```

#### Type Parameters

`TLog`<br/>

#### Parameters

`log` TLog<br/>

`buildItinerary` [Action\<IItineraryBuilder\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExecutionResult](../masstransit/executionresult)<br/>

### **ReviseItinerary\<TLog\>(TLog, Object, Action\<IItineraryBuilder\>)**

```csharp
ExecutionResult ReviseItinerary<TLog>(TLog log, object variables, Action<IItineraryBuilder> buildItinerary)
```

#### Type Parameters

`TLog`<br/>

#### Parameters

`log` TLog<br/>

`variables` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`buildItinerary` [Action\<IItineraryBuilder\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExecutionResult](../masstransit/executionresult)<br/>

### **ReviseItinerary\<TLog\>(TLog, IEnumerable\<KeyValuePair\<String, Object\>\>, Action\<IItineraryBuilder\>)**

```csharp
ExecutionResult ReviseItinerary<TLog>(TLog log, IEnumerable<KeyValuePair<string, object>> variables, Action<IItineraryBuilder> buildItinerary)
```

#### Type Parameters

`TLog`<br/>

#### Parameters

`log` TLog<br/>

`variables` [IEnumerable\<KeyValuePair\<String, Object\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`buildItinerary` [Action\<IItineraryBuilder\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExecutionResult](../masstransit/executionresult)<br/>

### **Terminate()**

Terminate the routing slip (with extreme prejudice), completing it but discarding any remaining itinerary
 activities.

```csharp
ExecutionResult Terminate()
```

#### Returns

[ExecutionResult](../masstransit/executionresult)<br/>

### **Terminate(Object)**

Terminate the routing slip (with extreme prejudice), completing it but discarding any remaining itinerary
 activities.
 An dictionary of values to add/set as variables on the routing slip

```csharp
ExecutionResult Terminate(object variables)
```

#### Parameters

`variables` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[ExecutionResult](../masstransit/executionresult)<br/>

### **Terminate(IEnumerable\<KeyValuePair\<String, Object\>\>)**

Terminate the routing slip (with extreme prejudice), completing it but discarding any remaining itinerary
 activities.
 An dictionary of values to add/set as variables on the routing slip

```csharp
ExecutionResult Terminate(IEnumerable<KeyValuePair<string, object>> variables)
```

#### Parameters

`variables` [IEnumerable\<KeyValuePair\<String, Object\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

#### Returns

[ExecutionResult](../masstransit/executionresult)<br/>

### **Faulted()**

The activity Faulted for an unknown reason, but compensation should be triggered

```csharp
ExecutionResult Faulted()
```

#### Returns

[ExecutionResult](../masstransit/executionresult)<br/>

### **Faulted(Exception)**

The activity Faulted, and compensation should be triggered

```csharp
ExecutionResult Faulted(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[ExecutionResult](../masstransit/executionresult)<br/>

### **FaultedWithVariables(Exception, Object)**

The activity Faulted with no exception, but compensation should be triggered and passing additional variables to set on
 the routing slip

```csharp
ExecutionResult FaultedWithVariables(Exception exception, object variables)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`variables` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
An anonymous object of values to add/set as variables on the routing slip

#### Returns

[ExecutionResult](../masstransit/executionresult)<br/>

### **FaultedWithVariables(Exception, IEnumerable\<KeyValuePair\<String, Object\>\>)**

The activity Faulted with no exception, but compensation should be triggered and passing additional variables to set on
 the routing slip

```csharp
ExecutionResult FaultedWithVariables(Exception exception, IEnumerable<KeyValuePair<string, object>> variables)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`variables` [IEnumerable\<KeyValuePair\<String, Object\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
An dictionary of values to add/set as variables on the routing slip

#### Returns

[ExecutionResult](../masstransit/executionresult)<br/>
