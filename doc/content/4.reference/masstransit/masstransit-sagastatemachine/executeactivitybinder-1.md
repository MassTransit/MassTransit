---

title: ExecuteActivityBinder<TInstance>

---

# ExecuteActivityBinder\<TInstance\>

Namespace: MassTransit.SagaStateMachine

Routes event activities to an activities

```csharp
public class ExecuteActivityBinder<TInstance> : IActivityBinder<TInstance>
```

#### Type Parameters

`TInstance`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExecuteActivityBinder\<TInstance\>](../masstransit-sagastatemachine/executeactivitybinder-1)<br/>
Implements [IActivityBinder\<TInstance\>](../masstransit-sagastatemachine/iactivitybinder-1)

## Properties

### **Event**

```csharp
public Event Event { get; }
```

#### Property Value

[Event](../../masstransit-abstractions/masstransit/event)<br/>

## Constructors

### **ExecuteActivityBinder(Event, IStateMachineActivity\<TInstance\>)**

```csharp
public ExecuteActivityBinder(Event event, IStateMachineActivity<TInstance> activity)
```

#### Parameters

`event` [Event](../../masstransit-abstractions/masstransit/event)<br/>

`activity` [IStateMachineActivity\<TInstance\>](../../masstransit-abstractions/masstransit/istatemachineactivity-1)<br/>

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
