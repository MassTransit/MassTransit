---

title: RequestStateMachine

---

# RequestStateMachine

Namespace: MassTransit.Components

Tracks a request, which was sent to a saga, and the saga deferred until some operation
 is completed, after which it will produce an event to trigger the response.

```csharp
public class RequestStateMachine : MassTransitStateMachine<RequestState>, SagaStateMachine<RequestState>, StateMachine<RequestState>, StateMachine, IVisitable, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MassTransitStateMachine\<RequestState\>](../masstransit/masstransitstatemachine-1) → [RequestStateMachine](../masstransit-components/requeststatemachine)<br/>
Implements [SagaStateMachine\<RequestState\>](../../masstransit-abstractions/masstransit/sagastatemachine-1), [StateMachine\<RequestState\>](../../masstransit-abstractions/masstransit/statemachine-1), [StateMachine](../../masstransit-abstractions/masstransit/statemachine), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Properties

### **Pending**

```csharp
public State Pending { get; }
```

#### Property Value

[State](../../masstransit-abstractions/masstransit/state)<br/>

### **Started**

```csharp
public Event<RequestStarted> Started { get; }
```

#### Property Value

[Event\<RequestStarted\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **Completed**

```csharp
public Event<RequestCompleted> Completed { get; }
```

#### Property Value

[Event\<RequestCompleted\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **Faulted**

```csharp
public Event<RequestFaulted> Faulted { get; }
```

#### Property Value

[Event\<RequestFaulted\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **Correlations**

```csharp
public IEnumerable<EventCorrelation> Correlations { get; }
```

#### Property Value

[IEnumerable\<EventCorrelation\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Accessor**

```csharp
public IStateAccessor<RequestState> Accessor { get; }
```

#### Property Value

[IStateAccessor\<RequestState\>](../../masstransit-abstractions/masstransit/istateaccessor-1)<br/>

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

### **RequestStateMachine()**

```csharp
public RequestStateMachine()
```

## Methods

### **RedeliverOnMissingInstance(Action\<IMissingInstanceRedeliveryConfigurator\>)**

Configure the state machine to redeliver  and  events
 in the scenario where they arrive prior to the  event.

```csharp
public static void RedeliverOnMissingInstance(Action<IMissingInstanceRedeliveryConfigurator> configure)
```

#### Parameters

`configure` [Action\<IMissingInstanceRedeliveryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
A redelivery configuration callback
