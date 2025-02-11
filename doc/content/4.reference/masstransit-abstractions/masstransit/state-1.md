---

title: State<TSaga>

---

# State\<TSaga\>

Namespace: MassTransit

A state within a state machine that can be targeted with events

```csharp
public interface State<TSaga> : State, IVisitable, IProbeSite, IComparable<State>
```

#### Type Parameters

`TSaga`<br/>
The instance type to which the state applies

Implements [State](../masstransit/state), [IVisitable](../masstransit/ivisitable), [IProbeSite](../masstransit/iprobesite), [IComparable\<State\>](https://learn.microsoft.com/en-us/dotnet/api/system.icomparable-1)

## Properties

### **Events**

```csharp
public abstract IEnumerable<Event> Events { get; }
```

#### Property Value

[IEnumerable\<Event\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **SuperState**

Returns the superState of the state, if there is one

```csharp
public abstract State<TSaga> SuperState { get; }
```

#### Property Value

[State\<TSaga\>](../masstransit/state-1)<br/>

## Methods

### **Raise(BehaviorContext\<TSaga\>)**

```csharp
Task Raise(BehaviorContext<TSaga> context)
```

#### Parameters

`context` [BehaviorContext\<TSaga\>](../masstransit/behaviorcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Raise\<T\>(BehaviorContext\<TSaga, T\>)**

Raise an event to the state, passing the instance

```csharp
Task Raise<T>(BehaviorContext<TSaga, T> context)
```

#### Type Parameters

`T`<br/>
The event data type

#### Parameters

`context` [BehaviorContext\<TSaga, T\>](../masstransit/behaviorcontext-2)<br/>
The event context

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Bind(Event, IStateMachineActivity\<TSaga\>)**

Bind an activity to an event

```csharp
void Bind(Event event, IStateMachineActivity<TSaga> activity)
```

#### Parameters

`event` [Event](../masstransit/event)<br/>

`activity` [IStateMachineActivity\<TSaga\>](../masstransit/istatemachineactivity-1)<br/>

### **Ignore(Event)**

Ignore the specified event in this state. Prevents an exception from being thrown if
 the event is raised during this state.

```csharp
void Ignore(Event event)
```

#### Parameters

`event` [Event](../masstransit/event)<br/>

### **Ignore\<T\>(Event\<T\>, StateMachineCondition\<TSaga, T\>)**

Ignore the specified event in this state if the filter condition passed. Prevents exceptions
 from being thrown if the event is raised during this state.

```csharp
void Ignore<T>(Event<T> event, StateMachineCondition<TSaga, T> filter)
```

#### Type Parameters

`T`<br/>

#### Parameters

`event` [Event\<T\>](../masstransit/event-1)<br/>

`filter` [StateMachineCondition\<TSaga, T\>](../masstransit/statemachinecondition-2)<br/>

### **AddSubstate(State\<TSaga\>)**

Adds a substate to the state

```csharp
void AddSubstate(State<TSaga> subState)
```

#### Parameters

`subState` [State\<TSaga\>](../masstransit/state-1)<br/>

### **HasState(State\<TSaga\>)**

True if the specified state is included in the state

```csharp
bool HasState(State<TSaga> state)
```

#### Parameters

`state` [State\<TSaga\>](../masstransit/state-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsStateOf(State\<TSaga\>)**

True if the specified state is a substate of the current state

```csharp
bool IsStateOf(State<TSaga> state)
```

#### Parameters

`state` [State\<TSaga\>](../masstransit/state-1)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
