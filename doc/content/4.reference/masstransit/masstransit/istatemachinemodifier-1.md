---

title: IStateMachineModifier<TSaga>

---

# IStateMachineModifier\<TSaga\>

Namespace: MassTransit

```csharp
public interface IStateMachineModifier<TSaga>
```

#### Type Parameters

`TSaga`<br/>

## Properties

### **Initial**

```csharp
public abstract State Initial { get; }
```

#### Property Value

[State](../../masstransit-abstractions/masstransit/state)<br/>

### **Final**

```csharp
public abstract State Final { get; }
```

#### Property Value

[State](../../masstransit-abstractions/masstransit/state)<br/>

## Methods

### **InstanceState(Expression\<Func\<TSaga, State\>\>)**

```csharp
IStateMachineModifier<TSaga> InstanceState(Expression<Func<TSaga, State>> instanceStateProperty)
```

#### Parameters

`instanceStateProperty` Expression\<Func\<TSaga, State\>\><br/>

#### Returns

[IStateMachineModifier\<TSaga\>](../masstransit/istatemachinemodifier-1)<br/>

### **InstanceState(Expression\<Func\<TSaga, String\>\>)**

```csharp
IStateMachineModifier<TSaga> InstanceState(Expression<Func<TSaga, string>> instanceStateProperty)
```

#### Parameters

`instanceStateProperty` Expression\<Func\<TSaga, String\>\><br/>

#### Returns

[IStateMachineModifier\<TSaga\>](../masstransit/istatemachinemodifier-1)<br/>

### **InstanceState(Expression\<Func\<TSaga, Int32\>\>, State[])**

```csharp
IStateMachineModifier<TSaga> InstanceState(Expression<Func<TSaga, int>> instanceStateProperty, State[] states)
```

#### Parameters

`instanceStateProperty` Expression\<Func\<TSaga, Int32\>\><br/>

`states` [State[]](../../masstransit-abstractions/masstransit/state)<br/>

#### Returns

[IStateMachineModifier\<TSaga\>](../masstransit/istatemachinemodifier-1)<br/>

### **Name(String)**

```csharp
IStateMachineModifier<TSaga> Name(string machineName)
```

#### Parameters

`machineName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[IStateMachineModifier\<TSaga\>](../masstransit/istatemachinemodifier-1)<br/>

### **Event(String, Event)**

```csharp
IStateMachineModifier<TSaga> Event(string name, out Event event)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`event` [Event](../../masstransit-abstractions/masstransit/event)<br/>

#### Returns

[IStateMachineModifier\<TSaga\>](../masstransit/istatemachinemodifier-1)<br/>

### **Event\<T\>(String, Event\<T\>)**

```csharp
IStateMachineModifier<TSaga> Event<T>(string name, out Event<T> event)
```

#### Type Parameters

`T`<br/>

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`event` [Event\<T\>](../../masstransit-abstractions/masstransit/event-1)<br/>

#### Returns

[IStateMachineModifier\<TSaga\>](../masstransit/istatemachinemodifier-1)<br/>

### **Event\<T\>(String, Action\<IEventCorrelationConfigurator\<TSaga, T\>\>, Event\<T\>)**

```csharp
IStateMachineModifier<TSaga> Event<T>(string name, Action<IEventCorrelationConfigurator<TSaga, T>> configure, out Event<T> event)
```

#### Type Parameters

