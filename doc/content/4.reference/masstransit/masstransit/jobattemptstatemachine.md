---

title: JobAttemptStateMachine

---

# JobAttemptStateMachine

Namespace: MassTransit

```csharp
public sealed class JobAttemptStateMachine : MassTransitStateMachine<JobAttemptSaga>, SagaStateMachine<JobAttemptSaga>, StateMachine<JobAttemptSaga>, StateMachine, IVisitable, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MassTransitStateMachine\<JobAttemptSaga\>](../masstransit/masstransitstatemachine-1) → [JobAttemptStateMachine](../masstransit/jobattemptstatemachine)<br/>
Implements [SagaStateMachine\<JobAttemptSaga\>](../../masstransit-abstractions/masstransit/sagastatemachine-1), [StateMachine\<JobAttemptSaga\>](../../masstransit-abstractions/masstransit/statemachine-1), [StateMachine](../../masstransit-abstractions/masstransit/statemachine), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Properties

### **Starting**

```csharp
public State Starting { get; }
```

#### Property Value

[State](../../masstransit-abstractions/masstransit/state)<br/>

### **Running**

```csharp
public State Running { get; }
```

#### Property Value

[State](../../masstransit-abstractions/masstransit/state)<br/>

### **CheckingStatus**

```csharp
public State CheckingStatus { get; }
```

#### Property Value

[State](../../masstransit-abstractions/masstransit/state)<br/>

### **Suspect**

```csharp
public State Suspect { get; }
```

#### Property Value

[State](../../masstransit-abstractions/masstransit/state)<br/>

### **Faulted**

```csharp
public State Faulted { get; }
```

#### Property Value

[State](../../masstransit-abstractions/masstransit/state)<br/>

### **StartJobAttempt**

```csharp
public Event<StartJobAttempt> StartJobAttempt { get; }
```

#### Property Value

[Event\<StartJobAttempt\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **StartJobFaulted**

```csharp
public Event<Fault<StartJob>> StartJobFaulted { get; }
```

#### Property Value

[Event\<Fault\<StartJob\>\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **FinalizeJobAttempt**

```csharp
public Event<FinalizeJobAttempt> FinalizeJobAttempt { get; }
```

#### Property Value

[Event\<FinalizeJobAttempt\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **CancelJobAttempt**

```csharp
public Event<CancelJobAttempt> CancelJobAttempt { get; }
```

#### Property Value

[Event\<CancelJobAttempt\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **AttemptStarted**

```csharp
public Event<JobAttemptStarted> AttemptStarted { get; }
```

#### Property Value

[Event\<JobAttemptStarted\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **AttemptFaulted**

```csharp
public Event<JobAttemptFaulted> AttemptFaulted { get; }
```

#### Property Value

[Event\<JobAttemptFaulted\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **AttemptCompleted**

```csharp
public Event<JobAttemptCompleted> AttemptCompleted { get; }
```

#### Property Value

[Event\<JobAttemptCompleted\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **AttemptCanceled**

```csharp
public Event<JobAttemptCanceled> AttemptCanceled { get; }
```

#### Property Value

[Event\<JobAttemptCanceled\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **AttemptStatus**

```csharp
public Event<JobAttemptStatus> AttemptStatus { get; }
```

#### Property Value

[Event\<JobAttemptStatus\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **StatusCheckRequested**

```csharp
public Schedule<JobAttemptSaga, JobStatusCheckRequested> StatusCheckRequested { get; }
```

#### Property Value

[Schedule\<JobAttemptSaga, JobStatusCheckRequested\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

### **Correlations**

```csharp
public IEnumerable<EventCorrelation> Correlations { get; }
```

#### Property Value

[IEnumerable\<EventCorrelation\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Accessor**

```csharp
public IStateAccessor<JobAttemptSaga> Accessor { get; }
```

#### Property Value

[IStateAccessor\<JobAttemptSaga\>](../../masstransit-abstractions/masstransit/istateaccessor-1)<br/>

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

### **JobAttemptStateMachine()**

```csharp
public JobAttemptStateMachine()
```
