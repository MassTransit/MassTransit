---

title: MassTransitStateMachine<TInstance>

---

# MassTransitStateMachine\<TInstance\>

Namespace: MassTransit

A MassTransit state machine adds functionality on top of Automatonymous supporting
 things like request/response, and correlating events to the state machine, as well
 as retry and policy configuration.

```csharp
public class MassTransitStateMachine<TInstance> : SagaStateMachine<TInstance>, StateMachine<TInstance>, StateMachine, IVisitable, IProbeSite
```

#### Type Parameters

`TInstance`<br/>
The state instance type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MassTransitStateMachine\<TInstance\>](../masstransit/masstransitstatemachine-1)<br/>
Implements [SagaStateMachine\<TInstance\>](../../masstransit-abstractions/masstransit/sagastatemachine-1), [StateMachine\<TInstance\>](../../masstransit-abstractions/masstransit/statemachine-1), [StateMachine](../../masstransit-abstractions/masstransit/statemachine), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Properties

### **Correlations**

```csharp
public IEnumerable<EventCorrelation> Correlations { get; }
```

#### Property Value

[IEnumerable\<EventCorrelation\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Accessor**

```csharp
public IStateAccessor<TInstance> Accessor { get; }
```

#### Property Value

[IStateAccessor\<TInstance\>](../../masstransit-abstractions/masstransit/istateaccessor-1)<br/>

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

## Methods

### **GetState(String)**

```csharp
public State<TInstance> GetState(string name)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[State\<TInstance\>](../../masstransit-abstractions/masstransit/state-1)<br/>

### **NextEvents(State)**

```csharp
public IEnumerable<Event> NextEvents(State state)
```

#### Parameters

`state` [State](../../masstransit-abstractions/masstransit/state)<br/>

#### Returns

[IEnumerable\<Event\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **IsCompositeEvent(Event)**

```csharp
public bool IsCompositeEvent(Event event)
```

#### Parameters

`event` [Event](../../masstransit-abstractions/masstransit/event)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Accept(StateMachineVisitor)**

```csharp
public void Accept(StateMachineVisitor visitor)
```

#### Parameters

`visitor` [StateMachineVisitor](../../masstransit-abstractions/masstransit/statemachinevisitor)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **ConnectEventObserver(IEventObserver\<TInstance\>)**

```csharp
public IDisposable ConnectEventObserver(IEventObserver<TInstance> observer)
```

#### Parameters

`observer` [IEventObserver\<TInstance\>](../../masstransit-abstractions/masstransit/ieventobserver-1)<br/>

#### Returns

[IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br/>

### **ConnectEventObserver(Event, IEventObserver\<TInstance\>)**

```csharp
public IDisposable ConnectEventObserver(Event event, IEventObserver<TInstance> observer)
```

#### Parameters

`event` [Event](../../masstransit-abstractions/masstransit/event)<br/>

`observer` [IEventObserver\<TInstance\>](../../masstransit-abstractions/masstransit/ieventobserver-1)<br/>

#### Returns

[IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br/>

### **ConnectStateObserver(IStateObserver\<TInstance\>)**

```csharp
public IDisposable ConnectStateObserver(IStateObserver<TInstance> stateObserver)
```

#### Parameters

`stateObserver` [IStateObserver\<TInstance\>](../../masstransit-abstractions/masstransit/istateobserver-1)<br/>

#### Returns

[IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br/>

### **InstanceState(Expression\<Func\<TInstance, State\>\>)**

Declares what property holds the TInstance's state on the current instance of the state machine

```csharp
protected internal void InstanceState(Expression<Func<TInstance, State>> instanceStateProperty)
```

#### Parameters

`instanceStateProperty` Expression\<Func\<TInstance, State\>\><br/>

**Remarks:**

Setting the state accessor more than once will cause the property managed by the state machine to change each time.
 Please note, the state machine can only manage one property at a given time per instance,
 and the best practice is to manage one property per machine.

### **InstanceState(Expression\<Func\<TInstance, String\>\>)**

Declares the property to hold the instance's state as a string (the state name is stored in the property)

```csharp
protected internal void InstanceState(Expression<Func<TInstance, string>> instanceStateProperty)
```

#### Parameters

`instanceStateProperty` Expression\<Func\<TInstance, String\>\><br/>

### **InstanceState(Expression\<Func\<TInstance, Int32\>\>, State[])**

Declares the property to hold the instance's state as an int (0 - none, 1 = initial, 2 = final, 3... the rest)

```csharp
protected internal void InstanceState(Expression<Func<TInstance, int>> instanceStateProperty, State[] states)
```

#### Parameters

`instanceStateProperty` Expression\<Func\<TInstance, Int32\>\><br/>

`states` [State[]](../../masstransit-abstractions/masstransit/state)<br/>
Specifies the states, in order, to which the int values should be assigned

### **Name(String)**

Specifies the name of the state machine

```csharp
protected internal void Name(string machineName)
```

#### Parameters

`machineName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Event(Expression\<Func\<Event\>\>)**

Declares an event, and initializes the event property

```csharp
protected internal void Event(Expression<Func<Event>> propertyExpression)
```

#### Parameters

`propertyExpression` Expression\<Func\<Event\>\><br/>

### **Event(String)**

```csharp
protected internal Event Event(string name)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Event](../../masstransit-abstractions/masstransit/event)<br/>

### **SetCompleted(Func\<TInstance, Task\<Boolean\>\>)**

Sets the method used to determine if a state machine instance has completed. The saga repository removes completed state machine instances.

```csharp
protected void SetCompleted(Func<TInstance, Task<bool>> completed)
```

#### Parameters

`completed` [Func\<TInstance, Task\<Boolean\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **SetCompleted(Func\<BehaviorContext\<TInstance\>, Task\<Boolean\>\>)**

```csharp
protected void SetCompleted(Func<BehaviorContext<TInstance>, Task<bool>> completed)
```

#### Parameters

`completed` [Func\<BehaviorContext\<TInstance\>, Task\<Boolean\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **SetCompletedWhenFinalized()**

Sets the state machine instance to Completed when in the final state. The saga repository removes completed state machine instances.

```csharp
protected void SetCompletedWhenFinalized()
```

### **Event\<T\>(Expression\<Func\<Event\<T\>\>\>, Action\<IEventCorrelationConfigurator\<TInstance, T\>\>)**

Declares an Event on the state machine with the specified data type, and allows the correlation of the event
 to be configured.

```csharp
protected void Event<T>(Expression<Func<Event<T>>> propertyExpression, Action<IEventCorrelationConfigurator<TInstance, T>> configureEventCorrelation)
```

#### Type Parameters

`T`<br/>
The event data type

#### Parameters

`propertyExpression` Expression\<Func\<Event\<T\>\>\><br/>
The event property

`configureEventCorrelation` [Action\<IEventCorrelationConfigurator\<TInstance, T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configuration callback for the event

### **Event\<TProperty, T\>(Expression\<Func\<TProperty\>\>, Expression\<Func\<TProperty, Event\<T\>\>\>, Action\<IEventCorrelationConfigurator\<TInstance, T\>\>)**

Declares an Event on the state machine with the specified data type, and allows the correlation of the event
 to be configured.

```csharp
protected internal void Event<TProperty, T>(Expression<Func<TProperty>> propertyExpression, Expression<Func<TProperty, Event<T>>> eventPropertyExpression, Action<IEventCorrelationConfigurator<TInstance, T>> configureEventCorrelation)
```

#### Type Parameters

`TProperty`<br/>
The property type

`T`<br/>
The event data type

#### Parameters

`propertyExpression` Expression\<Func\<TProperty\>\><br/>
The containing property

`eventPropertyExpression` Expression\<Func\<TProperty, Event\<T\>\>\><br/>
The event property expression

`configureEventCorrelation` [Action\<IEventCorrelationConfigurator\<TInstance, T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configuration callback for the event

### **Event\<TProperty, T\>(Expression\<Func\<TProperty\>\>, Expression\<Func\<TProperty, Event\<T\>\>\>)**

Declares a data event on a property of the state machine, and initializes the property

```csharp
protected internal void Event<TProperty, T>(Expression<Func<TProperty>> propertyExpression, Expression<Func<TProperty, Event<T>>> eventPropertyExpression)
```

#### Type Parameters

`TProperty`<br/>

`T`<br/>

#### Parameters

`propertyExpression` Expression\<Func\<TProperty\>\><br/>
The property

`eventPropertyExpression` Expression\<Func\<TProperty, Event\<T\>\>\><br/>
The event property on the property

### **Event\<T\>(Expression\<Func\<Event\<T\>\>\>)**

Declares an event on the state machine with the specified data type, where the data type contains the
 CorrelatedBy(Guid) interface. The correlation by CorrelationId is automatically configured to the saga
 instance.

```csharp
protected internal void Event<T>(Expression<Func<Event<T>>> propertyExpression)
```

#### Type Parameters

`T`<br/>
The event data type

#### Parameters

`propertyExpression` Expression\<Func\<Event\<T\>\>\><br/>
The property to initialize

### **Event\<T\>(String)**

Declares an Event on the state machine with the specified data type, and allows the correlation of the event
 to be configured.

```csharp
protected internal Event<T> Event<T>(string name)
```

#### Type Parameters

`T`<br/>
The event data type

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The event name (must be unique)

#### Returns

[Event\<T\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **Event\<T\>(String, Action\<IEventCorrelationConfigurator\<TInstance, T\>\>)**

Declares an Event on the state machine with the specified data type, and allows the correlation of the event
 to be configured.

```csharp
protected internal Event<T> Event<T>(string name, Action<IEventCorrelationConfigurator<TInstance, T>> configure)
```

#### Type Parameters

`T`<br/>
The event data type

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The event name (must be unique)

`configure` [Action\<IEventCorrelationConfigurator\<TInstance, T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configuration callback method

#### Returns

[Event\<T\>](../../masstransit-abstractions/masstransit/event-1)<br/>

### **CompositeEvent(Expression\<Func\<Event\>\>, Expression\<Func\<TInstance, CompositeEventStatus\>\>, Event[])**

Adds a composite event to the state machine. A composite event is triggered when all
 off the required events have been raised. Note that required events cannot be in the initial
 state since it would cause extra instances of the state machine to be created

```csharp
protected internal Event CompositeEvent(Expression<Func<Event>> propertyExpression, Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression, Event[] events)
```

#### Parameters

`propertyExpression` Expression\<Func\<Event\>\><br/>
The composite event

`trackingPropertyExpression` Expression\<Func\<TInstance, CompositeEventStatus\>\><br/>
The property in the instance used to track the state of the composite event

`events` [Event[]](../../masstransit-abstractions/masstransit/event)<br/>
The events that must be raised before the composite event is raised

#### Returns

[Event](../../masstransit-abstractions/masstransit/event)<br/>

### **CompositeEvent(Expression\<Func\<Event\>\>, Expression\<Func\<TInstance, CompositeEventStatus\>\>, CompositeEventOptions, Event[])**

Adds a composite event to the state machine. A composite event is triggered when all
 off the required events have been raised. Note that required events cannot be in the initial
 state since it would cause extra instances of the state machine to be created

```csharp
protected internal Event CompositeEvent(Expression<Func<Event>> propertyExpression, Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression, CompositeEventOptions options, Event[] events)
```

#### Parameters

`propertyExpression` Expression\<Func\<Event\>\><br/>
The composite event

`trackingPropertyExpression` Expression\<Func\<TInstance, CompositeEventStatus\>\><br/>
The property in the instance used to track the state of the composite event

`options` [CompositeEventOptions](../../masstransit-abstractions/masstransit/compositeeventoptions)<br/>
Options on the composite event

`events` [Event[]](../../masstransit-abstractions/masstransit/event)<br/>
The events that must be raised before the composite event is raised

#### Returns

[Event](../../masstransit-abstractions/masstransit/event)<br/>

### **CompositeEvent(Expression\<Func\<Event\>\>, Expression\<Func\<TInstance, Int32\>\>, Event[])**

Adds a composite event to the state machine. A composite event is triggered when all
 off the required events have been raised. Note that required events cannot be in the initial
 state since it would cause extra instances of the state machine to be created

```csharp
protected internal Event CompositeEvent(Expression<Func<Event>> propertyExpression, Expression<Func<TInstance, int>> trackingPropertyExpression, Event[] events)
```

#### Parameters

`propertyExpression` Expression\<Func\<Event\>\><br/>
The composite event

`trackingPropertyExpression` Expression\<Func\<TInstance, Int32\>\><br/>
The property in the instance used to track the state of the composite event

`events` [Event[]](../../masstransit-abstractions/masstransit/event)<br/>
The events that must be raised before the composite event is raised

#### Returns

[Event](../../masstransit-abstractions/masstransit/event)<br/>

### **CompositeEvent(Expression\<Func\<Event\>\>, Expression\<Func\<TInstance, Int32\>\>, CompositeEventOptions, Event[])**

Adds a composite event to the state machine. A composite event is triggered when all
 off the required events have been raised. Note that required events cannot be in the initial
 state since it would cause extra instances of the state machine to be created

```csharp
protected internal Event CompositeEvent(Expression<Func<Event>> propertyExpression, Expression<Func<TInstance, int>> trackingPropertyExpression, CompositeEventOptions options, Event[] events)
```

#### Parameters

`propertyExpression` Expression\<Func\<Event\>\><br/>
The composite event

`trackingPropertyExpression` Expression\<Func\<TInstance, Int32\>\><br/>
The property in the instance used to track the state of the composite event

`options` [CompositeEventOptions](../../masstransit-abstractions/masstransit/compositeeventoptions)<br/>
Options on the composite event

`events` [Event[]](../../masstransit-abstractions/masstransit/event)<br/>
The events that must be raised before the composite event is raised

#### Returns

[Event](../../masstransit-abstractions/masstransit/event)<br/>

### **CompositeEvent(String, Expression\<Func\<TInstance, CompositeEventStatus\>\>, Event[])**

```csharp
internal Event CompositeEvent(string name, Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression, Event[] events)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`trackingPropertyExpression` Expression\<Func\<TInstance, CompositeEventStatus\>\><br/>

`events` [Event[]](../../masstransit-abstractions/masstransit/event)<br/>

#### Returns

[Event](../../masstransit-abstractions/masstransit/event)<br/>

### **CompositeEvent(String, Expression\<Func\<TInstance, CompositeEventStatus\>\>, CompositeEventOptions, Event[])**

```csharp
protected internal Event CompositeEvent(string name, Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression, CompositeEventOptions options, Event[] events)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`trackingPropertyExpression` Expression\<Func\<TInstance, CompositeEventStatus\>\><br/>

`options` [CompositeEventOptions](../../masstransit-abstractions/masstransit/compositeeventoptions)<br/>

`events` [Event[]](../../masstransit-abstractions/masstransit/event)<br/>

#### Returns

[Event](../../masstransit-abstractions/masstransit/event)<br/>

### **CompositeEvent(String, Expression\<Func\<TInstance, Int32\>\>, Event[])**

```csharp
internal Event CompositeEvent(string name, Expression<Func<TInstance, int>> trackingPropertyExpression, Event[] events)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`trackingPropertyExpression` Expression\<Func\<TInstance, Int32\>\><br/>

`events` [Event[]](../../masstransit-abstractions/masstransit/event)<br/>

#### Returns

[Event](../../masstransit-abstractions/masstransit/event)<br/>

### **CompositeEvent(String, Expression\<Func\<TInstance, Int32\>\>, CompositeEventOptions, Event[])**

```csharp
protected internal Event CompositeEvent(string name, Expression<Func<TInstance, int>> trackingPropertyExpression, CompositeEventOptions options, Event[] events)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`trackingPropertyExpression` Expression\<Func\<TInstance, Int32\>\><br/>

`options` [CompositeEventOptions](../../masstransit-abstractions/masstransit/compositeeventoptions)<br/>

`events` [Event[]](../../masstransit-abstractions/masstransit/event)<br/>

#### Returns

[Event](../../masstransit-abstractions/masstransit/event)<br/>

### **CompositeEvent(Event, Expression\<Func\<TInstance, CompositeEventStatus\>\>, Event[])**

```csharp
protected internal Event CompositeEvent(Event event, Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression, Event[] events)
```

#### Parameters

`event` [Event](../../masstransit-abstractions/masstransit/event)<br/>

`trackingPropertyExpression` Expression\<Func\<TInstance, CompositeEventStatus\>\><br/>

`events` [Event[]](../../masstransit-abstractions/masstransit/event)<br/>

#### Returns

[Event](../../masstransit-abstractions/masstransit/event)<br/>

### **CompositeEvent(Event, Expression\<Func\<TInstance, CompositeEventStatus\>\>, CompositeEventOptions, Event[])**

```csharp
protected internal Event CompositeEvent(Event event, Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression, CompositeEventOptions options, Event[] events)
```

#### Parameters

`event` [Event](../../masstransit-abstractions/masstransit/event)<br/>

`trackingPropertyExpression` Expression\<Func\<TInstance, CompositeEventStatus\>\><br/>

`options` [CompositeEventOptions](../../masstransit-abstractions/masstransit/compositeeventoptions)<br/>

`events` [Event[]](../../masstransit-abstractions/masstransit/event)<br/>

#### Returns

[Event](../../masstransit-abstractions/masstransit/event)<br/>

### **CompositeEvent(Event, Expression\<Func\<TInstance, Int32\>\>, Event[])**

```csharp
protected internal Event CompositeEvent(Event event, Expression<Func<TInstance, int>> trackingPropertyExpression, Event[] events)
```

#### Parameters

`event` [Event](../../masstransit-abstractions/masstransit/event)<br/>

`trackingPropertyExpression` Expression\<Func\<TInstance, Int32\>\><br/>

`events` [Event[]](../../masstransit-abstractions/masstransit/event)<br/>

#### Returns

[Event](../../masstransit-abstractions/masstransit/event)<br/>

### **CompositeEvent(Event, Expression\<Func\<TInstance, Int32\>\>, CompositeEventOptions, Event[])**

```csharp
protected internal Event CompositeEvent(Event event, Expression<Func<TInstance, int>> trackingPropertyExpression, CompositeEventOptions options, Event[] events)
```

#### Parameters

`event` [Event](../../masstransit-abstractions/masstransit/event)<br/>

`trackingPropertyExpression` Expression\<Func\<TInstance, Int32\>\><br/>

`options` [CompositeEventOptions](../../masstransit-abstractions/masstransit/compositeeventoptions)<br/>

`events` [Event[]](../../masstransit-abstractions/masstransit/event)<br/>

#### Returns

[Event](../../masstransit-abstractions/masstransit/event)<br/>

### **State(Expression\<Func\<State\>\>)**

Declares a state on the state machine, and initialized the property

```csharp
protected internal void State(Expression<Func<State>> propertyExpression)
```

#### Parameters

`propertyExpression` Expression\<Func\<State\>\><br/>
The state property

### **State(String)**

```csharp
protected internal State<TInstance> State(string name)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[State\<TInstance\>](../../masstransit-abstractions/masstransit/state-1)<br/>

### **State\<TProperty\>(Expression\<Func\<TProperty\>\>, Expression\<Func\<TProperty, State\>\>)**

Declares a state on the state machine, and initialized the property

```csharp
protected internal void State<TProperty>(Expression<Func<TProperty>> propertyExpression, Expression<Func<TProperty, State>> statePropertyExpression)
```

#### Type Parameters

`TProperty`<br/>

#### Parameters

`propertyExpression` Expression\<Func\<TProperty\>\><br/>
The property containing the state

`statePropertyExpression` Expression\<Func\<TProperty, State\>\><br/>
The state property

### **SubState(Expression\<Func\<State\>\>, State)**

Declares a sub-state on the machine. A sub-state is a state that is valid within a super-state,
 allowing a state machine to have multiple "states" -- nested parts of an overall state.

```csharp
protected internal void SubState(Expression<Func<State>> propertyExpression, State superState)
```

#### Parameters

`propertyExpression` Expression\<Func\<State\>\><br/>
The state property expression

`superState` [State](../../masstransit-abstractions/masstransit/state)<br/>
The superstate of which this state is a substate

### **SubState(String, State)**

```csharp
protected internal State<TInstance> SubState(string name, State superState)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`superState` [State](../../masstransit-abstractions/masstransit/state)<br/>

#### Returns

[State\<TInstance\>](../../masstransit-abstractions/masstransit/state-1)<br/>

### **SubState\<TProperty\>(Expression\<Func\<TProperty\>\>, Expression\<Func\<TProperty, State\>\>, State)**

Declares a state on the state machine, and initialized the property

```csharp
protected internal void SubState<TProperty>(Expression<Func<TProperty>> propertyExpression, Expression<Func<TProperty, State>> statePropertyExpression, State superState)
```

#### Type Parameters

`TProperty`<br/>

#### Parameters

`propertyExpression` Expression\<Func\<TProperty\>\><br/>
The property containing the state

`statePropertyExpression` Expression\<Func\<TProperty, State\>\><br/>
The state property

`superState` [State](../../masstransit-abstractions/masstransit/state)<br/>
The superstate of which this state is a substate

### **During(State, EventActivities`1[])**