`T`<br/>

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`configure` [Action\<IEventCorrelationConfigurator\<TSaga, T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

`event` [Event\<T\>](../../masstransit-abstractions/masstransit/event-1)<br/>

#### Returns

[IStateMachineModifier\<TSaga\>](../masstransit/istatemachinemodifier-1)<br/>

### **Event\<TProperty, T\>(Expression\<Func\<TProperty\>\>, Expression\<Func\<TProperty, Event\<T\>\>\>)**

```csharp
IStateMachineModifier<TSaga> Event<TProperty, T>(Expression<Func<TProperty>> propertyExpression, Expression<Func<TProperty, Event<T>>> eventPropertyExpression)
```

#### Type Parameters

`TProperty`<br/>

`T`<br/>

#### Parameters

`propertyExpression` Expression\<Func\<TProperty\>\><br/>

`eventPropertyExpression` Expression\<Func\<TProperty, Event\<T\>\>\><br/>

#### Returns

[IStateMachineModifier\<TSaga\>](../masstransit/istatemachinemodifier-1)<br/>

### **CompositeEvent(String, Event, Expression\<Func\<TSaga, CompositeEventStatus\>\>, Event[])**

```csharp
IStateMachineModifier<TSaga> CompositeEvent(string name, out Event event, Expression<Func<TSaga, CompositeEventStatus>> trackingPropertyExpression, Event[] events)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`event` [Event](../../masstransit-abstractions/masstransit/event)<br/>

`trackingPropertyExpression` Expression\<Func\<TSaga, CompositeEventStatus\>\><br/>

`events` [Event[]](../../masstransit-abstractions/masstransit/event)<br/>

#### Returns

[IStateMachineModifier\<TSaga\>](../masstransit/istatemachinemodifier-1)<br/>

### **CompositeEvent(String, Event, Expression\<Func\<TSaga, CompositeEventStatus\>\>, CompositeEventOptions, Event[])**

```csharp
IStateMachineModifier<TSaga> CompositeEvent(string name, out Event event, Expression<Func<TSaga, CompositeEventStatus>> trackingPropertyExpression, CompositeEventOptions options, Event[] events)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`event` [Event](../../masstransit-abstractions/masstransit/event)<br/>

`trackingPropertyExpression` Expression\<Func\<TSaga, CompositeEventStatus\>\><br/>

`options` [CompositeEventOptions](../../masstransit-abstractions/masstransit/compositeeventoptions)<br/>

`events` [Event[]](../../masstransit-abstractions/masstransit/event)<br/>

#### Returns

[IStateMachineModifier\<TSaga\>](../masstransit/istatemachinemodifier-1)<br/>

### **CompositeEvent(String, Event, Expression\<Func\<TSaga, Int32\>\>, Event[])**

```csharp
IStateMachineModifier<TSaga> CompositeEvent(string name, out Event event, Expression<Func<TSaga, int>> trackingPropertyExpression, Event[] events)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`event` [Event](../../masstransit-abstractions/masstransit/event)<br/>

`trackingPropertyExpression` Expression\<Func\<TSaga, Int32\>\><br/>

`events` [Event[]](../../masstransit-abstractions/masstransit/event)<br/>

#### Returns

[IStateMachineModifier\<TSaga\>](../masstransit/istatemachinemodifier-1)<br/>

### **CompositeEvent(String, Event, Expression\<Func\<TSaga, Int32\>\>, CompositeEventOptions, Event[])**

```csharp
IStateMachineModifier<TSaga> CompositeEvent(string name, out Event event, Expression<Func<TSaga, int>> trackingPropertyExpression, CompositeEventOptions options, Event[] events)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`event` [Event](../../masstransit-abstractions/masstransit/event)<br/>

`trackingPropertyExpression` Expression\<Func\<TSaga, Int32\>\><br/>

`options` [CompositeEventOptions](../../masstransit-abstractions/masstransit/compositeeventoptions)<br/>

`events` [Event[]](../../masstransit-abstractions/masstransit/event)<br/>

#### Returns

[IStateMachineModifier\<TSaga\>](../masstransit/istatemachinemodifier-1)<br/>

### **State(String, State\<TSaga\>)**

```csharp
IStateMachineModifier<TSaga> State(string name, out State<TSaga> state)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`state` [State\<TSaga\>](../../masstransit-abstractions/masstransit/state-1)<br/>

#### Returns

[IStateMachineModifier\<TSaga\>](../masstransit/istatemachinemodifier-1)<br/>

### **State(String, State)**

```csharp
IStateMachineModifier<TSaga> State(string name, out State state)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`state` [State](../../masstransit-abstractions/masstransit/state)<br/>

#### Returns

[IStateMachineModifier\<TSaga\>](../masstransit/istatemachinemodifier-1)<br/>

### **State\<TProperty\>(Expression\<Func\<TProperty\>\>, Expression\<Func\<TProperty, State\>\>)**

```csharp
IStateMachineModifier<TSaga> State<TProperty>(Expression<Func<TProperty>> propertyExpression, Expression<Func<TProperty, State>> statePropertyExpression)
```

#### Type Parameters

`TProperty`<br/>

#### Parameters

`propertyExpression` Expression\<Func\<TProperty\>\><br/>

`statePropertyExpression` Expression\<Func\<TProperty, State\>\><br/>

#### Returns

[IStateMachineModifier\<TSaga\>](../masstransit/istatemachinemodifier-1)<br/>

### **SubState(String, State, State\<TSaga\>)**

```csharp
IStateMachineModifier<TSaga> SubState(string name, State superState, out State<TSaga> subState)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`superState` [State](../../masstransit-abstractions/masstransit/state)<br/>

`subState` [State\<TSaga\>](../../masstransit-abstractions/masstransit/state-1)<br/>

#### Returns

[IStateMachineModifier\<TSaga\>](../masstransit/istatemachinemodifier-1)<br/>

### **SubState\<TProperty\>(Expression\<Func\<TProperty\>\>, Expression\<Func\<TProperty, State\>\>, State)**

```csharp
IStateMachineModifier<TSaga> SubState<TProperty>(Expression<Func<TProperty>> propertyExpression, Expression<Func<TProperty, State>> statePropertyExpression, State superState)
```

#### Type Parameters

`TProperty`<br/>

#### Parameters

`propertyExpression` Expression\<Func\<TProperty\>\><br/>

`statePropertyExpression` Expression\<Func\<TProperty, State\>\><br/>

`superState` [State](../../masstransit-abstractions/masstransit/state)<br/>

#### Returns

[IStateMachineModifier\<TSaga\>](../masstransit/istatemachinemodifier-1)<br/>

### **During(State[])**

```csharp
IStateMachineEventActivitiesBuilder<TSaga> During(State[] states)
```

#### Parameters

`states` [State[]](../../masstransit-abstractions/masstransit/state)<br/>

#### Returns

[IStateMachineEventActivitiesBuilder\<TSaga\>](../masstransit/istatemachineeventactivitiesbuilder-1)<br/>

### **Initially()**

```csharp
IStateMachineEventActivitiesBuilder<TSaga> Initially()
```

#### Returns

[IStateMachineEventActivitiesBuilder\<TSaga\>](../masstransit/istatemachineeventactivitiesbuilder-1)<br/>

### **DuringAny()**

```csharp
IStateMachineEventActivitiesBuilder<TSaga> DuringAny()
```

#### Returns

[IStateMachineEventActivitiesBuilder\<TSaga\>](../masstransit/istatemachineeventactivitiesbuilder-1)<br/>

### **Finally(Func\<EventActivityBinder\<TSaga\>, EventActivityBinder\<TSaga\>\>)**

```csharp
IStateMachineModifier<TSaga> Finally(Func<EventActivityBinder<TSaga>, EventActivityBinder<TSaga>> activityCallback)
```

#### Parameters

`activityCallback` [Func\<EventActivityBinder\<TSaga\>, EventActivityBinder\<TSaga\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IStateMachineModifier\<TSaga\>](../masstransit/istatemachinemodifier-1)<br/>

### **WhenEnter(State, Func\<EventActivityBinder\<TSaga\>, EventActivityBinder\<TSaga\>\>)**

```csharp
IStateMachineModifier<TSaga> WhenEnter(State state, Func<EventActivityBinder<TSaga>, EventActivityBinder<TSaga>> activityCallback)
```

#### Parameters

`state` [State](../../masstransit-abstractions/masstransit/state)<br/>

`activityCallback` [Func\<EventActivityBinder\<TSaga\>, EventActivityBinder\<TSaga\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IStateMachineModifier\<TSaga\>](../masstransit/istatemachinemodifier-1)<br/>

### **WhenEnterAny(Func\<EventActivityBinder\<TSaga\>, EventActivityBinder\<TSaga\>\>)**

```csharp
IStateMachineModifier<TSaga> WhenEnterAny(Func<EventActivityBinder<TSaga>, EventActivityBinder<TSaga>> activityCallback)
```

#### Parameters

`activityCallback` [Func\<EventActivityBinder\<TSaga\>, EventActivityBinder\<TSaga\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IStateMachineModifier\<TSaga\>](../masstransit/istatemachinemodifier-1)<br/>

### **WhenLeaveAny(Func\<EventActivityBinder\<TSaga\>, EventActivityBinder\<TSaga\>\>)**

```csharp
IStateMachineModifier<TSaga> WhenLeaveAny(Func<EventActivityBinder<TSaga>, EventActivityBinder<TSaga>> activityCallback)
```

#### Parameters

`activityCallback` [Func\<EventActivityBinder\<TSaga\>, EventActivityBinder\<TSaga\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IStateMachineModifier\<TSaga\>](../masstransit/istatemachinemodifier-1)<br/>

### **BeforeEnterAny(Func\<EventActivityBinder\<TSaga, State\>, EventActivityBinder\<TSaga, State\>\>)**

```csharp
IStateMachineModifier<TSaga> BeforeEnterAny(Func<EventActivityBinder<TSaga, State>, EventActivityBinder<TSaga, State>> activityCallback)
```

#### Parameters

`activityCallback` [Func\<EventActivityBinder\<TSaga, State\>, EventActivityBinder\<TSaga, State\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IStateMachineModifier\<TSaga\>](../masstransit/istatemachinemodifier-1)<br/>

### **AfterLeaveAny(Func\<EventActivityBinder\<TSaga, State\>, EventActivityBinder\<TSaga, State\>\>)**

```csharp
IStateMachineModifier<TSaga> AfterLeaveAny(Func<EventActivityBinder<TSaga, State>, EventActivityBinder<TSaga, State>> activityCallback)
```

#### Parameters

`activityCallback` [Func\<EventActivityBinder\<TSaga, State\>, EventActivityBinder\<TSaga, State\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IStateMachineModifier\<TSaga\>](../masstransit/istatemachinemodifier-1)<br/>

### **WhenLeave(State, Func\<EventActivityBinder\<TSaga\>, EventActivityBinder\<TSaga\>\>)**

```csharp
IStateMachineModifier<TSaga> WhenLeave(State state, Func<EventActivityBinder<TSaga>, EventActivityBinder<TSaga>> activityCallback)
```

#### Parameters

`state` [State](../../masstransit-abstractions/masstransit/state)<br/>

`activityCallback` [Func\<EventActivityBinder\<TSaga\>, EventActivityBinder\<TSaga\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IStateMachineModifier\<TSaga\>](../masstransit/istatemachinemodifier-1)<br/>

### **BeforeEnter(State, Func\<EventActivityBinder\<TSaga, State\>, EventActivityBinder\<TSaga, State\>\>)**

```csharp
IStateMachineModifier<TSaga> BeforeEnter(State state, Func<EventActivityBinder<TSaga, State>, EventActivityBinder<TSaga, State>> activityCallback)
```

#### Parameters

`state` [State](../../masstransit-abstractions/masstransit/state)<br/>

`activityCallback` [Func\<EventActivityBinder\<TSaga, State\>, EventActivityBinder\<TSaga, State\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IStateMachineModifier\<TSaga\>](../masstransit/istatemachinemodifier-1)<br/>

### **AfterLeave(State, Func\<EventActivityBinder\<TSaga, State\>, EventActivityBinder\<TSaga, State\>\>)**

```csharp
IStateMachineModifier<TSaga> AfterLeave(State state, Func<EventActivityBinder<TSaga, State>, EventActivityBinder<TSaga, State>> activityCallback)
```

#### Parameters

`state` [State](../../masstransit-abstractions/masstransit/state)<br/>

`activityCallback` [Func\<EventActivityBinder\<TSaga, State\>, EventActivityBinder\<TSaga, State\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IStateMachineModifier\<TSaga\>](../masstransit/istatemachinemodifier-1)<br/>

### **OnUnhandledEvent(UnhandledEventCallback\<TSaga\>)**

```csharp
IStateMachineModifier<TSaga> OnUnhandledEvent(UnhandledEventCallback<TSaga> callback)
```

#### Parameters

`callback` [UnhandledEventCallback\<TSaga\>](../../masstransit-abstractions/masstransit/unhandledeventcallback-1)<br/>

#### Returns

[IStateMachineModifier\<TSaga\>](../masstransit/istatemachinemodifier-1)<br/>

### **Apply()**

```csharp
void Apply()
```
