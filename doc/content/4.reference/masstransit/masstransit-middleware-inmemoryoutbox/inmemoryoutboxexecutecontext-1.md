---

title: InMemoryOutboxExecuteContext<TArguments>

---

# InMemoryOutboxExecuteContext\<TArguments\>

Namespace: MassTransit.Middleware.InMemoryOutbox

```csharp
public class InMemoryOutboxExecuteContext<TArguments> : InMemoryOutboxCourierContextProxy, IPublishEndpoint, IPublishObserverConnector, ConsumeContext, PipeContext, MessageContext, ISendEndpointProvider, ISendObserverConnector, OutboxContext, ConsumeContext<RoutingSlip>, CourierContext, ExecuteContext<TArguments>, ExecuteContext
```

#### Type Parameters

`TArguments`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PublishEndpoint](../../masstransit-abstractions/masstransit-transports/publishendpoint) → [BaseConsumeContext](../masstransit-context/baseconsumecontext) → [ConsumeContextProxy](../masstransit-context/consumecontextproxy) → [InMemoryOutboxConsumeContext](../masstransit-middleware-inmemoryoutbox/inmemoryoutboxconsumecontext) → [InMemoryOutboxConsumeContext\<RoutingSlip\>](../masstransit-middleware-inmemoryoutbox/inmemoryoutboxconsumecontext-1) → [InMemoryOutboxCourierContextProxy](../masstransit-middleware-inmemoryoutbox/inmemoryoutboxcouriercontextproxy) → [InMemoryOutboxExecuteContext\<TArguments\>](../masstransit-middleware-inmemoryoutbox/inmemoryoutboxexecutecontext-1)<br/>
Implements [IPublishEndpoint](../../masstransit-abstractions/masstransit/ipublishendpoint), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext), [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [MessageContext](../../masstransit-abstractions/masstransit/messagecontext), [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [OutboxContext](../masstransit-middleware-inmemoryoutbox/outboxcontext), [ConsumeContext\<RoutingSlip\>](../../masstransit-abstractions/masstransit/consumecontext-1), [CourierContext](../../masstransit-abstractions/masstransit/couriercontext), [ExecuteContext\<TArguments\>](../../masstransit-abstractions/masstransit/executecontext-1), [ExecuteContext](../../masstransit-abstractions/masstransit/executecontext)

## Properties

### **Result**

```csharp
public ExecutionResult Result { get; set; }
```

#### Property Value

[ExecutionResult](../../masstransit-abstractions/masstransit/executionresult)<br/>

### **Arguments**

```csharp
public TArguments Arguments { get; }
```

#### Property Value

TArguments<br/>

### **Message**

```csharp
public RoutingSlip Message { get; }
```

#### Property Value

[RoutingSlip](../../masstransit-abstractions/masstransit-courier-contracts/routingslip)<br/>

### **CapturedContext**

```csharp
public ConsumeContext CapturedContext { get; }
```

#### Property Value

[ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

### **ClearToSend**

```csharp
public Task ClearToSend { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **CancellationToken**

Returns the CancellationToken for the context (implicit interface)

```csharp
public CancellationToken CancellationToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **MessageId**

```csharp
public Nullable<Guid> MessageId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **RequestId**

```csharp
public Nullable<Guid> RequestId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **CorrelationId**

```csharp
public Nullable<Guid> CorrelationId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ConversationId**

```csharp
public Nullable<Guid> ConversationId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **InitiatorId**

```csharp
public Nullable<Guid> InitiatorId { get; }
```

#### Property Value

[Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ExpirationTime**

```csharp
public Nullable<DateTime> ExpirationTime { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **SourceAddress**

```csharp
public Uri SourceAddress { get; }
```

#### Property Value

Uri<br/>

### **DestinationAddress**

```csharp
public Uri DestinationAddress { get; }
```

#### Property Value

Uri<br/>

### **ResponseAddress**

```csharp
public Uri ResponseAddress { get; }
```

#### Property Value

Uri<br/>

### **FaultAddress**

```csharp
public Uri FaultAddress { get; }
```

#### Property Value

Uri<br/>

### **SentTime**

```csharp
public Nullable<DateTime> SentTime { get; }
```

#### Property Value

[Nullable\<DateTime\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **Headers**

```csharp
public Headers Headers { get; }
```

#### Property Value

[Headers](../../masstransit-abstractions/masstransit/headers)<br/>

### **Host**

```csharp
public HostInfo Host { get; }
```

#### Property Value

[HostInfo](../../masstransit-abstractions/masstransit/hostinfo)<br/>

### **ConsumeCompleted**

```csharp
public Task ConsumeCompleted { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **SupportedMessageTypes**

```csharp
public IEnumerable<string> SupportedMessageTypes { get; }
```

#### Property Value

[IEnumerable\<String\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **ReceiveContext**

```csharp
public ReceiveContext ReceiveContext { get; protected set; }
```

#### Property Value

[ReceiveContext](../../masstransit-abstractions/masstransit/receivecontext)<br/>

### **SerializerContext**

```csharp
public SerializerContext SerializerContext { get; }
```

#### Property Value

[SerializerContext](../../masstransit-abstractions/masstransit/serializercontext)<br/>

## Constructors

### **InMemoryOutboxExecuteContext(ExecuteContext\<TArguments\>)**

```csharp
public InMemoryOutboxExecuteContext(ExecuteContext<TArguments> context)
```

#### Parameters

`context` [ExecuteContext\<TArguments\>](../../masstransit-abstractions/masstransit/executecontext-1)<br/>

## Methods

### **Completed()**

```csharp
public ExecutionResult Completed()
```

#### Returns

[ExecutionResult](../../masstransit-abstractions/masstransit/executionresult)<br/>

### **CompletedWithVariables(IEnumerable\<KeyValuePair\<String, Object\>\>)**

```csharp
public ExecutionResult CompletedWithVariables(IEnumerable<KeyValuePair<string, object>> variables)
```

#### Parameters

`variables` [IEnumerable\<KeyValuePair\<String, Object\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

#### Returns

[ExecutionResult](../../masstransit-abstractions/masstransit/executionresult)<br/>

### **CompletedWithVariables(Object)**

```csharp
public ExecutionResult CompletedWithVariables(object variables)
```

#### Parameters

`variables` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[ExecutionResult](../../masstransit-abstractions/masstransit/executionresult)<br/>

### **Completed\<TLog\>(TLog)**

```csharp
public ExecutionResult Completed<TLog>(TLog log)
```

#### Type Parameters

`TLog`<br/>

#### Parameters

`log` TLog<br/>

#### Returns

[ExecutionResult](../../masstransit-abstractions/masstransit/executionresult)<br/>

### **Completed\<TLog\>(Object)**

```csharp
public ExecutionResult Completed<TLog>(object logValues)
```

#### Type Parameters

`TLog`<br/>

#### Parameters

`logValues` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[ExecutionResult](../../masstransit-abstractions/masstransit/executionresult)<br/>

### **CompletedWithVariables\<TLog\>(TLog, Object)**

```csharp
public ExecutionResult CompletedWithVariables<TLog>(TLog log, object variables)
```

#### Type Parameters

`TLog`<br/>

#### Parameters

`log` TLog<br/>

`variables` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[ExecutionResult](../../masstransit-abstractions/masstransit/executionresult)<br/>

### **CompletedWithVariables\<TLog\>(Object, Object)**

```csharp
public ExecutionResult CompletedWithVariables<TLog>(object logValues, object variables)
```

#### Type Parameters

`TLog`<br/>

#### Parameters

`logValues` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`variables` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[ExecutionResult](../../masstransit-abstractions/masstransit/executionresult)<br/>

### **CompletedWithVariables\<TLog\>(TLog, IEnumerable\<KeyValuePair\<String, Object\>\>)**

```csharp
public ExecutionResult CompletedWithVariables<TLog>(TLog log, IEnumerable<KeyValuePair<string, object>> variables)
```

#### Type Parameters

`TLog`<br/>

#### Parameters

`log` TLog<br/>

`variables` [IEnumerable\<KeyValuePair\<String, Object\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

#### Returns

[ExecutionResult](../../masstransit-abstractions/masstransit/executionresult)<br/>

### **ReviseItinerary(Action\<IItineraryBuilder\>)**

```csharp
public ExecutionResult ReviseItinerary(Action<IItineraryBuilder> buildItinerary)
```

#### Parameters

`buildItinerary` [Action\<IItineraryBuilder\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExecutionResult](../../masstransit-abstractions/masstransit/executionresult)<br/>

### **ReviseItinerary\<TLog\>(TLog, Action\<IItineraryBuilder\>)**

```csharp
public ExecutionResult ReviseItinerary<TLog>(TLog log, Action<IItineraryBuilder> buildItinerary)
```

#### Type Parameters

`TLog`<br/>

#### Parameters

`log` TLog<br/>

`buildItinerary` [Action\<IItineraryBuilder\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExecutionResult](../../masstransit-abstractions/masstransit/executionresult)<br/>

### **ReviseItinerary\<TLog\>(TLog, Object, Action\<IItineraryBuilder\>)**

```csharp
public ExecutionResult ReviseItinerary<TLog>(TLog log, object variables, Action<IItineraryBuilder> buildItinerary)
```

#### Type Parameters

`TLog`<br/>

#### Parameters

`log` TLog<br/>

`variables` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`buildItinerary` [Action\<IItineraryBuilder\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExecutionResult](../../masstransit-abstractions/masstransit/executionresult)<br/>

### **ReviseItinerary\<TLog\>(TLog, IEnumerable\<KeyValuePair\<String, Object\>\>, Action\<IItineraryBuilder\>)**

```csharp
public ExecutionResult ReviseItinerary<TLog>(TLog log, IEnumerable<KeyValuePair<string, object>> variables, Action<IItineraryBuilder> buildItinerary)
```

#### Type Parameters

`TLog`<br/>

#### Parameters

`log` TLog<br/>

`variables` [IEnumerable\<KeyValuePair\<String, Object\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`buildItinerary` [Action\<IItineraryBuilder\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[ExecutionResult](../../masstransit-abstractions/masstransit/executionresult)<br/>

### **Terminate()**

```csharp
public ExecutionResult Terminate()
```

#### Returns

[ExecutionResult](../../masstransit-abstractions/masstransit/executionresult)<br/>

### **Terminate(Object)**

```csharp
public ExecutionResult Terminate(object variables)
```

#### Parameters

`variables` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[ExecutionResult](../../masstransit-abstractions/masstransit/executionresult)<br/>

### **Terminate(IEnumerable\<KeyValuePair\<String, Object\>\>)**

```csharp
public ExecutionResult Terminate(IEnumerable<KeyValuePair<string, object>> variables)
```

#### Parameters

`variables` [IEnumerable\<KeyValuePair\<String, Object\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

#### Returns

[ExecutionResult](../../masstransit-abstractions/masstransit/executionresult)<br/>

### **Faulted()**

```csharp
public ExecutionResult Faulted()
```

#### Returns

[ExecutionResult](../../masstransit-abstractions/masstransit/executionresult)<br/>

### **Faulted(Exception)**

```csharp
public ExecutionResult Faulted(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[ExecutionResult](../../masstransit-abstractions/masstransit/executionresult)<br/>

### **FaultedWithVariables(Exception, Object)**

```csharp
public ExecutionResult FaultedWithVariables(Exception exception, object variables)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`variables` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[ExecutionResult](../../masstransit-abstractions/masstransit/executionresult)<br/>

### **FaultedWithVariables(Exception, IEnumerable\<KeyValuePair\<String, Object\>\>)**

```csharp
public ExecutionResult FaultedWithVariables(Exception exception, IEnumerable<KeyValuePair<string, object>> variables)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`variables` [IEnumerable\<KeyValuePair\<String, Object\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

#### Returns

[ExecutionResult](../../masstransit-abstractions/masstransit/executionresult)<br/>

### **CreateActivityContext\<TActivity\>(TActivity)**

```csharp
public ExecuteActivityContext<TActivity, TArguments> CreateActivityContext<TActivity>(TActivity activity)
```

#### Type Parameters

`TActivity`<br/>

#### Parameters

`activity` TActivity<br/>

#### Returns

[ExecuteActivityContext\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/executeactivitycontext-2)<br/>