Declares the events and associated activities that are handled during the specified state

```csharp
protected internal void During(State state, EventActivities`1[] activities)
```

#### Parameters

`state` [State](../../masstransit-abstractions/masstransit/state)<br/>
The state

`activities` [EventActivities`1[]](../masstransit/eventactivities-1)<br/>
The event and activities

### **During(State, State, EventActivities`1[])**

Declares the events and associated activities that are handled during the specified states

```csharp
protected internal void During(State state1, State state2, EventActivities`1[] activities)
```

#### Parameters

`state1` [State](../../masstransit-abstractions/masstransit/state)<br/>
The state

`state2` [State](../../masstransit-abstractions/masstransit/state)<br/>
The other state

`activities` [EventActivities`1[]](../masstransit/eventactivities-1)<br/>
The event and activities

### **During(State, State, State, EventActivities`1[])**

Declares the events and associated activities that are handled during the specified states

```csharp
protected internal void During(State state1, State state2, State state3, EventActivities`1[] activities)
```

#### Parameters

`state1` [State](../../masstransit-abstractions/masstransit/state)<br/>
The state

`state2` [State](../../masstransit-abstractions/masstransit/state)<br/>
The other state

`state3` [State](../../masstransit-abstractions/masstransit/state)<br/>
The other other state

`activities` [EventActivities`1[]](../masstransit/eventactivities-1)<br/>
The event and activities

