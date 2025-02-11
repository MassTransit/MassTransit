---

title: RequestConsumerFuture<TRequest, TResponse>

---

# RequestConsumerFuture\<TRequest, TResponse\>

Namespace: MassTransit

```csharp
public class RequestConsumerFuture<TRequest, TResponse> : Future<TRequest, TResponse>, SagaStateMachine<FutureState>, StateMachine<FutureState>, StateMachine, IVisitable, IProbeSite, IFutureStateMachineConfigurator
```

#### Type Parameters

`TRequest`<br/>

`TResponse`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MassTransitStateMachine\<FutureState\>](../masstransit/masstransitstatemachine-1) → [Future\<TRequest, TResponse, Fault\<TRequest\>\>](../masstransit/future-3) → [Future\<TRequest, TResponse\>](../masstransit/future-2) → [RequestConsumerFuture\<TRequest, TResponse\>](../masstransit/requestconsumerfuture-2)<br/>
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
public Event<TRequest> CommandReceived { get; protected set; }
```

#### Property Value

[Event\<TRequest\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **ResultRequested**

Used by a Future Reference to get the future's result once completed or fault once faulted.

```csharp
public Event<Get<TRequest>> ResultRequested { get; protected set; }
```

#### Property Value

[Event\<Get\<TRequest\>\>](../../masstransit-abstractions/masstransit/event-1)<br/>

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

## Constructors

### **RequestConsumerFuture(IFutureDefinition)**

```csharp
public RequestConsumerFuture(IFutureDefinition definition)
```

#### Parameters

`definition` [IFutureDefinition](../../masstransit-abstractions/masstransit/ifuturedefinition)<br/>
