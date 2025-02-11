---

title: IgnoreEventActivityBinder<TInstance>

---

# IgnoreEventActivityBinder\<TInstance\>

Namespace: MassTransit.SagaStateMachine

```csharp
public class IgnoreEventActivityBinder<TInstance> : IActivityBinder<TInstance>
```

#### Type Parameters

`TInstance`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [IgnoreEventActivityBinder\<TInstance\>](../masstransit-sagastatemachine/ignoreeventactivitybinder-1)<br/>
Implements [IActivityBinder\<TInstance\>](../masstransit-sagastatemachine/iactivitybinder-1)

## Properties

### **Event**

```csharp
public Event Event { get; }
```

#### Property Value

[Event](../../masstransit-abstractions/masstransit/event)<br/>

## Constructors

### **IgnoreEventActivityBinder(Event)**

```csharp
public IgnoreEventActivityBinder(Event event)
```

#### Parameters

`event` [Event](../../masstransit-abstractions/masstransit/event)<br/>

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
