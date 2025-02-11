---

title: IgnoreEventActivityBinder<TInstance, TData>

---

# IgnoreEventActivityBinder\<TInstance, TData\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class IgnoreEventActivityBinder<TInstance, TData> : IActivityBinder<TInstance>
```

#### Type Parameters

`TInstance`<br/>

`TData`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [IgnoreEventActivityBinder\<TInstance, TData\>](../masstransit-sagastatemachine/ignoreeventactivitybinder-2)<br/>
Implements [IActivityBinder\<TInstance\>](../masstransit-sagastatemachine/iactivitybinder-1)

## Properties

### **Event**

```csharp
public Event Event { get; }
```

#### Property Value

[Event](../../masstransit-abstractions/masstransit/event)<br/>

## Constructors

### **IgnoreEventActivityBinder(Event\<TData\>, StateMachineCondition\<TInstance, TData\>)**

```csharp
public IgnoreEventActivityBinder(Event<TData> event, StateMachineCondition<TInstance, TData> filter)
```

#### Parameters

`event` [Event\<TData\>](../../masstransit-abstractions/masstransit/event-1)<br/>

`filter` [StateMachineCondition\<TInstance, TData\>](../../masstransit-abstractions/masstransit/statemachinecondition-2)<br/>

## Methods

### **IsStateTransitionEvent(State)**

```csharp
public bool IsStateTransitionEvent(State state)
```

#### Parameters

`state` [State](../../masstransit-abstractions/masstransit/state)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Bind(State\<TInstance\>)**

```csharp
public void Bind(State<TInstance> state)
```

#### Parameters

`state` [State\<TInstance\>](../../masstransit-abstractions/masstransit/state-1)<br/>

### **Bind(IBehaviorBuilder\<TInstance\>)**

```csharp
public void Bind(IBehaviorBuilder<TInstance> builder)
```

#### Parameters

`builder` [IBehaviorBuilder\<TInstance\>](../masstransit-sagastatemachine/ibehaviorbuilder-1)<br/>
