---

title: IActivityBinder<TSaga>

---

# IActivityBinder\<TSaga\>

Namespace: MassTransit.SagaStateMachine

```csharp
public interface IActivityBinder<TSaga>
```

#### Type Parameters

`TSaga`<br/>

## Properties

### **Event**

```csharp
public abstract Event Event { get; }
```

#### Property Value

[Event](../../masstransit-abstractions/masstransit/event)<br/>

## Methods

### **IsStateTransitionEvent(State)**

Returns True if the event is a state transition event (enter/leave/afterLeave/beforeEnter)
 for the specified state.

```csharp
bool IsStateTransitionEvent(State state)
```

#### Parameters

`state` [State](../../masstransit-abstractions/masstransit/state)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Bind(State\<TSaga\>)**

Binds the activity to the state, may also just ignore the event if it's an ignore event

```csharp
void Bind(State<TSaga> state)
```

#### Parameters

`state` [State\<TSaga\>](../../masstransit-abstractions/masstransit/state-1)<br/>

### **Bind(IBehaviorBuilder\<TSaga\>)**

Bind the activities to the builder

```csharp
void Bind(IBehaviorBuilder<TSaga> builder)
```

#### Parameters

`builder` [IBehaviorBuilder\<TSaga\>](../masstransit-sagastatemachine/ibehaviorbuilder-1)<br/>
