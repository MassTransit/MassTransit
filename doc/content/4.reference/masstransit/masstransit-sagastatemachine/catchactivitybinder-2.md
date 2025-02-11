---

title: CatchActivityBinder<TInstance, TException>

---

# CatchActivityBinder\<TInstance, TException\>

Namespace: MassTransit.SagaStateMachine

Creates a compensation activity with the compensation behavior

```csharp
public class CatchActivityBinder<TInstance, TException> : IActivityBinder<TInstance>
```

#### Type Parameters

`TInstance`<br/>

`TException`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CatchActivityBinder\<TInstance, TException\>](../masstransit-sagastatemachine/catchactivitybinder-2)<br/>
Implements [IActivityBinder\<TInstance\>](../masstransit-sagastatemachine/iactivitybinder-1)

## Properties

### **Event**

```csharp
public Event Event { get; }
```

#### Property Value

[Event](../../masstransit-abstractions/masstransit/event)<br/>

## Constructors

### **CatchActivityBinder(Event, EventActivities\<TInstance\>)**

```csharp
public CatchActivityBinder(Event event, EventActivities<TInstance> activities)
```

#### Parameters

`event` [Event](../../masstransit-abstractions/masstransit/event)<br/>

`activities` [EventActivities\<TInstance\>](../masstransit/eventactivities-1)<br/>

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
