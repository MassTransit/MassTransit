---

title: JobStateMachine

---

# JobStateMachine

Namespace: MassTransit

```csharp
public sealed class JobStateMachine : MassTransitStateMachine<JobSaga>, SagaStateMachine<JobSaga>, StateMachine<JobSaga>, StateMachine, IVisitable, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MassTransitStateMachine\<JobSaga\>](../masstransit/masstransitstatemachine-1) → [JobStateMachine](../masstransit/jobstatemachine)<br/>
Implements [SagaStateMachine\<JobSaga\>](../../masstransit-abstractions/masstransit/sagastatemachine-1), [StateMachine\<JobSaga\>](../../masstransit-abstractions/masstransit/statemachine-1), [StateMachine](../../masstransit-abstractions/masstransit/statemachine), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Properties

### **Submitted**

```csharp
public State Submitted { get; }
```

#### Property Value

[State](../../masstransit-abstractions/masstransit/state)<br/>

### **WaitingToStart**

```csharp
public State WaitingToStart { get; }
```

#### Property Value

[State](../../masstransit-abstractions/masstransit/state)<br/>

### **WaitingToRetry**

```csharp
public State WaitingToRetry { get; }
```

#### Property Value

[State](../../masstransit-abstractions/masstransit/state)<br/>

### **WaitingForSlot**

```csharp
public State WaitingForSlot { get; }
```

#### Property Value

[State](../../masstransit-abstractions/masstransit/state)<br/>

### **Started**

```csharp
public State Started { get; }
```

#### Property Value

[State](../../masstransit-abstractions/masstransit/state)<br/>

### **Completed**

```csharp
public State Completed { get; }
```

#### Property Value

[State](../../masstransit-abstractions/masstransit/state)<br/>

### **Canceled**

```csharp
public State Canceled { get; }
```

#### Property Value

[State](../../masstransit-abstractions/masstransit/state)<br/>

### **Faulted**

```csharp
public State Faulted { get; }
```

#### Property Value

[State](../../masstransit-abstractions/masstransit/state)<br/>

### **AllocatingJobSlot**

```csharp
public State AllocatingJobSlot { get; }
```

#### Property Value

[State](../../masstransit-abstractions/masstransit/state)<br/>

### **StartingJobAttempt**

```csharp
public State StartingJobAttempt { get; }
```

#### Property Value

[State](../../masstransit-abstractions/masstransit/state)<br/>

### **CancellationPending**

```csharp
public State CancellationPending { get; }
```

#### Property Value

[State](../../masstransit-abstractions/masstransit/state)<br/>

### **JobSlotAllocated**

```csharp
public Event<JobSlotAllocated> JobSlotAllocated { get; }
```

#### Property Value

[Event\<JobSlotAllocated\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **JobSlotUnavailable**

```csharp
public Event<JobSlotUnavailable> JobSlotUnavailable { get; }
```

#### Property Value

[Event\<JobSlotUnavailable\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **AllocateJobSlotFaulted**

```csharp
public Event<Fault<AllocateJobSlot>> AllocateJobSlotFaulted { get; }
```

#### Property Value

[Event\<Fault\<AllocateJobSlot\>\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **StartJobAttemptFaulted**

```csharp
public Event<Fault<StartJobAttempt>> StartJobAttemptFaulted { get; }
```

#### Property Value

[Event\<Fault\<StartJobAttempt\>\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **JobSubmitted**

```csharp
public Event<JobSubmitted> JobSubmitted { get; }
```

#### Property Value

[Event\<JobSubmitted\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **AttemptStarted**

```csharp
public Event<JobAttemptStarted> AttemptStarted { get; }
```

#### Property Value

[Event\<JobAttemptStarted\>](../../masstransit-abstractions/masstransit/event-1)<br/>

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

### **AttemptFaulted**

```csharp
public Event<JobAttemptFaulted> AttemptFaulted { get; }
```

#### Property Value

[Event\<JobAttemptFaulted\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **JobCompleted**

```csharp
public Event<JobCompleted> JobCompleted { get; }
```

#### Property Value

[Event\<JobCompleted\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **CancelJob**

```csharp
public Event<CancelJob> CancelJob { get; }
```

#### Property Value

[Event\<CancelJob\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **RetryJob**

```csharp
public Event<RetryJob> RetryJob { get; }
```

#### Property Value

[Event\<RetryJob\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **RunJob**

```csharp
public Event<RunJob> RunJob { get; }
```

#### Property Value

[Event\<RunJob\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **FinalizeJob**

```csharp
public Event<FinalizeJob> FinalizeJob { get; }
```

#### Property Value

[Event\<FinalizeJob\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **SetJobProgress**

```csharp
public Event<SetJobProgress> SetJobProgress { get; }
```

#### Property Value

[Event\<SetJobProgress\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **SaveJobState**

```csharp
public Event<SaveJobState> SaveJobState { get; }
```

#### Property Value

[Event\<SaveJobState\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **GetJobState**

```csharp
public Event<GetJobState> GetJobState { get; }
```

#### Property Value

[Event\<GetJobState\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **JobSlotWaitElapsed**

```csharp
public Schedule<JobSaga, JobSlotWaitElapsed> JobSlotWaitElapsed { get; }
```

#### Property Value

[Schedule\<JobSaga, JobSlotWaitElapsed\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

### **JobRetryDelayElapsed**

```csharp
public Schedule<JobSaga, JobRetryDelayElapsed> JobRetryDelayElapsed { get; }
```

#### Property Value

[Schedule\<JobSaga, JobRetryDelayElapsed\>](../../masstransit-abstractions/masstransit/schedule-2)<br/>

### **Correlations**

```csharp
public IEnumerable<EventCorrelation> Correlations { get; }
```

#### Property Value

[IEnumerable\<EventCorrelation\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Accessor**

```csharp
public IStateAccessor<JobSaga> Accessor { get; }
```

#### Property Value

[IStateAccessor\<JobSaga\>](../../masstransit-abstractions/masstransit/istateaccessor-1)<br/>

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

### **JobStateMachine()**

```csharp
public JobStateMachine()
```
