---

title: JobTypeStateMachine

---

# JobTypeStateMachine

Namespace: MassTransit

```csharp
public sealed class JobTypeStateMachine : MassTransitStateMachine<JobTypeSaga>, SagaStateMachine<JobTypeSaga>, StateMachine<JobTypeSaga>, StateMachine, IVisitable, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MassTransitStateMachine\<JobTypeSaga\>](../masstransit/masstransitstatemachine-1) → [JobTypeStateMachine](../masstransit/jobtypestatemachine)<br/>
Implements [SagaStateMachine\<JobTypeSaga\>](../../masstransit-abstractions/masstransit/sagastatemachine-1), [StateMachine\<JobTypeSaga\>](../../masstransit-abstractions/masstransit/statemachine-1), [StateMachine](../../masstransit-abstractions/masstransit/statemachine), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Properties

### **Active**

```csharp
public State Active { get; }
```

#### Property Value

[State](../../masstransit-abstractions/masstransit/state)<br/>

### **Idle**

```csharp
public State Idle { get; }
```

#### Property Value

[State](../../masstransit-abstractions/masstransit/state)<br/>

### **JobSlotRequested**

```csharp
public Event<AllocateJobSlot> JobSlotRequested { get; }
```

#### Property Value

[Event\<AllocateJobSlot\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **JobSlotReleased**

```csharp
public Event<JobSlotReleased> JobSlotReleased { get; }
```

#### Property Value

[Event\<JobSlotReleased\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **SetConcurrentJobLimit**

```csharp
public Event<SetConcurrentJobLimit> SetConcurrentJobLimit { get; }
```

#### Property Value

[Event\<SetConcurrentJobLimit\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **Correlations**

```csharp
public IEnumerable<EventCorrelation> Correlations { get; }
```

#### Property Value

[IEnumerable\<EventCorrelation\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Accessor**

```csharp
public IStateAccessor<JobTypeSaga> Accessor { get; }
```

#### Property Value

[IStateAccessor\<JobTypeSaga\>](../../masstransit-abstractions/masstransit/istateaccessor-1)<br/>

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

### **JobTypeStateMachine()**

```csharp
public JobTypeStateMachine()
```
