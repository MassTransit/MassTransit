---

title: Future<TCommand, TResult, TFault>

---

# Future\<TCommand, TResult, TFault\>

Namespace: MassTransit

A future is a deterministic, durable service that given a command, executes any number
 of requests, routing slips, functions, etc. to produce a result. Once the result has been set,
 it is available to any subsequent commands and requests for the result.

```csharp
public abstract class Future<TCommand, TResult, TFault> : MassTransitStateMachine<FutureState>, SagaStateMachine<FutureState>, StateMachine<FutureState>, StateMachine, IVisitable, IProbeSite, IFutureStateMachineConfigurator
```

#### Type Parameters

`TCommand`<br/>
The command type that creates the future

`TResult`<br/>
The result type that completes the future

`TFault`<br/>
The fault type that faults the future

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MassTransitStateMachine\<FutureState\>](../masstransit/masstransitstatemachine-1) → [Future\<TCommand, TResult, TFault\>](../masstransit/future-3)<br/>
Implements [SagaStateMachine\<FutureState\>](../../masstransit-abstractions/masstransit/sagastatemachine-1), [StateMachine\<FutureState\>](../../masstransit-abstractions/masstransit/statemachine-1), [StateMachine](../../masstransit-abstractions/masstransit/statemachine), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IFutureStateMachineConfigurator](../masstransit-futures/ifuturestatemachineconfigurator)

## Properties

### **WaitingForCompletion**

```csharp
public State WaitingForCompletion { get; protected set; }
```

#### Property Value

[State](../../masstransit-abstractions/masstransit/state)<br/>

### **Completed**

```csharp
public State Completed { get; protected set; }
```

#### Property Value

[State](../../masstransit-abstractions/masstransit/state)<br/>

### **Faulted**

```csharp
public State Faulted { get; protected set; }
```

#### Property Value

[State](../../masstransit-abstractions/masstransit/state)<br/>

### **CommandReceived**

Initiates and correlates the command to the future. Subsequent commands received while waiting for completion
 are added as subscribers.

```csharp
public Event<TCommand> CommandReceived { get; protected set; }
```

#### Property Value

