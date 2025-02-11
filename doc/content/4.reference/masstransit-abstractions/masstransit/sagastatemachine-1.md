---

title: SagaStateMachine<TSaga>

---

# SagaStateMachine\<TSaga\>

Namespace: MassTransit

```csharp
public interface SagaStateMachine<TSaga> : StateMachine<TSaga>, StateMachine, IVisitable, IProbeSite
```

#### Type Parameters

`TSaga`<br/>

Implements [StateMachine\<TSaga\>](../masstransit/statemachine-1), [StateMachine](../masstransit/statemachine), [IVisitable](../masstransit/ivisitable), [IProbeSite](../masstransit/iprobesite)

## Properties

### **Correlations**

Returns the event correlations for the state machine

```csharp
public abstract IEnumerable<EventCorrelation> Correlations { get; }
```

#### Property Value

[IEnumerable\<EventCorrelation\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

## Methods

### **IsCompleted(BehaviorContext\<TSaga\>)**

Returns true if the saga state machine instance is complete and can be removed from the repository

```csharp
Task<bool> IsCompleted(BehaviorContext<TSaga> context)
```

#### Parameters

`context` [BehaviorContext\<TSaga\>](../masstransit/behaviorcontext-1)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