### **During(State, State, State, State, EventActivities`1[])**

Declares the events and associated activities that are handled during the specified states

```csharp
protected internal void During(State state1, State state2, State state3, State state4, EventActivities`1[] activities)
```

#### Parameters

`state1` [State](../../masstransit-abstractions/masstransit/state)<br/>
The state

`state2` [State](../../masstransit-abstractions/masstransit/state)<br/>
The other state

`state3` [State](../../masstransit-abstractions/masstransit/state)<br/>
The other other state

`state4` [State](../../masstransit-abstractions/masstransit/state)<br/>
Okay, this is getting a bit ridiculous at this point

`activities` [EventActivities`1[]](../masstransit/eventactivities-1)<br/>
The event and activities

### **During(IEnumerable\<State\>, EventActivities`1[])**

Declares the events and associated activities that are handled during the specified states

```csharp
protected internal void During(IEnumerable<State> states, EventActivities`1[] activities)
```

#### Parameters

`states` [IEnumerable\<State\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
The states

`activities` [EventActivities`1[]](../masstransit/eventactivities-1)<br/>
The event and activities

### **Initially(EventActivities`1[])**

Declares the events and activities that are handled during the initial state

```csharp
protected internal void Initially(EventActivities`1[] activities)
```

#### Parameters

`activities` [EventActivities`1[]](../masstransit/eventactivities-1)<br/>
The event and activities