[Event\<TCommand\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **ResultRequested**

Used by a Future Reference to get the future's result once completed or fault once faulted.

```csharp
public Event<Get<TCommand>> ResultRequested { get; protected set; }
```

#### Property Value

[Event\<Get\<TCommand\>\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **Correlations**

```csharp
public IEnumerable<EventCorrelation> Correlations { get; }
```

#### Property Value

[IEnumerable\<EventCorrelation\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Accessor**

```csharp
public IStateAccessor<FutureState> Accessor { get; }
```

#### Property Value

[IStateAccessor\<FutureState\>](../../masstransit-abstractions/masstransit/istateaccessor-1)<br/>

### **Initial**

```csharp
public State Initial { get; }
```

#### Property Value

[State](../../masstransit-abstractions/masstransit/state)<br/>

### **Final**

```csharp
public State Final { get; }
```

#### Property Value

[State](../../masstransit-abstractions/masstransit/state)<br/>

### **States**

```csharp
public IEnumerable<State> States { get; }
```

#### Property Value

[IEnumerable\<State\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Events**

```csharp
public IEnumerable<Event> Events { get; }
```

#### Property Value

[IEnumerable\<Event\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

## Methods

### **ConfigureCommand(Action\<IEventCorrelationConfigurator\<FutureState, TCommand\>\>)**

Configure the initiating command, including correlation, etc.

```csharp
protected void ConfigureCommand(Action<IEventCorrelationConfigurator<FutureState, TCommand>> configure)
```

#### Parameters

`configure` [Action\<IEventCorrelationConfigurator\<FutureState, TCommand\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **SendRequest\<TRequest\>(Action\<IFutureRequestConfigurator\<TFault, TCommand, TRequest\>\>)**

Send a request when the future is requested

```csharp
protected FutureRequestHandle<TCommand, TResult, TFault, TRequest> SendRequest<TRequest>(Action<IFutureRequestConfigurator<TFault, TCommand, TRequest>> configure)
```

#### Type Parameters

`TRequest`<br/>
The request type to send

#### Parameters

`configure` [Action\<IFutureRequestConfigurator\<TFault, TCommand, TRequest\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[FutureRequestHandle\<TCommand, TResult, TFault, TRequest\>](../masstransit/futurerequesthandle-4)<br/>

### **SendRequest\<TInput, TRequest\>(Func\<TCommand, TInput\>, Action\<IFutureRequestConfigurator\<TFault, TInput, TRequest\>\>)**

Send a request when the future is requested

```csharp
protected FutureRequestHandle<TCommand, TResult, TFault, TRequest> SendRequest<TInput, TRequest>(Func<TCommand, TInput> inputSelector, Action<IFutureRequestConfigurator<TFault, TInput, TRequest>> configure)
```

#### Type Parameters

`TInput`<br/>
The input type

`TRequest`<br/>
The request type to send

#### Parameters

`inputSelector` [Func\<TCommand, TInput\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
Specify an input property from the command to use as the input for the request

`configure` [Action\<IFutureRequestConfigurator\<TFault, TInput, TRequest\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[FutureRequestHandle\<TCommand, TResult, TFault, TRequest\>](../masstransit/futurerequesthandle-4)<br/>

### **SendRequests\<TInput, TRequest\>(Func\<TCommand, IEnumerable\<TInput\>\>, Action\<IFutureRequestConfigurator\<TFault, TInput, TRequest\>\>)**

Sends multiple requests when the future is requested, using an enumerable request property as the source

```csharp
protected FutureRequestHandle<TCommand, TResult, TFault, TRequest> SendRequests<TInput, TRequest>(Func<TCommand, IEnumerable<TInput>> inputSelector, Action<IFutureRequestConfigurator<TFault, TInput, TRequest>> configure)
```

#### Type Parameters

`TInput`<br/>
The input property type

`TRequest`<br/>
The request type to send

#### Parameters

`inputSelector` [Func\<TCommand, IEnumerable\<TInput\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`configure` [Action\<IFutureRequestConfigurator\<TFault, TInput, TRequest\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[FutureRequestHandle\<TCommand, TResult, TFault, TRequest\>](../masstransit/futurerequesthandle-4)<br/>

### **ExecuteRoutingSlip(Action\<IFutureRoutingSlipConfigurator\<TResult, TFault, TCommand\>\>)**

Execute a routing slip when the future is requested

```csharp
protected FutureRoutingSlipHandle ExecuteRoutingSlip(Action<IFutureRoutingSlipConfigurator<TResult, TFault, TCommand>> configure)
```

#### Parameters

`configure` [Action\<IFutureRoutingSlipConfigurator\<TResult, TFault, TCommand\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[FutureRoutingSlipHandle](../masstransit/futureroutingsliphandle)<br/>

### **FaultPendingRequest\<T\>(Event\<Fault\<T\>\>, PendingFutureIdProvider\<T\>)**

```csharp
public void FaultPendingRequest<T>(Event<Fault<T>> requestFaulted, PendingFutureIdProvider<T> pendingIdProvider)
```

#### Type Parameters

`T`<br/>

#### Parameters

`requestFaulted` [Event\<Fault\<T\>\>](../../masstransit-abstractions/masstransit/event-1)<br/>

`pendingIdProvider` [PendingFutureIdProvider\<T\>](../masstransit/pendingfutureidprovider-1)<br/>

### **SetFaulted\<T\>(Event\<T\>, Func\<BehaviorContext\<FutureState, T\>, Task\>)**

```csharp
public void SetFaulted<T>(Event<T> faultEvent, Func<BehaviorContext<FutureState, T>, Task> callback)
```

#### Type Parameters

`T`<br/>

#### Parameters

`faultEvent` [Event\<T\>](../../masstransit-abstractions/masstransit/event-1)<br/>

`callback` [Func\<BehaviorContext\<FutureState, T\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **RequestIdOrFault(MessageContext)**

```csharp
protected static Guid RequestIdOrFault(MessageContext context)
```

#### Parameters

`context` [MessageContext](../../masstransit-abstractions/masstransit/messagecontext)<br/>

#### Returns

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **RequestIdOrDefault(MessageContext)**

```csharp
protected static Guid RequestIdOrDefault(MessageContext context)
```

#### Parameters

`context` [MessageContext](../../masstransit-abstractions/masstransit/messagecontext)<br/>

#### Returns

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **FutureIdOrFault(ConsumeContext, IDictionary\<String, Object\>)**

```csharp
protected static Guid FutureIdOrFault(ConsumeContext context, IDictionary<string, object> variables)
```

#### Parameters

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`variables` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

#### Returns

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **FutureIdOrDefault(ConsumeContext, IDictionary\<String, Object\>)**

```csharp
protected static Guid FutureIdOrDefault(ConsumeContext context, IDictionary<string, object> variables)
```

#### Parameters

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`variables` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

#### Returns

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **WhenAllCompleted(Action\<IFutureResultConfigurator\<TResult\>\>)**

```csharp
protected void WhenAllCompleted(Action<IFutureResultConfigurator<TResult>> configure)
```

#### Parameters

`configure` [Action\<IFutureResultConfigurator\<TResult\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **WhenAnyFaulted(Action\<IFutureFaultConfigurator\<TFault\>\>)**

When any result faulted, Set the future Faulted

```csharp
protected void WhenAnyFaulted(Action<IFutureFaultConfigurator<TFault>> configure)
```

#### Parameters

`configure` [Action\<IFutureFaultConfigurator\<TFault\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **WhenAllCompletedOrFaulted(Action\<IFutureFaultConfigurator\<TFault\>\>)**

When all requests have either completed or faulted, Set the future Faulted

```csharp
protected void WhenAllCompletedOrFaulted(Action<IFutureFaultConfigurator<TFault>> configure)
```

#### Parameters

`configure` [Action\<IFutureFaultConfigurator\<TFault\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
