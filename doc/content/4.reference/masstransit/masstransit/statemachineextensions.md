---

title: StateMachineExtensions

---

# StateMachineExtensions

Namespace: MassTransit

```csharp
public static class StateMachineExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [StateMachineExtensions](../masstransit/statemachineextensions)

## Methods

### **TransitionToState\<TSaga\>(BehaviorContext\<TSaga\>, State)**

Transition a state machine instance to a specific state, producing any events related
 to the transaction such as leaving the previous state and entering the target state

```csharp
public static Task TransitionToState<TSaga>(BehaviorContext<TSaga> context, State state)
```

#### Type Parameters

`TSaga`<br/>
The state instance type

#### Parameters

`context` [BehaviorContext\<TSaga\>](../../masstransit-abstractions/masstransit/behaviorcontext-1)<br/>

`state` [State](../../masstransit-abstractions/masstransit/state)<br/>
The target state

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
