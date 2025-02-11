---

title: IStateObserver<TSaga>

---

# IStateObserver\<TSaga\>

Namespace: MassTransit

```csharp
public interface IStateObserver<TSaga>
```

#### Type Parameters

`TSaga`<br/>

## Methods

### **StateChanged(BehaviorContext\<TSaga\>, State, State)**

Invoked prior to changing the state of the state machine

```csharp
Task StateChanged(BehaviorContext<TSaga> context, State currentState, State previousState)
```

#### Parameters

`context` [BehaviorContext\<TSaga\>](../masstransit/behaviorcontext-1)<br/>
The instance context of the state machine

`currentState` [State](../masstransit/state)<br/>
The current state (after the change)

`previousState` [State](../masstransit/state)<br/>
The previous state (before the change)

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