### **DuringAny(EventActivities`1[])**

Declares events and activities that are handled during any state except the Initial and Final

```csharp
protected internal void DuringAny(EventActivities`1[] activities)
```

#### Parameters

`activities` [EventActivities`1[]](../masstransit/eventactivities-1)<br/>
The event and activities

### **Finally(Func\<EventActivityBinder\<TInstance\>, EventActivityBinder\<TInstance\>\>)**

When the Final state is entered, execute the chained activities. This occurs in any state that is not the initial or final state

```csharp
protected internal void Finally(Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
```

#### Parameters

`activityCallback` [Func\<EventActivityBinder\<TInstance\>, EventActivityBinder\<TInstance\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
Specify the activities that are executes when the Final state is entered.

### **When(Event)**

When the event is fired in this state, execute the chained activities

```csharp
protected internal EventActivityBinder<TInstance> When(Event event)
```

#### Parameters

`event` [Event](../../masstransit-abstractions/masstransit/event)<br/>
The fired event

#### Returns

[EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

### **When(Event, StateMachineCondition\<TInstance\>)**

When the event is fired in this state, and the event data matches the filter expression, execute the chained activities

```csharp
protected internal EventActivityBinder<TInstance> When(Event event, StateMachineCondition<TInstance> filter)
```

#### Parameters

`event` [Event](../../masstransit-abstractions/masstransit/event)<br/>
The fired event

`filter` [StateMachineCondition\<TInstance\>](../../masstransit-abstractions/masstransit/statemachinecondition-1)<br/>
The filter applied to the event

#### Returns

[EventActivityBinder\<TInstance\>](../masstransit/eventactivitybinder-1)<br/>

### **WhenEnter(State, Func\<EventActivityBinder\<TInstance\>, EventActivityBinder\<TInstance\>\>)**

When entering the specified state

```csharp
protected internal void WhenEnter(State state, Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
```

#### Parameters

`state` [State](../../masstransit-abstractions/masstransit/state)<br/>

`activityCallback` [Func\<EventActivityBinder\<TInstance\>, EventActivityBinder\<TInstance\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **WhenEnterAny(Func\<EventActivityBinder\<TInstance\>, EventActivityBinder\<TInstance\>\>)**

When entering any state

```csharp
protected internal void WhenEnterAny(Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
```

#### Parameters

`activityCallback` [Func\<EventActivityBinder\<TInstance\>, EventActivityBinder\<TInstance\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **WhenLeaveAny(Func\<EventActivityBinder\<TInstance\>, EventActivityBinder\<TInstance\>\>)**

When leaving any state

```csharp
protected internal void WhenLeaveAny(Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
```

#### Parameters

`activityCallback` [Func\<EventActivityBinder\<TInstance\>, EventActivityBinder\<TInstance\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **BeforeEnterAny(Func\<EventActivityBinder\<TInstance, State\>, EventActivityBinder\<TInstance, State\>\>)**

Before entering any state

```csharp
protected internal void BeforeEnterAny(Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback)
```

#### Parameters

`activityCallback` [Func\<EventActivityBinder\<TInstance, State\>, EventActivityBinder\<TInstance, State\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **AfterLeaveAny(Func\<EventActivityBinder\<TInstance, State\>, EventActivityBinder\<TInstance, State\>\>)**

After leaving any state

```csharp
protected internal void AfterLeaveAny(Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback)
```

#### Parameters

`activityCallback` [Func\<EventActivityBinder\<TInstance, State\>, EventActivityBinder\<TInstance, State\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **WhenLeave(State, Func\<EventActivityBinder\<TInstance\>, EventActivityBinder\<TInstance\>\>)**

When leaving the specified state

```csharp
protected internal void WhenLeave(State state, Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
```

#### Parameters

`state` [State](../../masstransit-abstractions/masstransit/state)<br/>

`activityCallback` [Func\<EventActivityBinder\<TInstance\>, EventActivityBinder\<TInstance\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **BeforeEnter(State, Func\<EventActivityBinder\<TInstance, State\>, EventActivityBinder\<TInstance, State\>\>)**

Before entering the specified state

```csharp
protected internal void BeforeEnter(State state, Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback)
```

#### Parameters

`state` [State](../../masstransit-abstractions/masstransit/state)<br/>

`activityCallback` [Func\<EventActivityBinder\<TInstance, State\>, EventActivityBinder\<TInstance, State\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **AfterLeave(State, Func\<EventActivityBinder\<TInstance, State\>, EventActivityBinder\<TInstance, State\>\>)**

After leaving the specified state

```csharp
protected internal void AfterLeave(State state, Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback)
```

#### Parameters

`state` [State](../../masstransit-abstractions/masstransit/state)<br/>

`activityCallback` [Func\<EventActivityBinder\<TInstance, State\>, EventActivityBinder\<TInstance, State\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **When\<TMessage\>(Event\<TMessage\>)**

When the event is fired in this state, execute the chained activities

```csharp
protected internal EventActivityBinder<TInstance, TMessage> When<TMessage>(Event<TMessage> event)
```

#### Type Parameters

`TMessage`<br/>
The event data type

#### Parameters

`event` [Event\<TMessage\>](../../masstransit-abstractions/masstransit/event-1)<br/>
The fired event

#### Returns

[EventActivityBinder\<TInstance, TMessage\>](../masstransit/eventactivitybinder-2)<br/>

### **When\<TMessage\>(Event\<TMessage\>, StateMachineCondition\<TInstance, TMessage\>)**

When the event is fired in this state, and the event data matches the filter expression, execute the chained activities

```csharp
protected internal EventActivityBinder<TInstance, TMessage> When<TMessage>(Event<TMessage> event, StateMachineCondition<TInstance, TMessage> filter)
```

#### Type Parameters

`TMessage`<br/>
The event data type

#### Parameters

`event` [Event\<TMessage\>](../../masstransit-abstractions/masstransit/event-1)<br/>
The fired event

`filter` [StateMachineCondition\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/statemachinecondition-2)<br/>
The filter applied to the event

#### Returns

[EventActivityBinder\<TInstance, TMessage\>](../masstransit/eventactivitybinder-2)<br/>

### **Ignore(Event)**

Ignore the event in this state (no exception is thrown)

```csharp
protected internal EventActivities<TInstance> Ignore(Event event)
```

#### Parameters

`event` [Event](../../masstransit-abstractions/masstransit/event)<br/>
The ignored event

#### Returns

[EventActivities\<TInstance\>](../masstransit/eventactivities-1)<br/>

### **Ignore\<TData\>(Event\<TData\>)**

Ignore the event in this state (no exception is thrown)

```csharp
protected internal EventActivities<TInstance> Ignore<TData>(Event<TData> event)
```

#### Type Parameters

`TData`<br/>
The event data type

#### Parameters

`event` [Event\<TData\>](../../masstransit-abstractions/masstransit/event-1)<br/>
The ignored event

#### Returns

[EventActivities\<TInstance\>](../masstransit/eventactivities-1)<br/>

### **Ignore\<TData\>(Event\<TData\>, StateMachineCondition\<TInstance, TData\>)**

Ignore the event in this state (no exception is thrown)

```csharp
protected internal EventActivities<TInstance> Ignore<TData>(Event<TData> event, StateMachineCondition<TInstance, TData> filter)
```

#### Type Parameters

`TData`<br/>
The event data type

#### Parameters

`event` [Event\<TData\>](../../masstransit-abstractions/masstransit/event-1)<br/>
The ignored event

`filter` [StateMachineCondition\<TInstance, TData\>](../../masstransit-abstractions/masstransit/statemachinecondition-2)<br/>
The filter to apply to the event data

#### Returns

[EventActivities\<TInstance\>](../masstransit/eventactivities-1)<br/>

### **OnUnhandledEvent(UnhandledEventCallback\<TInstance\>)**

Specifies a callback to invoke when an event is raised in a state where the event is not handled

```csharp
protected internal void OnUnhandledEvent(UnhandledEventCallback<TInstance> callback)
```

#### Parameters

`callback` [UnhandledEventCallback\<TInstance\>](../../masstransit-abstractions/masstransit/unhandledeventcallback-1)<br/>
The unhandled event callback

### **Request\<TRequest, TResponse\>(Expression\<Func\<Request\<TInstance, TRequest, TResponse\>\>\>, Expression\<Func\<TInstance, Nullable\<Guid\>\>\>, Action\<IRequestConfigurator\<TInstance, TRequest, TResponse\>\>)**

Declares a request that is sent by the state machine to a service, and the associated response, fault, and
 timeout handling. The property is initialized with the fully built Request. The request must be declared before
 it is used in the state/event declaration statements.

```csharp
protected void Request<TRequest, TResponse>(Expression<Func<Request<TInstance, TRequest, TResponse>>> propertyExpression, Expression<Func<TInstance, Nullable<Guid>>> requestIdExpression, Action<IRequestConfigurator<TInstance, TRequest, TResponse>> configureRequest)
```

#### Type Parameters

`TRequest`<br/>
The request type

`TResponse`<br/>
The response type

#### Parameters

`propertyExpression` Expression\<Func\<Request\<TInstance, TRequest, TResponse\>\>\><br/>
The request property on the state machine

`requestIdExpression` Expression\<Func\<TInstance, Nullable\<Guid\>\>\><br/>
The property where the requestId is stored

`configureRequest` [Action\<IRequestConfigurator\<TInstance, TRequest, TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Allow the request settings to be specified inline

### **Request\<TRequest, TResponse\>(Expression\<Func\<Request\<TInstance, TRequest, TResponse\>\>\>, Action\<IRequestConfigurator\<TInstance, TRequest, TResponse\>\>)**

Declares a request that is sent by the state machine to a service, and the associated response, fault, and
 timeout handling. The property is initialized with the fully built Request. The request must be declared before
 it is used in the state/event declaration statements.
 Uses the Saga CorrelationId as the RequestId

```csharp
protected void Request<TRequest, TResponse>(Expression<Func<Request<TInstance, TRequest, TResponse>>> propertyExpression, Action<IRequestConfigurator<TInstance, TRequest, TResponse>> configureRequest)
```

#### Type Parameters

`TRequest`<br/>
The request type

`TResponse`<br/>
The response type

#### Parameters

`propertyExpression` Expression\<Func\<Request\<TInstance, TRequest, TResponse\>\>\><br/>
The request property on the state machine

`configureRequest` [Action\<IRequestConfigurator\<TInstance, TRequest, TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Allow the request settings to be specified inline

### **Request\<TRequest, TResponse\>(Expression\<Func\<Request\<TInstance, TRequest, TResponse\>\>\>, Expression\<Func\<TInstance, Nullable\<Guid\>\>\>, RequestSettings\<TInstance, TRequest, TResponse\>)**

Declares a request that is sent by the state machine to a service, and the associated response, fault, and
 timeout handling. The property is initialized with the fully built Request. The request must be declared before
 it is used in the state/event declaration statements.

```csharp
protected void Request<TRequest, TResponse>(Expression<Func<Request<TInstance, TRequest, TResponse>>> propertyExpression, Expression<Func<TInstance, Nullable<Guid>>> requestIdExpression, RequestSettings<TInstance, TRequest, TResponse> settings)
```

#### Type Parameters

`TRequest`<br/>
The request type

`TResponse`<br/>
The response type

#### Parameters

`propertyExpression` Expression\<Func\<Request\<TInstance, TRequest, TResponse\>\>\><br/>
The request property on the state machine

`requestIdExpression` Expression\<Func\<TInstance, Nullable\<Guid\>\>\><br/>
The property where the requestId is stored

`settings` [RequestSettings\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/requestsettings-3)<br/>
The request settings (which can be read from configuration, etc.)

### **Request\<TRequest, TResponse\>(Expression\<Func\<Request\<TInstance, TRequest, TResponse\>\>\>, RequestSettings\<TInstance, TRequest, TResponse\>)**

Declares a request that is sent by the state machine to a service, and the associated response, fault, and
 timeout handling. The property is initialized with the fully built Request. The request must be declared before
 it is used in the state/event declaration statements.
 Uses the Saga CorrelationId as the RequestId

```csharp
protected internal void Request<TRequest, TResponse>(Expression<Func<Request<TInstance, TRequest, TResponse>>> propertyExpression, RequestSettings<TInstance, TRequest, TResponse> settings)
```

#### Type Parameters

`TRequest`<br/>
The request type

`TResponse`<br/>
The response type

#### Parameters

`propertyExpression` Expression\<Func\<Request\<TInstance, TRequest, TResponse\>\>\><br/>
The request property on the state machine

`settings` [RequestSettings\<TInstance, TRequest, TResponse\>](../../masstransit-abstractions/masstransit/requestsettings-3)<br/>
The request settings (which can be read from configuration, etc.)

### **Request\<TRequest, TResponse, TResponse2\>(Expression\<Func\<Request\<TInstance, TRequest, TResponse, TResponse2\>\>\>, Expression\<Func\<TInstance, Nullable\<Guid\>\>\>, Action\<IRequestConfigurator\<TInstance, TRequest, TResponse, TResponse2\>\>)**

Declares a request that is sent by the state machine to a service, and the associated response, fault, and
 timeout handling. The property is initialized with the fully built Request. The request must be declared before
 it is used in the state/event declaration statements.

```csharp
protected void Request<TRequest, TResponse, TResponse2>(Expression<Func<Request<TInstance, TRequest, TResponse, TResponse2>>> propertyExpression, Expression<Func<TInstance, Nullable<Guid>>> requestIdExpression, Action<IRequestConfigurator<TInstance, TRequest, TResponse, TResponse2>> configureRequest)
```

#### Type Parameters

`TRequest`<br/>
The request type

`TResponse`<br/>
The response type

`TResponse2`<br/>
The alternate response type

#### Parameters

`propertyExpression` Expression\<Func\<Request\<TInstance, TRequest, TResponse, TResponse2\>\>\><br/>
The request property on the state machine

`requestIdExpression` Expression\<Func\<TInstance, Nullable\<Guid\>\>\><br/>
The property where the requestId is stored

`configureRequest` [Action\<IRequestConfigurator\<TInstance, TRequest, TResponse, TResponse2\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Allow the request settings to be specified inline

### **Request\<TRequest, TResponse, TResponse2\>(Expression\<Func\<Request\<TInstance, TRequest, TResponse, TResponse2\>\>\>, Action\<IRequestConfigurator\<TInstance, TRequest, TResponse, TResponse2\>\>)**

Declares a request that is sent by the state machine to a service, and the associated response, fault, and
 timeout handling. The property is initialized with the fully built Request. The request must be declared before
 it is used in the state/event declaration statements.
 Uses the Saga CorrelationId as the RequestId

```csharp
protected void Request<TRequest, TResponse, TResponse2>(Expression<Func<Request<TInstance, TRequest, TResponse, TResponse2>>> propertyExpression, Action<IRequestConfigurator<TInstance, TRequest, TResponse, TResponse2>> configureRequest)
```

#### Type Parameters

`TRequest`<br/>
The request type

`TResponse`<br/>
The response type

`TResponse2`<br/>
The alternate response type

#### Parameters

`propertyExpression` Expression\<Func\<Request\<TInstance, TRequest, TResponse, TResponse2\>\>\><br/>
The request property on the state machine

`configureRequest` [Action\<IRequestConfigurator\<TInstance, TRequest, TResponse, TResponse2\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Allow the request settings to be specified inline

### **Request\<TRequest, TResponse, TResponse2\>(Expression\<Func\<Request\<TInstance, TRequest, TResponse, TResponse2\>\>\>, Expression\<Func\<TInstance, Nullable\<Guid\>\>\>, RequestSettings\<TInstance, TRequest, TResponse, TResponse2\>)**

Declares a request that is sent by the state machine to a service, and the associated response, fault, and
 timeout handling. The property is initialized with the fully built Request. The request must be declared before
 it is used in the state/event declaration statements.

```csharp
protected internal void Request<TRequest, TResponse, TResponse2>(Expression<Func<Request<TInstance, TRequest, TResponse, TResponse2>>> propertyExpression, Expression<Func<TInstance, Nullable<Guid>>> requestIdExpression, RequestSettings<TInstance, TRequest, TResponse, TResponse2> settings)
```

#### Type Parameters

`TRequest`<br/>
The request type

`TResponse`<br/>
The response type

`TResponse2`<br/>
The alternate response type

#### Parameters

`propertyExpression` Expression\<Func\<Request\<TInstance, TRequest, TResponse, TResponse2\>\>\><br/>
The request property on the state machine

`requestIdExpression` Expression\<Func\<TInstance, Nullable\<Guid\>\>\><br/>
The property where the requestId is stored

`settings` [RequestSettings\<TInstance, TRequest, TResponse, TResponse2\>](../../masstransit-abstractions/masstransit/requestsettings-4)<br/>
The request settings (which can be read from configuration, etc.)

### **Request\<TRequest, TResponse, TResponse2\>(Expression\<Func\<Request\<TInstance, TRequest, TResponse, TResponse2\>\>\>, RequestSettings\<TInstance, TRequest, TResponse, TResponse2\>)**

Declares a request that is sent by the state machine to a service, and the associated response, fault, and
 timeout handling. The property is initialized with the fully built Request. The request must be declared before
 it is used in the state/event declaration statements.
 Uses the Saga CorrelationId as the RequestId

```csharp
protected internal void Request<TRequest, TResponse, TResponse2>(Expression<Func<Request<TInstance, TRequest, TResponse, TResponse2>>> propertyExpression, RequestSettings<TInstance, TRequest, TResponse, TResponse2> settings)
```

#### Type Parameters

`TRequest`<br/>
The request type

`TResponse`<br/>
The response type

`TResponse2`<br/>
The alternate response type

#### Parameters

`propertyExpression` Expression\<Func\<Request\<TInstance, TRequest, TResponse, TResponse2\>\>\><br/>
The request property on the state machine

`settings` [RequestSettings\<TInstance, TRequest, TResponse, TResponse2\>](../../masstransit-abstractions/masstransit/requestsettings-4)<br/>
The request settings (which can be read from configuration, etc.)

### **Request\<TRequest, TResponse, TResponse2, TResponse3\>(Expression\<Func\<Request\<TInstance, TRequest, TResponse, TResponse2, TResponse3\>\>\>, Expression\<Func\<TInstance, Nullable\<Guid\>\>\>, Action\<IRequestConfigurator\<TInstance, TRequest, TResponse, TResponse2, TResponse3\>\>)**

Declares a request that is sent by the state machine to a service, and the associated response, fault, and
 timeout handling. The property is initialized with the fully built Request. The request must be declared before
 it is used in the state/event declaration statements.

```csharp
protected void Request<TRequest, TResponse, TResponse2, TResponse3>(Expression<Func<Request<TInstance, TRequest, TResponse, TResponse2, TResponse3>>> propertyExpression, Expression<Func<TInstance, Nullable<Guid>>> requestIdExpression, Action<IRequestConfigurator<TInstance, TRequest, TResponse, TResponse2, TResponse3>> configureRequest)
```

#### Type Parameters

`TRequest`<br/>
The request type

`TResponse`<br/>
The response type

`TResponse2`<br/>
The alternate response type

`TResponse3`<br/>

#### Parameters

`propertyExpression` Expression\<Func\<Request\<TInstance, TRequest, TResponse, TResponse2, TResponse3\>\>\><br/>
The request property on the state machine

`requestIdExpression` Expression\<Func\<TInstance, Nullable\<Guid\>\>\><br/>
The property where the requestId is stored

`configureRequest` [Action\<IRequestConfigurator\<TInstance, TRequest, TResponse, TResponse2, TResponse3\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Allow the request settings to be specified inline

### **Request\<TRequest, TResponse, TResponse2, TResponse3\>(Expression\<Func\<Request\<TInstance, TRequest, TResponse, TResponse2, TResponse3\>\>\>, Action\<IRequestConfigurator\<TInstance, TRequest, TResponse, TResponse2, TResponse3\>\>)**

Declares a request that is sent by the state machine to a service, and the associated response, fault, and
 timeout handling. The property is initialized with the fully built Request. The request must be declared before
 it is used in the state/event declaration statements.
 Uses the Saga CorrelationId as the RequestId

```csharp
protected void Request<TRequest, TResponse, TResponse2, TResponse3>(Expression<Func<Request<TInstance, TRequest, TResponse, TResponse2, TResponse3>>> propertyExpression, Action<IRequestConfigurator<TInstance, TRequest, TResponse, TResponse2, TResponse3>> configureRequest)
```

#### Type Parameters

`TRequest`<br/>
The request type

`TResponse`<br/>
The response type

`TResponse2`<br/>
The alternate response type

`TResponse3`<br/>

#### Parameters

`propertyExpression` Expression\<Func\<Request\<TInstance, TRequest, TResponse, TResponse2, TResponse3\>\>\><br/>
The request property on the state machine

`configureRequest` [Action\<IRequestConfigurator\<TInstance, TRequest, TResponse, TResponse2, TResponse3\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Allow the request settings to be specified inline

### **Request\<TRequest, TResponse, TResponse2, TResponse3\>(Expression\<Func\<Request\<TInstance, TRequest, TResponse, TResponse2, TResponse3\>\>\>, Expression\<Func\<TInstance, Nullable\<Guid\>\>\>, RequestSettings\<TInstance, TRequest, TResponse, TResponse2, TResponse3\>)**

Declares a request that is sent by the state machine to a service, and the associated response, fault, and
 timeout handling. The property is initialized with the fully built Request. The request must be declared before
 it is used in the state/event declaration statements.

```csharp
protected internal void Request<TRequest, TResponse, TResponse2, TResponse3>(Expression<Func<Request<TInstance, TRequest, TResponse, TResponse2, TResponse3>>> propertyExpression, Expression<Func<TInstance, Nullable<Guid>>> requestIdExpression, RequestSettings<TInstance, TRequest, TResponse, TResponse2, TResponse3> settings)
```

#### Type Parameters

`TRequest`<br/>
The request type

`TResponse`<br/>
The response type

`TResponse2`<br/>
The alternate response type

`TResponse3`<br/>

#### Parameters

`propertyExpression` Expression\<Func\<Request\<TInstance, TRequest, TResponse, TResponse2, TResponse3\>\>\><br/>
The request property on the state machine

`requestIdExpression` Expression\<Func\<TInstance, Nullable\<Guid\>\>\><br/>
The property where the requestId is stored

`settings` [RequestSettings\<TInstance, TRequest, TResponse, TResponse2, TResponse3\>](../../masstransit-abstractions/masstransit/requestsettings-5)<br/>
The request settings (which can be read from configuration, etc.)

### **Request\<TRequest, TResponse, TResponse2, TResponse3\>(Expression\<Func\<Request\<TInstance, TRequest, TResponse, TResponse2, TResponse3\>\>\>, RequestSettings\<TInstance, TRequest, TResponse, TResponse2, TResponse3\>)**

Declares a request that is sent by the state machine to a service, and the associated response, fault, and
 timeout handling. The property is initialized with the fully built Request. The request must be declared before
 it is used in the state/event declaration statements.
 Uses the Saga CorrelationId as the RequestId

```csharp
protected internal void Request<TRequest, TResponse, TResponse2, TResponse3>(Expression<Func<Request<TInstance, TRequest, TResponse, TResponse2, TResponse3>>> propertyExpression, RequestSettings<TInstance, TRequest, TResponse, TResponse2, TResponse3> settings)
```

#### Type Parameters

`TRequest`<br/>
The request type

`TResponse`<br/>
The response type

`TResponse2`<br/>
The alternate response type

`TResponse3`<br/>

#### Parameters

`propertyExpression` Expression\<Func\<Request\<TInstance, TRequest, TResponse, TResponse2, TResponse3\>\>\><br/>
The request property on the state machine

`settings` [RequestSettings\<TInstance, TRequest, TResponse, TResponse2, TResponse3\>](../../masstransit-abstractions/masstransit/requestsettings-5)<br/>
The request settings (which can be read from configuration, etc.)

### **Schedule\<TMessage\>(Expression\<Func\<Schedule\<TInstance, TMessage\>\>\>, Expression\<Func\<TInstance, Nullable\<Guid\>\>\>, Action\<IScheduleConfigurator\<TInstance, TMessage\>\>)**

Declares a schedule placeholder that is stored with the state machine instance

```csharp
protected void Schedule<TMessage>(Expression<Func<Schedule<TInstance, TMessage>>> propertyExpression, Expression<Func<TInstance, Nullable<Guid>>> tokenIdExpression, Action<IScheduleConfigurator<TInstance, TMessage>> configureSchedule)
```

#### Type Parameters

`TMessage`<br/>
The request type

#### Parameters

`propertyExpression` Expression\<Func\<Schedule\<TInstance, TMessage\>\>\><br/>
The schedule property on the state machine

`tokenIdExpression` Expression\<Func\<TInstance, Nullable\<Guid\>\>\><br/>
The property where the tokenId is stored

`configureSchedule` [Action\<IScheduleConfigurator\<TInstance, TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback to configure the schedule

### **Schedule\<TMessage\>(Expression\<Func\<Schedule\<TInstance, TMessage\>\>\>, Expression\<Func\<TInstance, Nullable\<Guid\>\>\>, ScheduleSettings\<TInstance, TMessage\>)**

Declares a schedule placeholder that is stored with the state machine instance

```csharp
protected internal void Schedule<TMessage>(Expression<Func<Schedule<TInstance, TMessage>>> propertyExpression, Expression<Func<TInstance, Nullable<Guid>>> tokenIdExpression, ScheduleSettings<TInstance, TMessage> settings)
```

#### Type Parameters

`TMessage`<br/>
The scheduled message type

#### Parameters

`propertyExpression` Expression\<Func\<Schedule\<TInstance, TMessage\>\>\><br/>
The schedule property on the state machine

`tokenIdExpression` Expression\<Func\<TInstance, Nullable\<Guid\>\>\><br/>
The property where the tokenId is stored

`settings` [ScheduleSettings\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/schedulesettings-2)<br/>
The request settings (which can be read from configuration, etc.)

### **New(Action\<IStateMachineModifier\<TInstance\>\>)**

Create a new state machine using the builder pattern

```csharp
public static MassTransitStateMachine<TInstance> New(Action<IStateMachineModifier<TInstance>> modifier)
```

#### Parameters

`modifier` [Action\<IStateMachineModifier\<TInstance\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[MassTransitStateMachine\<TInstance\>](../masstransit/masstransitstatemachine-1)<br/>
